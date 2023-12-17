namespace StashMaven.WebApi.Features.Inventory.Shipments;

public partial class ShipmentController
{
    [HttpPatch]
    [Route("{shipmentId}/add-supplier")]
    public async Task<IActionResult> AddSupplierToShipmentAsync(
        string shipmentId,
        AddSupplierToShipmentHandler.AddSupplierToShipmentRequest request,
        [FromServices]
        AddSupplierToShipmentHandler handler)
    {
        StashMavenResult response = await handler.AddSupplierToShipmentAsync(shipmentId, request);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Message);
        }

        return Ok();
    }
}

[Injectable]
public class AddSupplierToShipmentHandler(
    StashMavenContext context)
{
    public class AddSupplierToShipmentRequest
    {
        public required string SupplierId { get; set; }
    }

    public async Task<StashMavenResult> AddSupplierToShipmentAsync(
        string shipmentId,
        AddSupplierToShipmentRequest request)
    {
        Shipment? shipment = await context.Shipments
            .SingleOrDefaultAsync(s => s.ShipmentId.Value == shipmentId);

        if (shipment == null)
        {
            return StashMavenResult.Error($"Shipment {shipmentId} not found");
        }

        Partner? supplier = await context.Partners
            .SingleOrDefaultAsync(p => p.PartnerId.Value == request.SupplierId);

        if (supplier == null)
        {
            return StashMavenResult.Error($"Supplier {request.SupplierId} not found");
        }

        shipment.SupplierId = new SupplierId(supplier.PartnerId.Value);
        await context.SaveChangesAsync();

        return StashMavenResult.Success();
    }
}
