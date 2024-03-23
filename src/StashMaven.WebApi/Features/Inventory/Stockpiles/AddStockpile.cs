using Npgsql;
using StashMaven.WebApi.Data.Services;

namespace StashMaven.WebApi.Features.Inventory.Stockpiles;

public partial class StockpileController
{
    [HttpPost]
    public async Task<IActionResult> AddStockpileAsync(
        [FromBody]
        AddStockpileHandler.AddStockpileRequest request,
        [FromServices]
        AddStockpileHandler handler)
    {
        StashMavenResult<AddStockpileHandler.AddStockpileResponse> result = await handler.AddStockpileAsync(request);

        if (!result.IsSuccess || result.Data is null)
        {
            return BadRequest(result.Message);
        }

        return Created($"api/v1/inventory/stockpile/{result.Data.StockpileId}", result.Data);
    }
}

[Injectable]
public class AddStockpileHandler(
    StashMavenRepository repository,
    UnitOfWork unitOfWork,
    UpsertOptionService optionService,
    CacheReader cacheReader)
{
    public record AddStockpileRequest(
        string Name,
        string ShortCode,
        bool IsDefault);

    public record AddStockpileResponse(
        string StockpileId);

    public async Task<StashMavenResult<AddStockpileResponse>> AddStockpileAsync(
        AddStockpileRequest request)
    {
        Stockpile stockpile = new()
        {
            StockpileId = new StockpileId(Guid.NewGuid().ToString()),
            Name = request.Name,
            ShortCode = request.ShortCode
        };

        repository.InsertStockpile(stockpile);

        if (request.IsDefault)
        {
            await optionService.UpsertStashMavenOptionAsync(
                StashMavenOption.Keys.DefaultStockpileShortCode,
                stockpile.ShortCode);
        }

        try
        {
            await unitOfWork.SaveChangesAsync();
            cacheReader.InvalidateKey(CacheReader.Keys.Stockpiles);

            return StashMavenResult<AddStockpileResponse>.Success(
                new AddStockpileResponse(stockpile.StockpileId.Value));
        }
        catch (DbUpdateException e)
        {
            if (e.InnerException is PostgresException { SqlState: StashMavenContext.PostgresUniqueViolation })
            {
                return StashMavenResult<AddStockpileResponse>.Error($"Stockpile {request.ShortCode} already exists.");
            }

            throw;
        }
    }
}
