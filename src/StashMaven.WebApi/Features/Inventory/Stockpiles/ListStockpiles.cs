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
        public int Page { get; set; }
        public int PageSize { get; set; }
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

    public async Task<ListStockpilesResponse> ListStockpilesAsync(ListStockpilesRequest request)
    {
        request.PageSize = Math.Clamp(request.PageSize, MinPageSize, MaxPageSize);
        request.Page = Math.Max(request.Page, MinPage);
        
        IReadOnlyList<Stockpile> stockpiles = await cacheReader.GetStockpilesAsync();
        
        Stockpile? defaultStockpile = await cacheReader.GetDefaultStockpileAsync();
        
        IEnumerable<Stockpile> result = stockpiles.AsEnumerable();
        
        if (request.Search is not null)
        {
            result = result.Where(x => x.Name.Contains(request.Search, StringComparison.InvariantCultureIgnoreCase));
        }
        
        if (request.SortBy is not null)
        {
            result = request.SortBy.ToLowerInvariant() switch
            {
                "name" => request.IsAscending
                    ? result.OrderBy(x => x.Name)
                    : result.OrderByDescending(x => x.Name),
                "shortcode" => request.IsAscending
                    ? result.OrderBy(x => x.ShortCode)
                    : result.OrderByDescending(x => x.ShortCode),
                _ => result
            };
        }
        
        List<StockpileItem> stockpileItems = result.Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
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
            TotalCount = stockpiles.Count
        };
    }
}
