using StashMaven.WebApi.Data.Services;

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
            return BadRequest(result.ErrorCode);
        }

        return Created($"/api/v1/partner/{result.Data.PartnerId}", result.Data);
    }
}

[Injectable]
public class AddPartnerHandler(StashMavenContext context, CountryService countryService)
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

        public bool IsRetail { get; set; }
        public required List<TaxIdentifier> TaxIdentifiers { get; set; } = [];
        public required PartnerAddress Address { get; set; }
    }

    public record AddPartnerResponse(string PartnerId);

    public async Task<StashMavenResult<AddPartnerResponse>> AddPartnerAsync(
        AddPartnerRequest request)
    {
        PartnerId partnerId = new(Guid.NewGuid().ToString());

        if (context.Partners.Any(p => p.CustomIdentifier == request.CustomIdentifier))
        {
            return StashMavenResult<AddPartnerResponse>.Error(ErrorCodes.CustomIdentifierNotUnique);
        }

        if (request.TaxIdentifiers.GroupBy(ti => ti.Type).Any(g => g.Count() > 1))
        {
            return StashMavenResult<AddPartnerResponse>.Error(ErrorCodes.TaxIdentifierTypeNotUnique);
        }

        if (request.TaxIdentifiers.Any(ti => !Enum.IsDefined(typeof(TaxIdentifierType), ti.Type)))
        {
            return StashMavenResult<AddPartnerResponse>.Error(ErrorCodes.TaxIdentifierTypeNotSupported);
        }

        if (request.TaxIdentifiers.Count(ti => ti.IsPrimary) > 1)
        {
            return StashMavenResult<AddPartnerResponse>.Error(ErrorCodes.OnlyOnePrimaryTaxIdentifier);
        }

        if (!request.IsRetail)
        {
            if (request.TaxIdentifiers.Any(taxIdentifier =>
                    context.TaxIdentifiers.Any(ti => ti.Type == taxIdentifier.Type && ti.Value == taxIdentifier.Value)))
            {
                return StashMavenResult<AddPartnerResponse>.Error(ErrorCodes.TaxIdentifierValueNotUnique);
            }
        }

        StashMavenResult countryAvailableResult =
            await countryService.IsCountryAvailableAsync(request.Address.CountryCode);

        if (!countryAvailableResult.IsSuccess)
        {
            return StashMavenResult<AddPartnerResponse>.Error(ErrorCodes.CountryCodeNotSupported);
        }

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
