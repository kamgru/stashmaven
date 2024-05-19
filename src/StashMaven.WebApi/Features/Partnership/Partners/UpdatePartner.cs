namespace StashMaven.WebApi.Features.Partnership.Partners;

public partial class PartnerController
{
    [HttpPatch]
    [Route("{partnerId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdatePartnerAsync(
        string partnerId,
        UpdatePartnerHandler.PatchPartnerRequest request,
        [FromServices]
        UpdatePartnerHandler handler)
    {
        StashMavenResult result = await handler.PatchPartnerAsync(partnerId, request);
        
        if (!result.IsSuccess)
        {
            return BadRequest(result.Message);
        }
        
        return Ok();
    }
}

[Injectable]
public class UpdatePartnerHandler(
    StashMavenContext context)
{
    public class AddressPatch
    {
        public required string Street { get; set; }
        public string? StreetAdditional { get; set; }
        public required string City { get; set; }
        public string? State { get; set; }
        public required string PostalCode { get; set; }
        public required string CountryCode { get; set; }
    }

    public class BusinessIdentifier
    {
        public required string Type { get; set; }
        public required string Value { get; set; }
    }

    public class PatchPartnerRequest
    {
        public string? CustomIdentifier { get; set; }
        public string? LegalName { get; set; }
        public List<BusinessIdentifier>? BusinessIdentifiers { get; set; }
        public AddressPatch? Address { get; set; }
    }

    public async Task<StashMavenResult> PatchPartnerAsync(
        string partnerId,
        PatchPartnerRequest request)
    {
        Partner? partner = await context.Partners
            .Include(p => p.Address)
            .Include(p => p.BusinessIdentifiers)
            .FirstOrDefaultAsync(p => p.PartnerId.Value == partnerId);

        if (partner is null)
        {
            return StashMavenResult.Error(ErrorCodes.PartnerNotFound);
        }

        if (request.CustomIdentifier is not null)
        {
            partner.CustomIdentifier = request.CustomIdentifier;
        }

        if (request.LegalName is not null)
        {
            partner.LegalName = request.LegalName;
        }

        if (request.Address is not null)
        {
            partner.Address = new Address
            {
                Street = request.Address.Street,
                StreetAdditional = request.Address.StreetAdditional,
                City = request.Address.City,
                State = request.Address.State,
                PostalCode = request.Address.PostalCode,
                CountryCode = request.Address.CountryCode,
            };
        }

        if (request.BusinessIdentifiers is not null)
        {
            partner.BusinessIdentifiers = request.BusinessIdentifiers
                .Select(bi => new Data.BusinessIdentifier
                {
                    Type = bi.Type,
                    Value = bi.Value,
                    IsPrimary = BusinessIdType.FromId(bi.Type).IsPrimary,
                    CreatedOn = DateTime.UtcNow,
                    UpdatedOn = DateTime.UtcNow
                })
                .ToList();
        }

        if (context.ChangeTracker.HasChanges())
        {
            partner.UpdatedOn = DateTime.UtcNow;
            await context.SaveChangesAsync();
        }
        
        return StashMavenResult.Success();
    }
}
