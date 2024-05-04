using StashMaven.WebApi.Data.Services;

namespace StashMaven.WebApi.Features.Inventory.Stockpiles;

public partial class StockpileController
{
    [HttpGet]
    [Route("{stockpileId}")]
    [ProducesResponseType<GetStockpileByIdHandler.GetStockpileByIdResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<int>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetStockpileByIdAsync(
        string stockpileId,
        [FromServices]
        GetStockpileByIdHandler handler)
    {
        StashMavenResult<GetStockpileByIdHandler.GetStockpileByIdResponse> result =
            await handler.GetStockpileByIdAsync(new GetStockpileByIdHandler.GetStockpileByIdRequest(stockpileId));

        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorCode);
        }

        return Ok(result.Data);
    }
}

[Injectable]
public class GetStockpileByIdHandler(CacheReader cacheReader)
{
    public record GetStockpileByIdRequest(string StockpileId);
    
    public record GetStockpileByIdResponse(
        string Name,
        string ShortCode,
        bool IsDefault);
    
    public async Task<StashMavenResult<GetStockpileByIdResponse>> GetStockpileByIdAsync(
        GetStockpileByIdRequest request)
    {
        IReadOnlyList<Stockpile> stockpiles = await cacheReader.GetStockpilesAsync();
        
        Stockpile? stockpile = stockpiles.FirstOrDefault(x => x.StockpileId.Value == request.StockpileId);
        
        if (stockpile is null)
        {
            return StashMavenResult<GetStockpileByIdResponse>.Error(ErrorCodes.StockpileNotFound);
        }
        
        Stockpile? defaultStockpile = await cacheReader.GetDefaultStockpileAsync();
        bool isDefault = stockpile.StockpileId == defaultStockpile?.StockpileId;
        
        return StashMavenResult<GetStockpileByIdResponse>.Success(new GetStockpileByIdResponse(
            stockpile.Name,
            stockpile.ShortCode,
            isDefault));
    }
}
