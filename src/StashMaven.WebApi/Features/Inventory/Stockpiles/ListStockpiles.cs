using StashMaven.WebApi.Data.Services;

namespace StashMaven.WebApi.Features.Inventory.Stockpiles;

public partial class StockpileController
{
    [HttpGet]
    [Route("list")]
    [ProducesResponseType<ListStockpilesHandler.ListStockpilesResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListStockpilesAsync(
        [FromQuery]
        ListStockpilesHandler.ListStockpilesRequest request,
        [FromServices]
        ListStockpilesHandler handler)
    {
        ListStockpilesHandler.ListStockpilesResponse response =
            await handler.ListStockpilesAsync(request);
        
        return Ok(response);
    }
}

[Injectable]
public class ListStockpilesHandler(CacheReader cacheReader)
{
    private const int MinPageSize = 5;
    private const int MaxPageSize = 100;
    private const int MinPage = 1;
    private const int MinSearchLength = 3;
    
    public class ListStockpilesRequest
    {
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public string? Search { get; set; }
        public bool IsAscending { get; set; }
        public string? SortBy { get; set; }
    }
    
    public class StockpileItem
    {
        public required string StockpileId { get; set; }
        public required string Name { get; set; }
        public required string ShortCode { get; set; }
        public bool IsDefault { get; set; }
    }
    
    public class ListStockpilesResponse
    {
        public List<StockpileItem> Items { get; set; } = [];
        public int TotalCount { get; set; }
    }
    
    public async Task<ListStockpilesResponse> ListStockpilesAsync(
        ListStockpilesRequest request)
    {
        IReadOnlyList<Stockpile> stockpiles = await cacheReader.GetStockpilesAsync();
        
        Stockpile? defaultStockpile = await cacheReader.GetDefaultStockpileAsync();
        
        if (request.Search is not null && request.Search.Length >= MinSearchLength)
        {
            stockpiles = stockpiles
                .Where(x => x.Name.Contains(request.Search, StringComparison.InvariantCultureIgnoreCase))
                .ToList();
        }
        
        if (request.SortBy is not null)
        {
            stockpiles = request.SortBy.ToLowerInvariant() switch
            {
                "name" => request.IsAscending
                    ? stockpiles.OrderBy(x => x.Name).ToList()
                    : stockpiles.OrderByDescending(x => x.Name).ToList(),
                "shortcode" => request.IsAscending
                    ? stockpiles.OrderBy(x => x.ShortCode).ToList()
                    : stockpiles.OrderByDescending(x => x.ShortCode).ToList(),
                _ => stockpiles
            };
        }
        
        int totalCount = stockpiles.Count;
        
        if (request is { Page: { } page, PageSize: { } pageSize })
        {
            request.Page = Math.Max(page, MinPage);
            request.PageSize = Math.Clamp(pageSize, MinPageSize, MaxPageSize);
            
            stockpiles = stockpiles.Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }
        
        List<StockpileItem> stockpileItems = stockpiles
            .Select(x => new StockpileItem
            {
                StockpileId = x.StockpileId.Value,
                Name = x.Name,
                ShortCode = x.ShortCode,
                IsDefault = defaultStockpile?.StockpileId == x.StockpileId
            })
            .ToList();
        
        return new ListStockpilesResponse
        {
            Items = stockpileItems,
            TotalCount = totalCount
        };
    }
}
