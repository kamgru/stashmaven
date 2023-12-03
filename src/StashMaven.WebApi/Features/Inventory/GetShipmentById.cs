using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StashMaven.WebApi.Data;

namespace StashMaven.WebApi.Features.Inventory;

public partial class InventoryController
{
    [HttpGet]
    [Route("shipment/{shipmentId}")]
    public async Task<IActionResult> GetShipmentByIdAsync(
        string shipmentId,
        [FromServices]
        GetShipmentByIdHandler handler)
    {
        StashMavenResult<GetShipmentByIdHandler.ShipmentsResponse> response = await handler.GetShipmentByIdAsync(
            new GetShipmentByIdHandler.GetShipmentByIdRequest
            {
                ShipmentId = shipmentId,
            });

        if (!response.IsSuccess)
        {
            return BadRequest(response.Message);
        }

        return Ok(response.Data);
    }
}

[Injectable]
public class GetShipmentByIdHandler(
    StashMavenContext context)
{
    public class GetShipmentByIdRequest
    {
        public required string ShipmentId { get; init; }
    }

    public class ShipmentsResponse
    {
        public string? SupplierId { get; set; }
        public Currency Currency { get; set; }
        public ShipmentDirection ShipmentDirection { get; set; }
    }

    public async Task<StashMavenResult<ShipmentsResponse>> GetShipmentByIdAsync(
        GetShipmentByIdRequest request)
    {
        Shipment? shipment = await context.Shipments
            .Include(s => s.SupplierId)
            .Include(s => s.Kind)
            .SingleOrDefaultAsync(s => s.ShipmentId.Value == request.ShipmentId);

        if (shipment == null)
        {
            return StashMavenResult<ShipmentsResponse>.Error($"Shipment {request.ShipmentId} not found");
        }

        return StashMavenResult<ShipmentsResponse>.Success(new ShipmentsResponse
        {
            SupplierId = shipment.SupplierId?.Value,
            Currency = shipment.Currency,
            ShipmentDirection = shipment.Kind.ShipmentDirection,
        });
    }
}
