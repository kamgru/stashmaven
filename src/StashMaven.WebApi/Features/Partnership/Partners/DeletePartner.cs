namespace StashMaven.WebApi.Features.Partnership.Partners;

public partial class PartnerController
{
    [HttpDelete]
    [Route("{partnerId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeletePartnerAsync(
        string partnerId,
        [FromServices]
        DeletePartnerHandler handler)
    {
        StashMavenResult result = await handler.DeletePartnerAsync(partnerId);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Message);
        }

        return NoContent();
    }
}

[Injectable]
public class DeletePartnerHandler(StashMavenContext context)
{
    public async Task<StashMavenResult> DeletePartnerAsync(
        string partnerId)
    {
        Partner? partner = await context.Partners
            .Include(p => p.Address)
            .Include(p => p.BusinessIdentifiers)
            .Include(p => p.Shipments)
            .FirstOrDefaultAsync(p => p.PartnerId.Value == partnerId);

        if (partner is null)
        {
            return StashMavenResult.Error(ErrorCodes.PartnerNotFound);
        }
        
        if (partner.Shipments.Any())
        {
            return StashMavenResult.Error(ErrorCodes.PartnerHasShipments);
        }
        
        context.Partners.Remove(partner);
        await context.SaveChangesAsync();
        
        return StashMavenResult.Success();
    }
}
