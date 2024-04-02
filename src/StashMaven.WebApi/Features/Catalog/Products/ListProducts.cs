namespace StashMaven.WebApi.Features.Catalog.Products;

public partial class ProductController
{
    [HttpGet]
    [Route("list")]
    public async Task<IActionResult> ListProductsAsync(
        [FromQuery]
        ListProductsHandler.ListProductsRequest request,
        [FromServices]
        ListProductsHandler handler)
    {
        ListProductsHandler.ListProductsResponse response =
            await handler.ListProductsAsync(request);
        return Ok(response);
    }
}

[Injectable]
public class ListProductsHandler(
    StashMavenContext context)
{
    private const int MinPageSize = 5;
    private const int MaxPageSize = 100;
    private const int MinPage = 1;
    private const int MinSearchLength = 3;

    public class Product
    {
        public required string ProductId { get; set; }
        public required string Sku { get; set; }
        public required string Name { get; set; }
        public UnitOfMeasure UnitOfMeasure { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public class ListProductsRequest
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string? Search { get; set; }
        public bool IsAscending { get; set; }
        public string? SortBy { get; set; }
    }

    public class ListProductsResponse
    {
        public List<Product> Items { get; set; } = [];
        public int TotalCount { get; set; }
    }

    public async Task<ListProductsResponse> ListProductsAsync(
        ListProductsRequest request)
    {
        request.PageSize = Math.Clamp(request.PageSize, MinPageSize, MaxPageSize);
        request.Page = Math.Max(request.Page, MinPage);

        IQueryable<Product> products = context.Products
            .Select(c => new Product
            {
                ProductId = c.ProductId.Value,
                Sku = c.Sku,
                Name = c.Name,
                UnitOfMeasure = c.UnitOfMeasure,
                CreatedOn = c.CreatedOn,
            });

        if (!string.IsNullOrWhiteSpace(request.Search) && request.Search.Length >= MinSearchLength)
        {
            string search = $"%{request.Search}%";
            products = products.Where(p =>
                EF.Functions.ILike(p.Sku, search)
                || EF.Functions.ILike(p.Name, search));
        }

        if (!string.IsNullOrWhiteSpace(request.SortBy))
        {
            if (request.SortBy.Equals("sku", StringComparison.OrdinalIgnoreCase))
            {
                products = request.IsAscending
                    ? products.OrderBy(p => p.Sku)
                    : products.OrderByDescending(p => p.Sku);
            }
            else if (request.SortBy.Equals("name", StringComparison.OrdinalIgnoreCase))
            {
                products = request.IsAscending
                    ? products.OrderBy(p => p.Name)
                    : products.OrderByDescending(p => p.Name);
            }
            else
            {
                products = request.IsAscending
                    ? products.OrderBy(p => p.CreatedOn)
                    : products.OrderByDescending(p => p.CreatedOn);
            }
        }

        int totalCount = await products.CountAsync();
        List<Product> productsList = await products
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return new ListProductsResponse
        {
            Items = productsList,
            TotalCount = totalCount
        };
    }
}
