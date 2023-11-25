using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using StashMaven.WebApi.Data;

namespace StashMaven.WebApi.PartnerFeatures;

public class CreatePartnerHandler
{
    public class CreatePartnerRequest
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
            public required TaxIdentifierType TaxIdentifierType { get; set; }
            public required string Value { get; set; }
            public bool IsPrimary { get; set; }
        }

        [MinLength(3)]
        public required string CustomIdentifier { get; set; }

        public required string LegalName { get; set; }
        public required List<TaxIdentifier> BusinessIdentifications { get; set; }
        public required PartnerAddress Address { get; set; }
    }

    private readonly StashMavenContext _context;

    public CreatePartnerHandler(
        StashMavenContext context)
    {
        _context = context;
    }

    public async Task<Guid> CreatePartnerAsync(
        CreatePartnerRequest request)
    {
        Guid partnerId = Guid.NewGuid();

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
            TaxIdentifiers = request.BusinessIdentifications
                .Select(bi => new TaxIdentifier
                {
                    Type = bi.TaxIdentifierType,
                    Value = bi.Value,
                    IsPrimary = bi.IsPrimary,
                    CreatedOn = DateTime.UtcNow,
                    UpdatedOn = DateTime.UtcNow
                })
                .ToList()
        };

        _context.Partners.Add(partner);
        await _context.SaveChangesAsync();

        return partnerId;
    }
}
