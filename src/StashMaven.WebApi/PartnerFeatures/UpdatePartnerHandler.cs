using Microsoft.EntityFrameworkCore;
using StashMaven.WebApi.Data;

namespace StashMaven.WebApi.PartnerFeatures;

public class UpdatePartnerHandler
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

    public class TaxIdentifierPatch
    {
        public required TaxIdentifierType Type { get; set; }
        public required string Value { get; set; }
        public bool IsPrimary { get; set; }
    }

    public class PatchPartnerRequest
    {
        public string? CustomIdentifier { get; set; }
        public string? LegalName { get; set; }
        public List<TaxIdentifierPatch>? TaxIdentifiers { get; set; }
        public AddressPatch? Address { get; set; }
    }

    private readonly StashMavenContext _context;

    public UpdatePartnerHandler(
        StashMavenContext context)
    {
        _context = context;
    }

    public async Task PatchPartnerAsync(
        Guid partnerId,
        PatchPartnerRequest request)
    {
        Partner partner = await _context.Partners
                              .Include(p => p.Address)
                              .Include(p => p.TaxIdentifiers)
                              .FirstOrDefaultAsync(p => p.PartnerId == partnerId)
                          ?? throw new EntityNotFoundException($"Partner with id {partnerId} not found");

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

        if (request.TaxIdentifiers is not null)
        {
            partner.TaxIdentifiers = request.TaxIdentifiers
                .Select(bi => new TaxIdentifier
                {
                    Type = bi.Type,
                    Value = bi.Value,
                    IsPrimary = bi.IsPrimary,
                    CreatedOn = DateTime.UtcNow,
                    UpdatedOn = DateTime.UtcNow
                })
                .ToList();
        }

        if (_context.ChangeTracker.HasChanges())
        {
            partner.UpdatedOn = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}
