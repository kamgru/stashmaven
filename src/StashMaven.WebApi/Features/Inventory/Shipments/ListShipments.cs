using StashMaven.WebApi.Data.Services;

namespace StashMaven.WebApi.Features.Inventory.Shipments;

public partial class ShipmentController
{
    [HttpGet]
    [Route("list")]
    [ProducesResponseType<ListShipmentsHandler.ListShipmentsRequest>(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListShipmentsAsync(
        [FromQuery]
        ListShipmentsHandler.ListShipmentsRequest request,
        [FromServices]
        ListShipmentsHandler handler)
    {
        StashMavenResult<ListShipmentsHandler.ListShipmentsResponse> response =
            await handler.ListShipmentsAsync(request);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Message);
        }

        return Ok(response.Data);
    }
}

[Injectable]
public class ListShipmentsHandler(StashMavenContext context, CacheReader cacheReader)
{
    private const int MinPageSize = 5;
    private const int MaxPageSize = 100;
    private const int MinPage = 1;
    private const int MinSearchLength = 3;

    public class ListShipmentsRequest
    {
        public string? StockpileId { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string? Search { get; set; }
        public string? SortBy { get; set; }
        public bool IsAscending { get; set; }
    }

    public class ShipmentListItem
    {
        public required string ShipmentId { get; set; } = null!;
        public required string KindShortCode { get; set; } = null!;
        public string? SequenceNumber { get; set; }
        public string? PartnerIdentifier { get; set; }
        public string? TotalMoney { get; set; }
        public string? AcceptanceStatus { get; set; }
        public required DateTime CreatedOn { get; set; }
    }

    public class ListShipmentsResponse
    {
        public List<ShipmentListItem> Items { get; set; } = [];
        public int TotalCount { get; set; }
        public required string StockpileId { get; set; }
    }

    public async Task<StashMavenResult<ListShipmentsResponse>> ListShipmentsAsync(
        ListShipmentsRequest request)
    {
        request.Page = Math.Max(request.Page, MinPage);
        request.PageSize = Math.Clamp(request.PageSize, MinPageSize, MaxPageSize);

        Stockpile? stockpile;

        if (string.IsNullOrWhiteSpace(request.StockpileId))
        {
            stockpile = await cacheReader.GetDefaultStockpileAsync();
        }
        else
        {
            IReadOnlyList<Stockpile> stockpiles = await cacheReader.GetStockpilesAsync();
            stockpile = stockpiles.FirstOrDefault(x => x.StockpileId.Value == request.StockpileId);
        }

        if (stockpile == null)
        {
            return StashMavenResult<ListShipmentsResponse>.Error(
                $"Stockpile {request.StockpileId} not found");
        }

        var query = context.Shipments
            .Include(s => s.Kind)
            .Include(s => s.Partner)
            .Include(s => s.SourceReference)
            .Include(s => s.SequenceNumber)
            .Where(s => s.Stockpile.StockpileId.Value == stockpile.StockpileId.Value)
            .Select(x => new
            {
                x.ShipmentId,
                x.Kind,
                x.SequenceNumber,
                x.SourceReference,
                x.Partner,
                x.Acceptance,
                x.CreatedOn
            });

        if (!string.IsNullOrWhiteSpace(request.Search) && request.Search.Length >= MinSearchLength)
        {
            string search = $"%{request.Search}%";
            query = query.Where(i => EF.Functions.ILike(i.SequenceNumber.Value, search)
                                     || EF.Functions.ILike(i.SourceReference.Identifier, search));
        }

        query = request.SortBy?.ToLowerInvariant() switch
        {
            "sequence" when request.IsAscending => query.OrderBy(x => x.SequenceNumber),
            "sequence" when !request.IsAscending => query.OrderByDescending(x => x.SequenceNumber),
            _ => query.OrderByDescending(x => x.CreatedOn)
        };

        int totalCount = await query.CountAsync();

        var shipments = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        ListShipmentsResponse response = new()
        {
            TotalCount = totalCount,
            StockpileId = stockpile.StockpileId.Value,
            Items = shipments.Select(
                    s => new ShipmentListItem
                    {
                        ShipmentId = s.ShipmentId.Value,
                        KindShortCode = s.Kind.ShortCode,
                        SequenceNumber = s.SequenceNumber?.Value,
                        PartnerIdentifier = s.Partner?.CustomIdentifier,
                        TotalMoney = "0.00",
                        AcceptanceStatus = s.Acceptance.ToString(),
                        CreatedOn = s.CreatedOn
                    })
                .ToList()
        };
        return StashMavenResult<ListShipmentsResponse>.Success(response);
    }
}
