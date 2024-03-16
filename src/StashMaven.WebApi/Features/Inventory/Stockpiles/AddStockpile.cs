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
    StashMavenContext context,
    CacheReader cacheReader)
{
    public class AddStockpileRequest
    {
        public required string Name { get; set; }
        public required string ShortCode { get; set; }
    }

    public record AddStockpileResponse(string StockpileId);

    public async Task<StashMavenResult<AddStockpileResponse>> AddStockpileAsync(
        AddStockpileRequest request)
    {
        Stockpile stockpile = new()
        {
            StockpileId = new StockpileId(Guid.NewGuid().ToString()),
            Name = request.Name,
            ShortCode = request.ShortCode
        };

        List<ShipmentKind> shipmentKinds = await context.ShipmentKinds.ToListAsync();

        SequenceGenerator sequenceGenerator = new()
        {
            SequenceGeneratorId = new SequenceGeneratorId(Guid.NewGuid().ToString()),
            Version = 0,
        };

        sequenceGenerator.Entries = shipmentKinds.Select(x => new SequenceEntry
            {
                Group = request.ShortCode,
                Delimiter = x.ShortCode,
                NextValue = 1,
                SequenceGenerator = sequenceGenerator,
                Version = 0
            })
            .ToList();

        await context.Stockpiles.AddAsync(stockpile);

        try
        {
            await context.SaveChangesAsync();
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
