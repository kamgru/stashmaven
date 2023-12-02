using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using StashMaven.WebApi.Data;

namespace StashMaven.WebApi.Features.Inventory;

public partial class InventoryController
{
    [HttpPatch]
    [Route("shipment/{shipmentId}/accept")]
    public async Task<IActionResult> AcceptShipmentAsync(
        string shipmentId,
        [FromServices]
        AcceptShipment handler)
    {
        StashMavenResult response = await handler.AcceptShipmentAsync(shipmentId);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Message);
        }

        return Ok();
    }
}

[Injectable]
public class AcceptShipment(
    StashMavenContext context)
{
    public async Task<StashMavenResult> AcceptShipmentAsync(
        string shipmentId)
    {
        Shipment? shipment = await context.Shipments
            .Include(shipment => shipment.ShipmentRecords)
            .SingleOrDefaultAsync(s => s.ShipmentId.Value == shipmentId);

        if (shipment == null)
        {
            return StashMavenResult.Error($"Shipment {shipmentId} not found");
        }

        if (shipment.ShipmentAcceptance != ShipmentAcceptance.Pending)
        {
            return StashMavenResult.Error($"Shipment {shipmentId} is not pending");
        }

        shipment.ShipmentAcceptance = ShipmentAcceptance.Accepted;
        shipment.UpdatedOn = DateTime.UtcNow;

        foreach (IGrouping<InventoryItemId, ShipmentRecord> recordGroup
                 in shipment.ShipmentRecords.GroupBy(x => x.InventoryItemId))
        {
            InventoryItem? inventoryItem = await context.InventoryItems
                .SingleOrDefaultAsync(c => c.InventoryItemId.Value == recordGroup.Key.Value);

            if (inventoryItem == null)
            {
                return StashMavenResult.Error($"Inventory item {recordGroup.Key.Value} not found");
            }

            inventoryItem.Quantity += (int)shipment.ShipmentDirection * recordGroup.Sum(x => x.Quantity);
        }

        bool changesSaved = false;
        while (!changesSaved)
        {
            try
            {
                await context.SaveChangesAsync();
                changesSaved = true;
            }
            catch (DbUpdateConcurrencyException e)
            {
                foreach (EntityEntry entry in e.Entries)
                {
                    if (entry.Entity is not InventoryItem item)
                    {
                        continue;
                    }

                    PropertyValues proposedValues = entry.CurrentValues;
                    PropertyValues? databaseValues = entry.GetDatabaseValues();

                    if (databaseValues == null)
                    {
                        return StashMavenResult.Error(
                            $"Fatal error during concurrency resolution: database values for {item.InventoryItemId} are null");
                    }

                    foreach (IProperty property in proposedValues.Properties)
                    {
                        if (property.Name != nameof(InventoryItem.Quantity))
                        {
                            continue;
                        }

                        object? databaseValue = databaseValues[property];

                        if (databaseValue == null)
                        {
                            return StashMavenResult.Error(
                                $"Fatal error during concurrency resolution: database value for {item.InventoryItemId} is null");
                        }

                        decimal quantityChange = shipment.ShipmentRecords
                            .Where(r => r.InventoryItemId.Value == item.InventoryItemId.Value)
                            .Sum(r => r.Quantity * (int)shipment.ShipmentDirection);

                        proposedValues[property] =
                            (decimal)databaseValue + quantityChange;
                    }

                    entry.OriginalValues.SetValues(databaseValues);
                }
            }
        }

        return StashMavenResult.Success();
    }
}
