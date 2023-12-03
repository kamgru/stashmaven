using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StashMaven.WebApi.Data;

namespace StashMaven.WebApi.Features.Inventory;

public partial class InventoryController
{
    [HttpPost]
    [Route("shipment")]
    public async Task<IActionResult> CreateShipmentAsync(
        CreateShipmentHandler.CreateShipmentRequest request,
        [FromServices]
        CreateShipmentHandler handler)
    {
        StashMavenResult<CreateShipmentHandler.CreateShipmentResponse> response =
            await handler.CreateShipmentAsync(request);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Message);
        }

        return Ok(response.Data);
    }
}

[Injectable]
public class CreateShipmentHandler(
    StashMavenContext context)
{
    public class CreateShipmentRequest
    {
        public required string ShipmentKindId { get; set; }
        public Currency Currency { get; set; }
    }

    public class CreateShipmentResponse
    {
        public ShipmentId? ShipmentId { get; set; }
    }

    public async Task<StashMavenResult<CreateShipmentResponse>> CreateShipmentAsync(
        CreateShipmentRequest request)
    {
        ShipmentKind? shipmentKind = await context.ShipmentKinds
            .FirstOrDefaultAsync(x => x.ShipmentKindId.Value == request.ShipmentKindId);

        if (shipmentKind == null)
        {
            return StashMavenResult<CreateShipmentResponse>.Error("Shipment kind not found.");
        }

        Shipment shipment = new()
        {
            ShipmentId = new ShipmentId(Guid.NewGuid().ToString()),
            Kind = shipmentKind,
            Currency = request.Currency,
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow,
        };

        await context.Shipments.AddAsync(shipment);
        await context.SaveChangesAsync();

        return StashMavenResult<CreateShipmentResponse>.Success(new CreateShipmentResponse
        {
            ShipmentId = shipment.ShipmentId,
        });
    }
}
