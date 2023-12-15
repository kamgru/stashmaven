namespace StashMaven.WebApi.Features.Inventory;

public partial class InventoryController
{
    [HttpGet]
    [Route("inventory-item/list")]
    public async Task<IActionResult> ListInventoryItemsAsync(
        [FromQuery]
        ListInventoryItemsHandler.ListInventoryItemsRequest request,
        [FromServices]
        ListInventoryItemsHandler handler)
    {
        StashMavenResult<ListInventoryItemsHandler.ListInventoryItemsResponse> response =
            await handler.ListInventoryItemsAsync(request);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Message);
        }

        return Ok(response.Data);
    }
}

[Injectable]
public class ListInventoryItemsHandler(StashMavenContext context)
{
    private const int MinPageSize = 5;
    private const int MaxPageSize = 100;
    private const int MinPage = 1;
    private const int MinSearchLength = 3;

    public class ListInventoryItemsRequest
    {
        public required string StockpileId { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string? Search { get; set; }
        public bool IsAscending { get; set; }
        public string? SortBy { get; set; }
    }

    public class ListInventoryItemsResponse
    {
        public List<InventoryItem> Items { get; set; } = [];
        public int TotalCount { get; set; }
    }

    public class InventoryItem
    {
        public required string InventoryItemId { get; set; }
        public required string Name { get; set; }
        public required string Sku { get; set; }
        public decimal Quantity { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal TaxRate { get; set; }
    }

    public async Task<StashMavenResult<ListInventoryItemsResponse>> ListInventoryItemsAsync(
        ListInventoryItemsRequest request)
    {
        request.PageSize = Math.Clamp(request.PageSize, MinPageSize, MaxPageSize);
        request.Page = Math.Max(request.Page, MinPage);

        List<TaxDefinition> taxDefinitions = await context.TaxDefinitions.ToListAsync();

        IQueryable<InventoryItem> inventoryItems = context.Stockpiles
            .Where(x => x.StockpileId.Value == request.StockpileId)
            .SelectMany(x => x.InventoryItems)
            .Select(i => new InventoryItem
            {
                InventoryItemId = i.InventoryItemId.Value,
                Name = i.Name,
                Sku = i.Sku,
                Quantity = i.Quantity,
                PurchasePrice = i.UnitPrice,
                TaxRate = taxDefinitions.First(t => t.TaxDefinitionId == i.TaxDefinitionId).Rate
            });

        if (!string.IsNullOrWhiteSpace(request.Search) && request.Search.Length >= MinSearchLength)
        {
            string search = $"%{request.Search}%";
            inventoryItems =
                inventoryItems.Where(i => EF.Functions.ILike(i.Sku, search)
                                          || EF.Functions.ILike(i.Name, search));
        }

        if (!string.IsNullOrWhiteSpace(request.SortBy))
        {
            inventoryItems = request.SortBy.ToLowerInvariant() switch
            {
                "name" => request.IsAscending
                    ? inventoryItems.OrderBy(i => i.Name)
                    : inventoryItems.OrderByDescending(i => i.Name),
                "sku" => request.IsAscending
                    ? inventoryItems.OrderBy(i => i.Sku)
                    : inventoryItems.OrderByDescending(i => i.Sku),
                _ => inventoryItems.OrderBy(x => x.Name)
            };
        }
        else
        {
            inventoryItems = inventoryItems.OrderBy(x => x.Name);
        }

        List<InventoryItem> items = await inventoryItems
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        int totalCount = await inventoryItems.CountAsync();

        return StashMavenResult<ListInventoryItemsResponse>.Success(new ListInventoryItemsResponse
        {
            Items = items,
            TotalCount = totalCount
        });
    }
}
