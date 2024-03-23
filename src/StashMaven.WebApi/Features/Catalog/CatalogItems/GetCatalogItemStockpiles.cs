using StashMaven.WebApi.Data.Services;

namespace StashMaven.WebApi.Features.Catalog.CatalogItems;

public partial class CatalogItemController
{
    [HttpGet]
    [Route("{catalogItemId}/stockpiles")]
    [ProducesResponseType<GetCatalogItemStockpilesHandler.GetCatalogItemStockpilesResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<int>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetCatalogItemStockpilesAsync(
        string catalogItemId,
        [FromServices]
        GetCatalogItemStockpilesHandler handler)
    {
        StashMavenResult<GetCatalogItemStockpilesHandler.GetCatalogItemStockpilesResponse> result =
            await handler.GetCatalogItemStockpilesAsync(catalogItemId);

        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorCode);
        }

        return Ok(result.Data);
    }
}

[Injectable]
public class GetCatalogItemStockpilesHandler(
    StashMavenContext context,
    CacheReader cacheReader)
{
    public class GetCatalogItemStockpilesResponse
    {
        public required ICollection<StockpileInfo> Stockpiles { get; set; }
    }

    public class StockpileInfo
    {
        public required string StockpileId { get; set; }
        public required string StockpileShortCode { get; set; }
        public required string StockpileName { get; set; }
        public decimal? Quantity { get; set; }
    }

    public async Task<StashMavenResult<GetCatalogItemStockpilesResponse>> GetCatalogItemStockpilesAsync(
        string catalogItemId)
    {
        if (!await context.CatalogItems.AnyAsync(x => x.CatalogItemId.Value == catalogItemId))
        {
            return StashMavenResult<GetCatalogItemStockpilesResponse>.Error(ErrorCodes.CatalogItemNotFound);
        }

        IReadOnlyList<Stockpile> stockpiles = await cacheReader.GetStockpilesAsync();

        List<InventoryItem> items = await context.InventoryItems
            .Where(x => x.CatalogItem.CatalogItemId.Value == catalogItemId)
            .Include(inventoryItem => inventoryItem.Stockpile)
            .ToListAsync();

        List<StockpileInfo> stockpileInfos = [];
        foreach (Stockpile stockpile in stockpiles)
        {
            InventoryItem? item = items.FirstOrDefault(x => x.Stockpile.Id == stockpile.Id);
            stockpileInfos.Add(new StockpileInfo
            {
                Quantity = item?.Quantity,
                StockpileId = stockpile.StockpileId.Value,
                StockpileShortCode = stockpile.ShortCode,
                StockpileName = stockpile.Name
            });
        }

        return StashMavenResult<GetCatalogItemStockpilesResponse>.Success(
            new GetCatalogItemStockpilesResponse
            {
                Stockpiles = stockpileInfos
            });
    }
}
