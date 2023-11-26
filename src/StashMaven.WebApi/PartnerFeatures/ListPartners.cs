using Microsoft.EntityFrameworkCore;
using StashMaven.WebApi.Data;

namespace StashMaven.WebApi.PartnerFeatures;

public class ListPartnersHandler
{
    private const int MinPageSize = 5;
    private const int MaxPageSize = 100;
    private const int MinPage = 1;
    private const int MinSearchLength = 3;

    public class Partner
    {
        public Guid PartnerId { get; set; }
        public required string LegalName { get; set; }
        public required string CustomIdentifier { get; set; }
        public required string Street { get; set; }
        public required string City { get; set; }
        public required string PostalCode { get; set; }
        public required string PrimaryTaxIdentifierType { get; set; }
        public required string PrimaryTaxIdentifierValue { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }

    public class ListPartnerRequest
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string? Search { get; set; }
        public bool IsAscending { get; set; }
        public string? SortBy { get; set; }
    }

    public class ListPartnerResponse
    {
        public List<Partner> Partners { get; set; } = new();
        public int TotalCount { get; set; }
    }

    private readonly StashMavenContext _context;

    public ListPartnersHandler(
        StashMavenContext context)
    {
        _context = context;
    }

    public async Task<ListPartnerResponse> ListPartnersAsync(
        ListPartnerRequest request)
    {
        request.PageSize = Math.Clamp(request.PageSize, MinPageSize, MaxPageSize);
        request.Page = Math.Max(request.Page, MinPage);

        IQueryable<Partner> partners = _context.Partners
            .Include(p => p.Address)
            .Include(p => p.TaxIdentifiers)
            .Select(p => new Partner
            {
                PartnerId = p.PartnerId,
                LegalName = p.LegalName,
                CustomIdentifier = p.CustomIdentifier,
                Street = p.Address!.Street,
                City = p.Address.City,
                PostalCode = p.Address.PostalCode,
                PrimaryTaxIdentifierType = p.TaxIdentifiers
                    .First(ti => ti.IsPrimary)
                    .Type.ToString(),
                PrimaryTaxIdentifierValue = p.TaxIdentifiers
                    .First(ti => ti.IsPrimary)
                    .Value,
                CreatedOn = p.CreatedOn,
                UpdatedOn = p.UpdatedOn
            });

        if (!string.IsNullOrWhiteSpace(request.Search) && request.Search.Length >= MinSearchLength)
        {
            string search = $"%{request.Search}%";
            partners = partners.Where(p =>
                EF.Functions.ILike(p.CustomIdentifier, search)
                || EF.Functions.ILike(p.LegalName, search)
                || EF.Functions.ILike(p.PrimaryTaxIdentifierValue, search));
        }

        if (!string.IsNullOrWhiteSpace(request.SortBy))
        {
            if (request.SortBy.Equals("customIdentifier", StringComparison.OrdinalIgnoreCase))
            {
                partners = request.IsAscending
                    ? partners.OrderBy(p => p.CustomIdentifier)
                    : partners.OrderByDescending(p => p.CustomIdentifier);
            }
            else if (request.SortBy.Equals("legalName", StringComparison.OrdinalIgnoreCase))
            {
                partners = request.IsAscending
                    ? partners.OrderBy(p => p.LegalName)
                    : partners.OrderByDescending(p => p.LegalName);
            }
            else
            {
                partners = request.IsAscending
                    ? partners.OrderBy(p => p.CreatedOn)
                    : partners.OrderByDescending(p => p.CreatedOn);
            }
        }

        int totalCount = await partners.CountAsync();
        List<Partner> partnersList = await partners
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return new ListPartnerResponse
        {
            Partners = partnersList,
            TotalCount = totalCount
        };
    }
}
