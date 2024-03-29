namespace StashMaven.WebApi.Features.Inventory.Shipments;

public partial class ShipmentController
{
    [HttpPatch]
    [Route("{shipmentId}/partner")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangeShipmentPartnerAsync(
        string shipmentId,
        ChangeShipmentPartnerHandler.ChangeShipmentPartnerRequest request,
        [FromServices]
        ChangeShipmentPartnerHandler handler)
    {
        StashMavenResult response = await handler.ChangeShipmentPartnerAsync(shipmentId, request);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Message);
        }

        return Ok();
    }
}

[Injectable]
public class ChangeShipmentPartnerHandler(
    StashMavenContext context)
{
    public class ChangeShipmentPartnerRequest
    {
        public required string PartnerId { get; set; }
    }

    public async Task<StashMavenResult> ChangeShipmentPartnerAsync(
        string shipmentId,
        ChangeShipmentPartnerRequest request)
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
            .Include(partner => partner.TaxIdentifiers)
            .SingleOrDefaultAsync(p => p.PartnerId.Value == request.PartnerId);

        if (partner is null)
        {
            return StashMavenResult.Error($"Partner {request.PartnerId} not found");
        }

        if (partner.Address is null)
        {
            return StashMavenResult.Error($"Partner {request.PartnerId} has no address");
        }
        
        TaxIdentifier? primaryTaxIdentifier = partner.TaxIdentifiers.FirstOrDefault(x => x.IsPrimary);
        
        if (shipment.PartnerRefSnapshot is not null)
        {
            shipment.PartnerRefSnapshot.LegalName = partner.LegalName;
            shipment.PartnerRefSnapshot.Address = partner.Address.ToString();
            shipment.PartnerRefSnapshot.PartnerTaxId = new PartnerTaxIdSnapshot
            {
                TaxIdType = primaryTaxIdentifier?.Type.ToString(),
                TaxIdValue = primaryTaxIdentifier?.Value
            };
        }
        else
        {
            shipment.PartnerRefSnapshot = new PartnerRefSnapshot
            {
                LegalName = partner.LegalName,
                Address = partner.Address.ToString(),
                PartnerTaxId = new PartnerTaxIdSnapshot
                {
                    TaxIdType = primaryTaxIdentifier?.Type.ToString(),
                    TaxIdValue = primaryTaxIdentifier?.Value
                }
            };
        }

        shipment.Partner = partner;

        await context.SaveChangesAsync();

        return StashMavenResult.Success();
    }
}
