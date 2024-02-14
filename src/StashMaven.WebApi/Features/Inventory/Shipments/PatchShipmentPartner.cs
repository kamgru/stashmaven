namespace StashMaven.WebApi.Features.Inventory.Shipments;

public partial class ShipmentController
{
    [HttpPatch]
    [Route("{shipmentId}/partner")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PatchShipmentPartnerAsync(
        string shipmentId,
        PatchShipmentPartnerHandler.PatchShipmentPartnerRequest request,
        [FromServices]
        PatchShipmentPartnerHandler handler)
    {
        StashMavenResult response = await handler.PatchShipmentPartnerAsync(shipmentId, request);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Message);
        }

        return Ok();
    }
}

[Injectable]
public class PatchShipmentPartnerHandler(
    StashMavenContext context)
{
    public class PatchShipmentPartnerRequest
    {
        public required string PartnerId { get; set; }
    }

    public async Task<StashMavenResult> PatchShipmentPartnerAsync(
        string shipmentId,
        PatchShipmentPartnerRequest request)
    {
        Shipment? shipment = await context.Shipments
            .Include(x => x.PartnerRefSnapshot)
            .AsTracking()
            .SingleOrDefaultAsync(s => s.ShipmentId.Value == shipmentId);

        if (shipment is null)
        {
            return StashMavenResult.Error($"Shipment {shipmentId} not found");
        }

        Partner? partner = await context.Partners
            .AsTracking()
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
        
        if (shipment.PartnerRefSnapshot is not null)
        {
            shipment.PartnerRefSnapshot.LegalName = partner.LegalName;
            shipment.PartnerRefSnapshot.Address = partner.Address.ToString();
        }
        else
        {
            shipment.PartnerRefSnapshot = new PartnerRefSnapshot
            {
                LegalName = partner.LegalName,
                Address = partner.Address.ToString()
            };
        }

        shipment.Partner = partner;

        await context.SaveChangesAsync();

        return StashMavenResult.Success();
    }
}
