using StashMaven.WebApi.Data.Services;

namespace StashMaven.WebApi.Features.Inventory.Stockpiles;

public partial class StockpileController
{
    [HttpGet]
    [Route("default")]
    [ProducesResponseType<GetDefaultStockpileHandler.GetDefaultStockpileResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetDefaultStockpileAsync(
        [FromServices]
        GetDefaultStockpileHandler handler)
    {
        StashMavenResult<GetDefaultStockpileHandler.GetDefaultStockpileResponse> result =
            await handler.GetDefaultStockpileAsync();

        return result.IsSuccess
            ? Ok(result.Data)
            : BadRequest(result.Message);
    }
}

[Injectable]
public class GetDefaultStockpileHandler(CacheReader cacheReader)
{
    public class GetDefaultStockpileResponse
    {
        public required string StockpileId { get; set; }
        public required string Name { get; set; }
        public required string ShortCode { get; set; }
    }

    public async Task<StashMavenResult<GetDefaultStockpileResponse>> GetDefaultStockpileAsync()
    {
        Stockpile? stockpile = await cacheReader.GetDefaultStockpileAsync();

        if (stockpile == null)
        {
            return StashMavenResult<GetDefaultStockpileResponse>.Error("Default stockpile not found");
        }

        return StashMavenResult<GetDefaultStockpileResponse>.Success(new GetDefaultStockpileResponse
        {
            StockpileId = stockpile.StockpileId.Value,
            Name = stockpile.Name,
            ShortCode = stockpile.ShortCode
        });
    }
}
