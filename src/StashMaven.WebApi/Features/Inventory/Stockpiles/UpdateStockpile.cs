using Npgsql;
using StashMaven.WebApi.Data.Services;

namespace StashMaven.WebApi.Features.Inventory.Stockpiles;

public partial class StockpileController
{
    [HttpPut]
    [Route("{stockpileId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<int>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PatchStockpileAsync(
        [FromRoute]
        string stockpileId,
        [FromBody]
        UpdateStockpileHandler.UpdateStockpileRequest request,
        [FromServices]
        UpdateStockpileHandler handler)
    {
        StashMavenResult result = await handler.UpdateStockpileAsync(new StockpileId(stockpileId), request);

        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorCode);
        }

        return Ok();
    }
}

[Injectable]
public class UpdateStockpileHandler(
    StashMavenRepository repository,
    UnitOfWork unitOfWork,
    UpsertOptionService optionService,
    ShipmentService shipmentService,
    CacheReader cacheReader)
{
    public record UpdateStockpileRequest(
        string ShortCode,
        string Name,
        bool IsDefault);

    public async Task<StashMavenResult> UpdateStockpileAsync(
        StockpileId stockpileId,
        UpdateStockpileRequest request)
    {
        Stockpile? stockpile = await repository.GetStockpileAsync(stockpileId);

        if (stockpile is null)
        {
            return StashMavenResult.Error(ErrorCodes.StockpileNotFound);
        }

        if (!stockpile.ShortCode.Equals(request.ShortCode))
        {
            if (await shipmentService.CheckIfStockpileHasShipmentsAsync(stockpile.StockpileId))
            {
                return StashMavenResult.Error(ErrorCodes.StockpileHasShipments);
            }

            stockpile.ShortCode = request.ShortCode;
        }

        stockpile.Name = request.Name;
        
        Stockpile? defaultStockpile = await cacheReader.GetDefaultStockpileAsync();

        if (stockpile.StockpileId != defaultStockpile?.StockpileId && request.IsDefault)
        {
            await optionService.UpsertStashMavenOptionAsync(
                StashMavenOption.Keys.DefaultStockpileShortCode,
                stockpile.ShortCode);
            cacheReader.InvalidateKey(CacheReader.Keys.DefaultStockpile);
        }
        else if (stockpile.StockpileId == defaultStockpile?.StockpileId && !request.IsDefault)
        {
            return StashMavenResult.Error(ErrorCodes.DefaultStockpileRequired);
        }

        try
        {
            await unitOfWork.SaveChangesAsync();
            cacheReader.InvalidateKey(CacheReader.Keys.Stockpiles);
        }
        catch (DbUpdateException exception) when (exception.InnerException is PostgresException
                                                  {
                                                      SqlState: StashMavenContext.PostgresUniqueViolation
                                                  })
        {
            return StashMavenResult.Error(ErrorCodes.StockpileShortCodeNotUnique);
        }

        return StashMavenResult.Success();
    }
}
