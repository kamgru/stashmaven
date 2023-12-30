namespace StashMaven.WebApi.Features.Catalog.CatalogItems;

public partial class CatalogItemController
{
    [HttpGet]
    [Route("list")]
    public async Task<IActionResult> ListCatalogItemsAsync(
        [FromQuery]
        ListCatalogItemsHandler.ListCatalogItemsRequest request,
        [FromServices]
        ListCatalogItemsHandler handler)
    {
        ListCatalogItemsHandler.ListCatalogItemsResponse response =
            await handler.ListCatalogItemsAsync(request);
        return Ok(response);
    }
}

[Injectable]
public class ListCatalogItemsHandler(
    StashMavenContext context)
{
    private const int MinPageSize = 5;
    private const int MaxPageSize = 100;
    private const int MinPage = 1;
    private const int MinSearchLength = 3;

    public class CatalogItem
    {
        public required string CatalogItemId { get; set; }
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
        public List<CatalogItem> Items { get; set; } = [];
        public int TotalCount { get; set; }
    }

    public async Task<ListCatalogItemsResponse> ListCatalogItemsAsync(
        ListCatalogItemsRequest request)
    {
        request.PageSize = Math.Clamp(request.PageSize, MinPageSize, MaxPageSize);
        request.Page = Math.Max(request.Page, MinPage);

        await context.TaxDefinitions.ToListAsync();

        IQueryable<CatalogItem> catalogItems = context.CatalogItems
            .Select(c => new CatalogItem
            {
                CatalogItemId = c.CatalogItemId.Value,
                Sku = c.Sku,
                Name = c.Name,
                UnitOfMeasure = c.UnitOfMeasure,
                Tax = c.SellTax.Name,
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
            Items = catalogItemsList,
            TotalCount = totalCount
        };
    }
}
