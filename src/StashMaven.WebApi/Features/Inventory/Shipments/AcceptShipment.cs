using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using StashMaven.WebApi.Data.Services;

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
    SequenceService sequenceService,
    StashMavenRepository repository,
    UnitOfWork unitOfWork)
{
    public async Task<StashMavenResult> AcceptShipmentAsync(
        ShipmentId shipmentId)
    {
        Shipment? shipment = await repository.GetShipmentAsync(shipmentId);

        if (shipment == null)
        {
            return StashMavenResult.Error(ErrorCodes.ShipmentNotFound, $"Shipment {shipmentId} not found");
        }

        if (shipment.PartnerRefSnapshot is null || shipment.Partner is null)
        {
            return StashMavenResult.Error(ErrorCodes.ShipmentHasNoPartner, $"Shipment {shipmentId} has no partner");
        }

        if (shipment.Acceptance != ShipmentAcceptance.Pending)
        {
            return StashMavenResult.Error(ErrorCodes.ShipmentNotPending, $"Shipment {shipmentId} is not pending");
        }

        if (shipment.SourceReference is null)
        {
            return StashMavenResult.Error(ErrorCodes.ShipmentHasNoSourceReference,
                $"Shipment {shipmentId} has no source reference");
        }

        shipment.Acceptance = ShipmentAcceptance.Accepted;
        shipment.UpdatedOn = DateTime.UtcNow;

        foreach (IGrouping<InventoryItem, ShipmentRecord> recordGroup
                 in shipment.Records.GroupBy(x => x.InventoryItem))
        {
            recordGroup.Key.Quantity += (int)shipment.Kind.Direction * recordGroup.Sum(x => x.Quantity);
        }

        try
        {
            StashMavenResult<ShipmentSequenceNumber> result = await sequenceService.GenerateShipmentSequence(shipment);
            if (!result.IsSuccess)
            {
                return StashMavenResult.Error(result);
            }

            shipment.SequenceNumber = result.Data;
        }
        catch (StashMavenException sme)
        {
            return StashMavenResult.Error(ErrorCodes.FatalError, sme.Message);
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
                                StashMavenResult<ShipmentSequenceNumber> sequenceResult =
                                    await sequenceService.GenerateShipmentSequence(shipment);
                                if (!sequenceResult.IsSuccess)
                                {
                                    return StashMavenResult.Error(sequenceResult);
                                }

                                shipment.SequenceNumber = sequenceResult.Data;
                            }
                            catch (StashMavenException sme)
                            {
                                return StashMavenResult.Error(ErrorCodes.FatalError, sme.Message);
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
            throw new StashMavenException(
                "Fatal error during concurrency resolution: entity is not InventoryItem. This should never happen.");
        }

        PropertyValues proposedValues = entry.CurrentValues;
        PropertyValues? databaseValues = entry.GetDatabaseValues();

        if (databaseValues == null)
        {
            return StashMavenResult.Error(
                ErrorCodes.ConcurrencyResolutionFailed,
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
                    ErrorCodes.ConcurrencyResolutionFailed,
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
}
