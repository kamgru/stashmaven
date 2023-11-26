using Microsoft.EntityFrameworkCore;
using StashMaven.WebApi.Data;

namespace StashMaven.WebApi.CatalogFeatures;

public class ListCatalogItemsHandler
{
    private const int MinPageSize = 5;
    private const int MaxPageSize = 100;
    private const int MinPage = 1;
    private const int MinSearchLength = 3;

    public class CatalogItem
    {
        public Guid CatalogItemId { get; set; }
        public required string Sku { get; set; }
        public required string Name { get; set; }
        public UnitOfMeasure UnitOfMeasure { get; set; }
        public required string Tax { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public class ListCatalogItemsRequest
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string? Search { get; set; }
        public bool IsAscending { get; set; }
        public string? SortBy { get; set; }
    }

    public class ListCatalogItemsResponse
    {
        public List<CatalogItem> CatalogItems { get; set; } = new();
        public int TotalCount { get; set; }
    }

    private readonly StashMavenContext _context;

    public ListCatalogItemsHandler(
        StashMavenContext context)
    {
        _context = context;
    }

    public async Task<ListCatalogItemsResponse> ListCatalogItemsAsync(
        ListCatalogItemsRequest request)
    {
        request.PageSize = Math.Clamp(request.PageSize, MinPageSize, MaxPageSize);
        request.Page = Math.Max(request.Page, MinPage);

        IQueryable<CatalogItem> catalogItems = _context.CatalogItems
            .Include(c => c.TaxDefinition)
            .Select(c => new CatalogItem
            {
                CatalogItemId = c.CatalogItemId,
                Sku = c.Sku,
                Name = c.Name,
                UnitOfMeasure = c.UnitOfMeasure,
                Tax = c.TaxDefinition!.Name,
                CreatedOn = c.CreatedOn,
            });

        if (!string.IsNullOrWhiteSpace(request.Search) && request.Search.Length >= MinSearchLength)
        {

            string search = $"%{request.Search}%";
            catalogItems = catalogItems.Where(p =>
                EF.Functions.ILike(p.Sku, search)
                || EF.Functions.ILike(p.Name, search));
        }

        if (!string.IsNullOrWhiteSpace(request.SortBy))
        {
            if (request.SortBy.Equals("sku", StringComparison.OrdinalIgnoreCase))
            {
                catalogItems = request.IsAscending
                    ? catalogItems.OrderBy(p => p.Sku)
                    : catalogItems.OrderByDescending(p => p.Sku);
            }
            else if (request.SortBy.Equals("legalName", StringComparison.OrdinalIgnoreCase))
            {
                catalogItems = request.IsAscending
                    ? catalogItems.OrderBy(p => p.Name)
                    : catalogItems.OrderByDescending(p => p.Name);
            }
            else
            {
                catalogItems = request.IsAscending
                    ? catalogItems.OrderBy(p => p.CreatedOn)
                    : catalogItems.OrderByDescending(p => p.CreatedOn);
            }
        }

        int totalCount = await catalogItems.CountAsync();
        List<CatalogItem> catalogItemsList = await catalogItems
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return new ListCatalogItemsResponse
        {
            CatalogItems = catalogItemsList,
            TotalCount = totalCount
        };
    }
}