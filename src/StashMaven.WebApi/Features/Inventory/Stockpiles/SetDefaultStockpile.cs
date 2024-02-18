using StashMaven.WebApi.Data.Services;

namespace StashMaven.WebApi.Features.Inventory.Stockpiles;

public partial class StockpileController
{
    [HttpPost]
    [Route("{stockpileId}/default")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SetDefaultStockpileAsync(
        [FromRoute] string stockpileId,
        [FromServices] SetDefaultStockpileHandler handler)
    {
        StashMavenResult result = await handler.SetDefaultStockpileAsync(stockpileId);

        return result.IsSuccess
            ? Ok()
            : BadRequest(result.Message);
    }
    
}

[Injectable]
public class SetDefaultStockpileHandler(
    StashMavenRepository repository,
    UnitOfWork unitOfWork,
    UpsertOptionService optionService,
    CacheReader cacheReader)
{
    public async Task<StashMavenResult> SetDefaultStockpileAsync(
        string stockpileId)
    {
        Stockpile? stockpile = await repository.GetStockpileAsync(new StockpileId(stockpileId));

        if (stockpile is null)
        {
            return StashMavenResult.Error("Stockpile not found");
        }

        await optionService.UpsertStashMavenOptionAsync(
            StashMavenOption.Keys.DefaultStockpileShortCode,
            stockpile.ShortCode);
        
        await unitOfWork.SaveChangesAsync();
        
        cacheReader.InvalidateKey(CacheReader.Keys.DefaultStockpile);
        
        return StashMavenResult.Success();
    }
}
