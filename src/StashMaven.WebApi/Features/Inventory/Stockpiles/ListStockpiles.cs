using StashMaven.WebApi.Data.Services;

namespace StashMaven.WebApi.Features.Inventory.Stockpiles;

public partial class StockpileController
{
    [HttpGet]
    [Route("list")]
    [ProducesResponseType<ListStockpilesHandler.ListStockpilesResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListStockpilesAsync(
        [FromServices]
        ListStockpilesHandler handler)
    {
        ListStockpilesHandler.ListStockpilesResponse response =
            await handler.ListStockpilesAsync();

        return Ok(response);
    }
}

[Injectable]
public class ListStockpilesHandler(CacheReader cacheReader)
{
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
    }

    public async Task<ListStockpilesResponse> ListStockpilesAsync()
    {
        IReadOnlyList<Stockpile> stockpiles = await cacheReader.GetStockpilesAsync();
        
        Stockpile? defaultStockpile = await cacheReader.GetDefaultStockpileAsync();

        return new ListStockpilesResponse
        {
            Items = stockpiles.Select(x => new StockpileItem
                {
                    StockpileId = x.StockpileId.Value,
                    Name = x.Name,
                    ShortCode = x.ShortCode,
                    IsDefault = defaultStockpile?.StockpileId == x.StockpileId
                })
                .ToList()
        };
    }
}
