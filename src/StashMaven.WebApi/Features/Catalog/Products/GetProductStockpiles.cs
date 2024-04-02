using StashMaven.WebApi.Data.Services;

namespace StashMaven.WebApi.Features.Catalog.Products;

public partial class ProductController
{
    [HttpGet]
    [Route("{productId}/stockpiles")]
    [ProducesResponseType<GetProductStockpilesHandler.GetProductStockpilesResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<int>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetProductStockpilesAsync(
        string productId,
        [FromServices]
        GetProductStockpilesHandler handler)
    {
        StashMavenResult<GetProductStockpilesHandler.GetProductStockpilesResponse> result =
            await handler.GetProductStockpilesAsync(productId);

        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorCode);
        }

        return Ok(result.Data);
    }
}

[Injectable]
public class GetProductStockpilesHandler(
    StashMavenContext context,
    CacheReader cacheReader)
{
    public class GetProductStockpilesResponse
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

    public async Task<StashMavenResult<GetProductStockpilesResponse>> GetProductStockpilesAsync(
        string productId)
    {
        if (!await context.Products.AnyAsync(x => x.ProductId.Value == productId))
        {
            return StashMavenResult<GetProductStockpilesResponse>.Error(ErrorCodes.ProductNotFound);
        }

        IReadOnlyList<Stockpile> stockpiles = await cacheReader.GetStockpilesAsync();

        List<InventoryItem> items = await context.InventoryItems
            .Where(x => x.Product.ProductId.Value == productId)
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

        return StashMavenResult<GetProductStockpilesResponse>.Success(
            new GetProductStockpilesResponse
            {
                Stockpiles = stockpileInfos
            });
    }
}
