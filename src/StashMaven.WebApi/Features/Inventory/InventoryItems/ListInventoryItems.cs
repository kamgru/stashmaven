using StashMaven.WebApi.Data.Services;

namespace StashMaven.WebApi.Features.Inventory.InventoryItems;

public partial class InventoryItemController
{
    [HttpGet]
    [Route("list")]
    [ProducesResponseType<ListInventoryItemsHandler.ListInventoryItemsRequest>(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListInventoryItems(
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
public class ListInventoryItemsHandler(StashMavenContext context, CacheReader cacheReader)
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
        public string? SortBy { get; set; }
        public bool IsAscending { get; set; }
    }

    public class InventoryItemListItem
    {
        public required string InventoryItemId { get; set; }
        public required string Sku { get; set; }
        public required string Name { get; set; }
        public decimal Quantity { get; set; }
        public UnitOfMeasure UnitOfMeasure { get; set; }
        public decimal LastPurchasePrice { get; set; }
    }

    public class ListInventoryItemsResponse
    {
        public List<InventoryItemListItem> Items { get; set; } = [];
        public int TotalCount { get; set; }
        public required string StockpileId { get; set; }
    }

    public async Task<StashMavenResult<ListInventoryItemsResponse>> ListInventoryItemsAsync(
        ListInventoryItemsRequest request)
    {
        request.Page = Math.Max(request.Page, MinPage);
        request.PageSize = Math.Clamp(request.PageSize, MinPageSize, MaxPageSize);

        Stockpile? stockpile;

        // if (string.IsNullOrWhiteSpace(request.StockpileId))
        // {
        //     // stockpile = await cacheReader.GetDefaultStockpileAsync();
        // }
        // else
        // {
        stockpile = await context.Stockpiles
            .Include(s => s.InventoryItems)
            .SingleOrDefaultAsync(s => s.StockpileId.Value == request.StockpileId);
        // }

        if (stockpile == null)
        {
            return StashMavenResult<ListInventoryItemsResponse>.Error(
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

        List<InventoryItemListItem> inventoryItems = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(i => new InventoryItemListItem
            {
                InventoryItemId = i.InventoryItemId.Value,
                Sku = i.Sku,
                Name = i.Name,
                Quantity = i.Quantity,
                UnitOfMeasure = i.UnitOfMeasure,
                LastPurchasePrice = i.LastPurchasePrice,
            })
            .ToListAsync();

        return StashMavenResult<ListInventoryItemsResponse>.Success(
            new ListInventoryItemsResponse
            {
                Items = inventoryItems,
                TotalCount = totalCount,
                StockpileId = stockpile.StockpileId.Value
            });
    }
}
