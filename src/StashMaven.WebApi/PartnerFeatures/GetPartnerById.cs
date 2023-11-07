using Microsoft.EntityFrameworkCore;
using StashMaven.WebApi.Data;

namespace StashMaven.WebApi.PartnerFeatures;

public class GetPartnerByIdHandler
{
    public class GetPartnerResponse
    {
        public required string LegalName { get; set; }
        public required string CustomIdentifier { get; set; }
        public required string Street { get; set; }
        public string? StreetAdditional { get; set; }
        public required string City { get; set; }
        public string? State { get; set; }
        public required string PostalCode { get; set; }
        public required string CountryCode { get; set; }
        public required List<TaxIdentifier> TaxIdentifiers { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }

        public class TaxIdentifier
        {
            public required string Type { get; set; }
            public required string Value { get; set; }
            public bool IsPrimary { get; set; }
        }
    }

    private readonly StashMavenContext _context;

    public GetPartnerByIdHandler(
        StashMavenContext context)
    {
        _context = context;
    }

    public async Task<StashMavenResult<GetPartnerResponse>> GetPartnerByIdAsync(
        string partnerId)
    {
        if (!Guid.TryParse(partnerId, out Guid partnerGuid))
        {
            return StashMavenResult<GetPartnerResponse>.Error("Invalid partner id");
        }

        Partner? partner = await _context.Partners
            .Include(p => p.Address)
            .Include(p => p.TaxIdentifiers)
            .FirstOrDefaultAsync(p => p.PartnerId == partnerGuid);

        if (partner == null)
        {
            return StashMavenResult<GetPartnerResponse>.Error("Partner not found");
        }

        TaxIdentifier? primaryTaxIdentifier = partner.TaxIdentifiers
            .FirstOrDefault(ti => ti.IsPrimary);

        if (primaryTaxIdentifier == null)
        {
            throw new InvalidOperationException("Partner has no primary tax identifier");
        }

        if (partner.Address == null)
        {
            throw new InvalidOperationException("Partner has no address");
        }

        GetPartnerResponse response = new()
        {
            LegalName = partner.LegalName,
            CustomIdentifier = partner.CustomIdentifier,
            Street = partner.Address.Street,
            StreetAdditional = partner.Address.StreetAdditional,
            City = partner.Address.City,
            State = partner.Address.State,
            PostalCode = partner.Address.PostalCode,
            CountryCode = partner.Address.CountryCode,
            TaxIdentifiers = partner.TaxIdentifiers.Select(ti => new GetPartnerResponse.TaxIdentifier
                {
                    Type = ti.Type.ToString(),
                    Value = ti.Value,
                    IsPrimary = ti.IsPrimary
                })
                .ToList(),
            CreatedOn = partner.CreatedOn,
            UpdatedOn = partner.UpdatedOn
        };

        return StashMavenResult<GetPartnerResponse>.Success(response);
    }
}
