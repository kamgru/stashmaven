namespace StashMaven.WebApi.Features.Inventory.Shipments;

public partial class ShipmentController
{
    [HttpPatch]
    [Route("{shipmentId}/add-partner")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
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

        if (shipment is null)
        {
            return StashMavenResult.Error($"Shipment {shipmentId} not found");
        }

        Partner? partner = await context.Partners
            .Include(partner => partner.Address)
            .SingleOrDefaultAsync(p => p.PartnerId.Value == request.PartnerId);

        if (partner is null)
        {
            return StashMavenResult.Error($"Partner {request.PartnerId} not found");
        }

        if (partner.Address is null)
        {
            return StashMavenResult.Error($"Partner {request.PartnerId} has no address");
        }

        shipment.PartnerRefSnapshot = new PartnerRefSnapshot
        {
            LegalName = partner.LegalName,
            Address = partner.Address.ToString() ?? ""
        };

        shipment.Partner = partner;

        await context.SaveChangesAsync();

        return StashMavenResult.Success();
    }
}
