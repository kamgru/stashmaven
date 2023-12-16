namespace StashMaven.WebApi.Features.Inventory.Stockpiles;

public partial class StockpileController
{
    [HttpGet]
    [Route("list")]
    [ProducesResponseType<ListStockpilesHandler.ListStockpilesResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListStockpilesAsync(
        [FromQuery]
        ListStockpilesHandler.ListStockpilesRequest request,
        [FromServices]
        ListStockpilesHandler handler)
    {
        ListStockpilesHandler.ListStockpilesResponse response =
            await handler.ListStockpilesAsync(request);

        return Ok(response);
    }
}

[Injectable]
public class ListStockpilesHandler(StashMavenContext context)
{
    public class ListStockpilesRequest
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
    }

    public class Stockpile
    {
        public required string StockpileId { get; set; }
        public required string Name { get; set; }
        public required string ShortCode { get; set; }
    }

    public class ListStockpilesResponse
    {
        public List<Stockpile> Items { get; set; } = [];
        public int TotalCount { get; set; }
    }

    public async Task<ListStockpilesResponse> ListStockpilesAsync(
        ListStockpilesRequest request)
    {
        request.Page = Math.Max(1, request.Page);
        request.PageSize = Math.Clamp(request.PageSize, 5, 100);

        IQueryable<Stockpile> query = context.Stockpiles
            .Select(x => new Stockpile
            {
                Name = x.Name,
                ShortCode = x.ShortCode,
                StockpileId = x.StockpileId.Value
            });

        int totalCount = await query.CountAsync();

        List<Stockpile> stockpiles = await query
            .OrderBy(x => x.Name)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return new ListStockpilesResponse
        {
            Items = stockpiles,
            TotalCount = totalCount
        };
    }
}
