namespace StashMaven.WebApi.Features.Inventory.InventoryItems;

public partial class InventoryItemController
{
    [HttpGet]
    public async Task<IActionResult> ListInventoryItemsInStockpileAsync(
        [FromQuery]
        ListInventoryItemsInStockpile.ListInventoryItemsInStockpileRequest request,
        [FromServices]
        ListInventoryItemsInStockpile handler)
    {
        StashMavenResult<ListInventoryItemsInStockpile.ListInventoryItemsInStockpileResponse> response =
            await handler.ListInventoryItemsInStockpileAsync(request);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Message);
        }

        return Ok(response.Data);
    }
}

public class ListInventoryItemsInStockpile(StashMavenContext context)
{
    private const int MinPageSize = 5;
    private const int MaxPageSize = 100;
    private const int MinPage = 1;
    private const int MinSearchLength = 3;

    public class ListInventoryItemsInStockpileRequest
    {
        public required string StockpileId { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string? Search { get; set; }
        public string? SortBy { get; set; }
        public bool IsAscending { get; set; }
    }

    public class ListInventoryItem
    {
        public required string InventoryItemId { get; set; }
        public required string Sku { get; set; }
        public required string Name { get; set; }
        public decimal Quantity { get; set; }
    }

    public class ListInventoryItemsInStockpileResponse
    {
        public List<ListInventoryItem> InventoryItems { get; set; } = [];
        public int TotalCount { get; set; }
    }

    public async Task<StashMavenResult<ListInventoryItemsInStockpileResponse>> ListInventoryItemsInStockpileAsync(
        ListInventoryItemsInStockpileRequest request)
    {
        request.Page = Math.Max(request.Page, MinPage);
        request.PageSize = Math.Clamp(request.PageSize, MinPageSize, MaxPageSize);

        Stockpile? stockpile = await context.Stockpiles
            .Include(s => s.InventoryItems)
            .SingleOrDefaultAsync(s => s.StockpileId.Value == request.StockpileId);

        if (stockpile == null)
        {
            return StashMavenResult<ListInventoryItemsInStockpileResponse>.Error(
                $"Stockpile {request.StockpileId} not found");
        }

        IQueryable<InventoryItem> query = context.InventoryItems
            .Where(i => i.Stockpile.Id == stockpile.Id);

        if (!string.IsNullOrWhiteSpace(request.Search) && request.Search.Length >= MinSearchLength)
        {
            string search = $"%{request.Search}%";
            query = query.Where(i => EF.Functions.ILike(i.Sku, search)
                                     || EF.Functions.ILike(i.Name, search));
        }

        query = request.SortBy?.ToLowerInvariant() switch
        {
            "sku" when request.IsAscending => query.OrderBy(x => x.Sku),
            "sku" when !request.IsAscending => query.OrderByDescending(x => x.Sku),
            "name" when request.IsAscending => query.OrderBy(x => x.Name),
            "name" when !request.IsAscending => query.OrderByDescending(x => x.Name),
            _ => query.OrderBy(x => x.Name)
        };

        int totalCount = await query.CountAsync();

        List<ListInventoryItem> inventoryItems = await query
            .Skip(request.Page * request.PageSize)
            .Take(request.PageSize)
            .Select(i => new ListInventoryItem
            {
                InventoryItemId = i.InventoryItemId.Value,
                Sku = i.Sku,
                Name = i.Name,
                Quantity = i.Quantity,
            })
            .ToListAsync();

        return StashMavenResult<ListInventoryItemsInStockpileResponse>.Success(
            new ListInventoryItemsInStockpileResponse
            {
                InventoryItems = inventoryItems,
                TotalCount = totalCount,
            });
    }
}
