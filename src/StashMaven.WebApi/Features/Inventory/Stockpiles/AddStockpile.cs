using Npgsql;

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
        StashMavenResult<StockpileId> result = await handler.AddStockpileAsync(request);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Message);
        }

        return Created($"api/v1/inventory/stockpile/{result.Data!.Value}", result.Data);
    }
}

[Injectable]
public class AddStockpileHandler(StashMavenContext context)
{
    public class AddStockpileRequest
    {
        public required string Name { get; set; }
        public required string ShortCode { get; set; }
    }

    public async Task<StashMavenResult<StockpileId>> AddStockpileAsync(
        AddStockpileRequest request)
    {
        Stockpile stockpile = new()
        {
            StockpileId = new StockpileId(Guid.NewGuid().ToString()),
            Name = request.Name,
            ShortCode = request.ShortCode
        };

        await context.Stockpiles.AddAsync(stockpile);

        try
        {
            await context.SaveChangesAsync();
            return StashMavenResult<StockpileId>.Success(stockpile.StockpileId);
        }
        catch (DbUpdateException e)
        {
            if (e.InnerException is PostgresException { SqlState: StashMavenContext.PostgresUniqueViolation })
            {
                return StashMavenResult<StockpileId>.Error($"Stockpile {request.ShortCode} already exists.");
            }

            throw;
        }
    }
}
