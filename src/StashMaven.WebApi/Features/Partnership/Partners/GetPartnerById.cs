namespace StashMaven.WebApi.Features.Partnership.Partners;

public partial class PartnerController
{
    [HttpGet]
    [Route("{partnerId}")]
    public async Task<IActionResult> GetPartnerByIdAsync(
        string partnerId,
        [FromServices]
        GetPartnerByIdHandler handler)
    {
        StashMavenResult<GetPartnerByIdHandler.GetPartnerResponse> response =
            await handler.GetPartnerByIdAsync(partnerId);

        if (!response.IsSuccess)
        {
            return NotFound();
        }

        return Ok(response.Data);
    }
}

[Injectable]
public class GetPartnerByIdHandler(StashMavenContext context)
{
    public class GetPartnerResponse
    {
        public required string LegalName { get; set; }
        public required string CustomIdentifier { get; set; }
        public required List<TaxIdentifier> TaxIdentifiers { get; set; }
        public required PartnerAddress Address { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }

        public class TaxIdentifier
        {
            public required string Type { get; set; }
            public required string Value { get; set; }
            public bool IsPrimary { get; set; }
        }

        public class PartnerAddress
        {
            public required string Street { get; set; }
            public string? StreetAdditional { get; set; }
            public required string City { get; set; }
            public string? State { get; set; }
            public required string PostalCode { get; set; }
            public required string CountryCode { get; set; }
        }
    }

    public async Task<StashMavenResult<GetPartnerResponse>> GetPartnerByIdAsync(
        string partnerId)
    {
        Partner? partner = await context.Partners
            .Include(p => p.Address)
            .Include(p => p.BusinessIdentifiers)
            .FirstOrDefaultAsync(p => p.PartnerId.Value == partnerId);

        if (partner == null)
        {
            return StashMavenResult<GetPartnerResponse>.Error(ErrorCodes.PartnerNotFound);
        }

        GetPartnerResponse response = new()
        {
            LegalName = partner.LegalName,
            CustomIdentifier = partner.CustomIdentifier,
            TaxIdentifiers = partner.BusinessIdentifiers.Select(ti => new GetPartnerResponse.TaxIdentifier
                {
                    Type = ti.Type.ToString(),
                    Value = ti.Value,
                })
                .ToList(),
            Address = new GetPartnerResponse.PartnerAddress
            {
                Street = partner.Address!.Street,
                StreetAdditional = partner.Address.StreetAdditional,
                City = partner.Address.City,
                State = partner.Address.State,
                PostalCode = partner.Address.PostalCode,
                CountryCode = partner.Address.CountryCode
            },
            CreatedOn = partner.CreatedOn,
            UpdatedOn = partner.UpdatedOn
        };

        return StashMavenResult<GetPartnerResponse>.Success(response);
    }
}
