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
            .Include(s => s.Kind)
            .Include(s => s.Records)
            .ThenInclude(r => r.InventoryItem)
            .FirstOrDefaultAsync(s => s.ShipmentId.Value == shipmentId);

        if (shipment == null)
        {
            return StashMavenResult.Error($"Shipment {shipmentId} not found");
        }

        if (shipment.SupplierId is null)
        {
            return StashMavenResult.Error($"Shipment {shipmentId} has no supplier");
        }

        if (shipment.ShipmentAcceptance != ShipmentAcceptance.Pending)
        {
            return StashMavenResult.Error($"Shipment {shipmentId} is not pending");
        }

        shipment.ShipmentAcceptance = ShipmentAcceptance.Accepted;
        shipment.UpdatedOn = DateTime.UtcNow;

        foreach (IGrouping<InventoryItem, ShipmentRecord> recordGroup
                 in shipment.Records.GroupBy(x => x.InventoryItem))
        {
            recordGroup.Key.Quantity += (int)shipment.Kind.ShipmentDirection * recordGroup.Sum(x => x.Quantity);
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
                    if (entry.Entity is not InventoryItem currentInventoryItem)
                    {
                        continue;
                    }

                    PropertyValues proposedValues = entry.CurrentValues;
                    PropertyValues? databaseValues = entry.GetDatabaseValues();

                    if (databaseValues == null)
                    {
                        return StashMavenResult.Error(
                            $"Fatal error during concurrency resolution: database values for {currentInventoryItem.InventoryItemId} are null");
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
                                $"Fatal error during concurrency resolution: database value for {currentInventoryItem.InventoryItemId} is null");
                        }

                        decimal quantityChange = shipment.Records
                            .Where(x => x.InventoryItem == currentInventoryItem)
                            .Sum(x => x.Quantity * (int)shipment.Kind.ShipmentDirection);

                        proposedValues[property] = (decimal)databaseValue + quantityChange;
                    }

                    entry.OriginalValues.SetValues(databaseValues);
                }
            }
        }

        return StashMavenResult.Success();
    }
}
