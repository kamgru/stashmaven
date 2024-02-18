using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;

namespace StashMaven.WebApi.Features.Inventory.Shipments;

public partial class ShipmentController
{
    [HttpPost]
    [Route("{shipmentId}/accept")]
    public async Task<IActionResult> AcceptShipmentAsync(
        string shipmentId,
        [FromServices]
        AcceptShipmentHandler handler)
    {
        StashMavenResult response = await handler.AcceptShipmentAsync(new ShipmentId(shipmentId));

        if (!response.IsSuccess)
        {
            return BadRequest(response.Message);
        }

        return Ok();
    }
}

[Injectable]
public class AcceptShipmentHandler(
    StashMavenRepository repository,
    UnitOfWork unitOfWork)
{
    public async Task<StashMavenResult> AcceptShipmentAsync(
        ShipmentId shipmentId)
    {
        Shipment? shipment = await repository.GetShipmentAsync(shipmentId);

        if (shipment == null)
        {
            return StashMavenResult.Error($"Shipment {shipmentId} not found");
        }

        if (shipment.PartnerRefSnapshot is null || shipment.Partner is null)
        {
            return StashMavenResult.Error($"Shipment {shipmentId} has no partner");
        }

        if (shipment.Acceptance != ShipmentAcceptance.Pending)
        {
            return StashMavenResult.Error($"Shipment {shipmentId} is not pending");
        }

        shipment.Acceptance = ShipmentAcceptance.Accepted;
        shipment.UpdatedOn = DateTime.UtcNow;

        foreach (IGrouping<InventoryItem, ShipmentRecord> recordGroup
                 in shipment.Records.GroupBy(x => x.InventoryItem))
        {
            recordGroup.Key.Quantity += (int)shipment.Kind.Direction * recordGroup.Sum(x => x.Quantity);
        }

        SequenceGenerator? sequenceGenerator =
            await repository.GetSequenceGeneratorAsync(shipment.Kind.SequenceGeneratorId);

        if (sequenceGenerator == null)
        {
            return StashMavenResult.Error($"Sequence generator {shipment.Kind.SequenceGeneratorId.Value} not found.");
        }

        try
        {
            shipment.SequenceNumber = shipment.SequenceNumber = GenerateNextSequence(
                sequenceGenerator,
                shipment.Stockpile,
                shipment.Kind);
        }
        catch (StashMavenException sme)
        {
            return StashMavenResult.Error(sme.Message);
        }

        bool changesSaved = false;
        while (!changesSaved)
        {
            try
            {
                await unitOfWork.SaveChangesAsync();
                changesSaved = true;
            }
            catch (DbUpdateConcurrencyException e)
            {
                foreach (EntityEntry entry in e.Entries)
                {
                    switch (entry.Entity)
                    {
                        case InventoryItem:
                            StashMavenResult result =
                                ResolveInventoryItemConcurrency(entry, shipment);
                            if (!result.IsSuccess)
                            {
                                return result;
                            }

                            break;
                        case SequenceEntry:
                            await entry.ReloadAsync();
                            try
                            {
                                shipment.SequenceNumber = GenerateNextSequence(
                                    sequenceGenerator,
                                    shipment.Stockpile,
                                    shipment.Kind);
                            }
                            catch (StashMavenException sme)
                            {
                                return StashMavenResult.Error(sme.Message);
                            }

                            break;
                    }
                }
            }
        }

        return StashMavenResult.Success();
    }

    private StashMavenResult ResolveInventoryItemConcurrency(
        EntityEntry entry,
        Shipment shipment)
    {
        if (entry.Entity is not InventoryItem currentInventoryItem)
        {
            throw new StashMavenException("");
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
                .Sum(x => x.Quantity * (int)shipment.Kind.Direction);

            proposedValues[property] = (decimal)databaseValue + quantityChange;
        }

        entry.OriginalValues.SetValues(databaseValues);

        return StashMavenResult.Success();
    }

    private ShipmentSequenceNumber GenerateNextSequence(
        SequenceGenerator sequenceGenerator,
        Stockpile stockpile,
        ShipmentKind shipmentKind)
    {
        SequenceEntry? entry = sequenceGenerator.Entries
            .FirstOrDefault(x => x.Delimiter == stockpile.ShortCode && x.Group == shipmentKind.ShortCode);

        if (entry == null)
        {
            throw new StashMavenException(
                $"Sequence entry for stockpile {stockpile.Name} not found. This should never happen.");
        }

        string shipmentSequenceIdentifier =
            $"{entry.Group} {entry.NextValue}/{entry.Delimiter}/{DateTime.UtcNow:yy}";

        entry.NextValue += 1;

        return new ShipmentSequenceNumber(shipmentSequenceIdentifier);
    }
}
