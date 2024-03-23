using StashMaven.WebApi.Data.Services;

namespace StashMaven.WebApi.Features.Inventory.Stockpiles;

public partial class StockpileController
{
    [HttpPatch]
    [Route("{stockpileId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PatchStockpileAsync(
        [FromRoute] string stockpileId,
        [FromBody] PatchStockpileHandler.PatchStockpileRequest request,
        [FromServices] PatchStockpileHandler handler)
    {
        StashMavenResult result = await handler.PatchStockpileAsync(new StockpileId(stockpileId), request);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Message);
        }
        
        return Ok();
    }
}

[Injectable]
public class PatchStockpileHandler(
    StashMavenRepository repository,
    UnitOfWork unitOfWork,
    UpsertOptionService optionService,
    ShipmentService shipmentService,
    CacheReader cacheReader)
{
    public record PatchStockpileRequest(
        string ShortCode,
        string Name,
        bool IsDefault);

    public async Task<StashMavenResult> PatchStockpileAsync(
        StockpileId stockpileId,
        PatchStockpileRequest request)
    {
        Stockpile? stockpile = await repository.GetStockpileAsync(stockpileId);

        if (stockpile is null)
        {
            return StashMavenResult.Error("Stockpile not found");
        }

        if (!stockpile.ShortCode.Equals(request.ShortCode))
        {
            if (await shipmentService.CheckIfStockpileHasShipmentsAsync(stockpile.StockpileId))
            {
                return StashMavenResult.Error("Cannot change the short code of a stockpile that has shipments");
            }

            stockpile.ShortCode = request.ShortCode;
        }

        stockpile.Name = request.Name;

        if (request.IsDefault)
        {
            await optionService.UpsertStashMavenOptionAsync(
                StashMavenOption.Keys.DefaultStockpileShortCode,
                stockpile.ShortCode);
            
            cacheReader.InvalidateKey(CacheReader.Keys.DefaultStockpile);
        }

        await unitOfWork.SaveChangesAsync();
        cacheReader.InvalidateKey(CacheReader.Keys.Stockpiles);

        return StashMavenResult.Success();
    }
}
