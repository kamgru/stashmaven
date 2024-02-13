namespace StashMaven.WebApi.Features.Inventory.Shipments;

public partial class ShipmentController
{
    [HttpPost]
    [ProducesResponseType<AddShipmentHandler.AddShipmentResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddShipmentAsync(
        AddShipmentHandler.AddShipmentRequest request,
        [FromServices]
        AddShipmentHandler handler)
    {
        StashMavenResult<AddShipmentHandler.AddShipmentResponse> response =
            await handler.AddShipmentAsync(request);

        if (!response.IsSuccess || response.Data is null)
        {
            return BadRequest(response.Message);
        }

        return Created($"api/v1/shipment/{response.Data.ShipmentId}", response.Data);
    }
}

[Injectable]
public class AddShipmentHandler(
    StashMavenContext context)
{
    public class AddShipmentRequest
    {
        public required string StockpileId { get; set; }
        public required string ShipmentKindId { get; set; }
        public Currency Currency { get; set; }
    }

    public record AddShipmentResponse(string ShipmentId);

    public async Task<StashMavenResult<AddShipmentResponse>> AddShipmentAsync(
        AddShipmentRequest request)
    {
        ShipmentKind? shipmentKind = await context.ShipmentKinds
            .FirstOrDefaultAsync(x => x.ShipmentKindId.Value == request.ShipmentKindId);

        if (shipmentKind == null)
        {
            return StashMavenResult<AddShipmentResponse>.Error("Shipment kind not found.");
        }

        Stockpile? stockpile = await context.Stockpiles
            .FirstOrDefaultAsync(s => s.StockpileId.Value == request.StockpileId);

        if (stockpile == null)
        {
            return StashMavenResult<AddShipmentResponse>.Error("Stockpile not found.");
        }

        Shipment shipment = new()
        {
            ShipmentId = new ShipmentId(Guid.NewGuid().ToString()),
            Kind = shipmentKind,
            Currency = request.Currency,
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow,
            Stockpile = stockpile,
        };

        await context.Shipments.AddAsync(shipment);
        await context.SaveChangesAsync();

        return StashMavenResult<AddShipmentResponse>.Success(new AddShipmentResponse(shipment.ShipmentId.Value));
    }
}
