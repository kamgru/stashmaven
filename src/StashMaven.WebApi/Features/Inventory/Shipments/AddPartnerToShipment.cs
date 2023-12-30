namespace StashMaven.WebApi.Features.Inventory.Shipments;

public partial class ShipmentController
{
    [HttpPatch]
    [Route("{shipmentId}/add-partner")]
    public async Task<IActionResult> AddPartnerToShipmentAsync(
        string shipmentId,
        AddPartnerToShipmentHandler.AddPartnerToShipmentRequest request,
        [FromServices]
        AddPartnerToShipmentHandler handler)
    {
        StashMavenResult response = await handler.AddPartnerToShipmentAsync(shipmentId, request);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Message);
        }

        return Ok();
    }
}

[Injectable]
public class AddPartnerToShipmentHandler(
    StashMavenContext context)
{
    public class AddPartnerToShipmentRequest
    {
        public required string PartnerId { get; set; }
    }

    public async Task<StashMavenResult> AddPartnerToShipmentAsync(
        string shipmentId,
        AddPartnerToShipmentRequest request)
    {
        Shipment? shipment = await context.Shipments
            .SingleOrDefaultAsync(s => s.ShipmentId.Value == shipmentId);

        if (shipment == null)
        {
            return StashMavenResult.Error($"Shipment {shipmentId} not found");
        }

        Partner? partner = await context.Partners
            .SingleOrDefaultAsync(p => p.PartnerId.Value == request.PartnerId);

        if (partner == null)
        {
            return StashMavenResult.Error($"Partner {request.PartnerId} not found");
        }

        shipment.PartnerReference = new ShipmentPartnerReference
        {
            LegalName = partner.LegalName,
            Address = partner.Address?.ToString() ?? ""
        };

        shipment.Partner = partner;

        await context.SaveChangesAsync();

        return StashMavenResult.Success();
    }
}
