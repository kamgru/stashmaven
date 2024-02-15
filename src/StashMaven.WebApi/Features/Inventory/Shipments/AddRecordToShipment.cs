namespace StashMaven.WebApi.Features.Inventory.Shipments;

public partial class ShipmentController
{
    [HttpPatch]
    [Route("{shipmentId}/record")]
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
            .AsTracking()
            .Include(shipment => shipment.Records)
            .Include(shipment => shipment.Kind)
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

        shipment.Records.Add(new ShipmentRecord
        {
            Quantity = request.Quantity,
            UnitOfMeasure = inventoryItem.UnitOfMeasure,
            UnitPrice = request.UnitPrice,
            TaxRate = shipment.Kind.Direction switch
            {
                ShipmentDirection.In => inventoryItem.BuyTax.Rate,
                ShipmentDirection.Out => inventoryItem.SellTax.Rate,
                _ => -1
            },
            InventoryItem = inventoryItem
        });

        await context.SaveChangesAsync();

        return StashMavenResult.Success();
    }
}
