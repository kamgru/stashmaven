using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StashMaven.WebApi.Data;

namespace StashMaven.WebApi.Features.Inventory;

public partial class InventoryController
{
    [HttpPatch]
    [Route("shipment/{shipmentId}/add-record")]
    public async Task<IActionResult> AddRecordToShipmentAsync(
        string shipmentId,
        AddRecordToShipment.AddRecordToShipmentRequest request,
        [FromServices]
        AddRecordToShipment handler)
    {
        StashMavenResult response = await handler.AddRecordToShipmentAsync(shipmentId, request);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Message);
        }

        return Ok();
    }
}

[Injectable]
public class AddRecordToShipment(
    StashMavenContext context)
{
    public class AddRecordToShipmentRequest
    {
        public required string InventoryItemId { get; set; }
        public required decimal Quantity { get; set; }
        public required decimal UnitPrice { get; set; }
    }

    public async Task<StashMavenResult> AddRecordToShipmentAsync(
        string shipmentId,
        AddRecordToShipmentRequest request)
    {
        Shipment? shipment = await context.Shipments
            .Include(shipment => shipment.Records)
            .SingleOrDefaultAsync(s => s.ShipmentId.Value == shipmentId);

        if (shipment == null)
        {
            return StashMavenResult.Error($"Shipment {shipmentId} not found");
        }

        InventoryItem? inventoryItem = await context.InventoryItems
            .SingleOrDefaultAsync(c => c.InventoryItemId.Value == request.InventoryItemId);

        if (inventoryItem == null)
        {
            return StashMavenResult.Error($"Inventory item {request.InventoryItemId} not found");
        }

        TaxDefinition? taxDefinition = await context.TaxDefinitions
            .SingleOrDefaultAsync(t => t.TaxDefinitionId.Value == inventoryItem.TaxDefinitionId.Value);

        if (taxDefinition == null)
        {
            return StashMavenResult.Error($"Tax definition {inventoryItem.TaxDefinitionId.Value} not found");
        }

        shipment.Records.Add(new ShipmentRecord
        {
            Quantity = request.Quantity,
            UnitOfMeasure = inventoryItem.UnitOfMeasure,
            UnitPrice = request.UnitPrice,
            TaxRate = taxDefinition.Rate,
            InventoryItem = inventoryItem
        });

        await context.SaveChangesAsync();

        return StashMavenResult.Success();
    }
}
