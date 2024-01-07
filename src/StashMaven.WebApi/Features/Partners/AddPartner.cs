namespace StashMaven.WebApi.Features.Partners;

public partial class PartnerController
{
    [HttpPost]
    [ProducesResponseType<AddPartnerHandler.AddPartnerResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddPartnerAsync(
        AddPartnerHandler.AddPartnerRequest request,
        [FromServices]
        AddPartnerHandler handler)
    {
        StashMavenResult<AddPartnerHandler.AddPartnerResponse> result = await handler.AddPartnerAsync(request);

        if (!result.IsSuccess || result.Data is null)
        {
            return BadRequest(result.Message);
        }

        return Created($"/api/v1/partner/{result.Data.PartnerId}", result.Data);
    }
}

[Injectable]
public class AddPartnerHandler(StashMavenContext context)
{
    public class AddPartnerRequest
    {
        public class PartnerAddress
        {
            public required string Street { get; set; }
            public string? StreetAdditional { get; set; }
            public required string City { get; set; }
            public string? State { get; set; }
            public required string PostalCode { get; set; }

            [MinLength(2)]
            [MaxLength(3)]
            public required string CountryCode { get; set; }
        }

        public class TaxIdentifier
        {
            public required TaxIdentifierType Type { get; set; }
            public required string Value { get; set; }
            public bool IsPrimary { get; set; }
        }

        [MinLength(Partner.CustomIdentifierMinLength)]
        [MaxLength(Partner.CustomIdentifierMaxLength)]
        public required string CustomIdentifier { get; set; }

        [MinLength(Partner.LegalNameMinLength)]
        [MaxLength(Partner.LegalNameMaxLength)]
        public required string LegalName { get; set; }

        public required List<TaxIdentifier> TaxIdentifiers { get; set; } = [];
        public required PartnerAddress Address { get; set; }
    }

    public record AddPartnerResponse(string PartnerId);

    public async Task<StashMavenResult<AddPartnerResponse>> AddPartnerAsync(
        AddPartnerRequest request)
    {
        PartnerId partnerId = new(Guid.NewGuid().ToString());

        //TODO: validation:
        // - null checks, duh
        // - CustomIdentifier must be unique
        // - TaxIdentifierType must be unique
        // - TaxIdentifierType must be valid
        // - CountryCode must be valid
        // - Address must be valid
        // - Only a single tax identifier can be primary

        Partner partner = new()
        {
            LegalName = request.LegalName,
            CustomIdentifier = request.CustomIdentifier,
            PartnerId = partnerId,
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow,
            Address = new Address
            {
                Street = request.Address.Street,
                StreetAdditional = request.Address.StreetAdditional,
                City = request.Address.City,
                State = request.Address.State,
                PostalCode = request.Address.PostalCode,
                CountryCode = request.Address.CountryCode,
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow
            },
            TaxIdentifiers = request.TaxIdentifiers
                .Select(bi => new TaxIdentifier
                {
                    Type = bi.Type,
                    Value = bi.Value,
                    IsPrimary = bi.IsPrimary,
                    CreatedOn = DateTime.UtcNow,
                    UpdatedOn = DateTime.UtcNow
                })
                .ToList()
        };

        context.Partners.Add(partner);
        await context.SaveChangesAsync();

        return StashMavenResult<AddPartnerResponse>.Success(new AddPartnerResponse(partnerId.Value));
    }
}
