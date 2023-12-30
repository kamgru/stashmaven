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
public class GetDefaultStockpileHandler(StashMavenContext context)
{
    public class GetDefaultStockpileResponse
    {
        public required string StockpileId { get; set; }
        public required string Name { get; set; }
        public required string ShortCode { get; set; }
    }

    public async Task<StashMavenResult<GetDefaultStockpileResponse>> GetDefaultStockpileAsync()
    {
        StashMavenOption? defaultStockpileOption = await context.StashMavenOptions
            .FirstOrDefaultAsync(x => x.Key == StashMavenOption.Keys.DefaultStockpileShortCode);

        if (defaultStockpileOption == null)
        {
            return StashMavenResult<GetDefaultStockpileResponse>.Error("Default stockpile not set");
        }

        Stockpile? stockpile = await context.Stockpiles
            .FirstOrDefaultAsync(x => x.ShortCode == defaultStockpileOption.Value);

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
