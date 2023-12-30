namespace StashMaven.WebApi.Features.Inventory.Shipments;

public partial class ShipmentController
{
    [HttpGet]
    [Route("list")]
    public async Task<IActionResult> ListShipmentsAsync(
        [FromQuery]
        ListShipmentsHandler.ListShipmentsRequest request,
        [FromServices]
        ListShipmentsHandler handler)
    {
        ListShipmentsHandler.ListShipmentsResponse response = await handler.ListShipmentsAsync(request);

        return Ok(response);
    }
}

[Injectable]
public class ListShipmentsHandler(StashMavenContext context)
{
    public class ListShipmentsRequest
    {
        public required string StockpileId { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
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
        public List<ShipmentListItem> Shipments { get; set; } = [];
    }

    public async Task<ListShipmentsResponse> ListShipmentsAsync(
        ListShipmentsRequest request)
    {
        ListShipmentsResponse response = new();

        List<Shipment> shipments = await context.Shipments
            .Include(s => s.Kind)
            .Include(s => s.Partner)
            .OrderByDescending(s => s.CreatedOn)
            .Skip(request.Page * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        response.Shipments = shipments.Select(
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
            .ToList();

        return response;
    }
}
