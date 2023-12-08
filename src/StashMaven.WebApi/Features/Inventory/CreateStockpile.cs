using Microsoft.AspNetCore.Mvc;
using StashMaven.WebApi.Data;

namespace StashMaven.WebApi.Features.Inventory;

public partial class InventoryController
{
    [HttpPost]
    [Route("stockpile")]
    public async Task<IActionResult> CreateStockpileAsync(
        [FromBody]
        CreateStockpileHandler.CreateStockpileRequest request,
        [FromServices]
        CreateStockpileHandler handler)
    {
        StashMavenResult<StockpileId> result = await handler.CreateStockpileAsync(request);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Message);
        }

        return Created($"api/v1/inventory/stockpile/{result.Data!.Value}", result.Data);
    }
}

[Injectable]
public class CreateStockpileHandler(StashMavenContext context)
{
    public class CreateStockpileRequest
    {
        public required string Name { get; set; }
    }

    public async Task<StashMavenResult<StockpileId>> CreateStockpileAsync(
        CreateStockpileRequest request)
    {
        Stockpile stockpile = new()
        {
            StockpileId = new StockpileId(Guid.NewGuid().ToString()),
            Name = request.Name,
        };

        await context.Stockpiles.AddAsync(stockpile);
        await context.SaveChangesAsync();

        return StashMavenResult<StockpileId>.Success(stockpile.StockpileId);
    }
}
