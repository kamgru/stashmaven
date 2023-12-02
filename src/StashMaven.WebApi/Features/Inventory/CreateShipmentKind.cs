using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using StashMaven.WebApi.Data;

namespace StashMaven.WebApi.Features.Inventory;

public partial class InventoryController
{
    [HttpPost]
    [Route("shipment-kind")]
    public async Task<IActionResult> CreateShipmentKindAsync(
        CreateShipmentKind.CreateShipmentKindRequest request,
        [FromServices]
        CreateShipmentKind handler)
    {
        StashMavenResult<ShipmentKindId> response =
            await handler.CreateShipmentKindAsync(request);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Message);
        }

        return Ok(response.Data);
    }
}

[Injectable]
public class CreateShipmentKind(StashMavenContext context)
{
    private const string PostgresUniqueViolation = "23505";

    public class CreateShipmentKindRequest
    {
        public required string Name { get; set; }
        public required string ShortCode { get; set; }
    }

    public async Task<StashMavenResult<ShipmentKindId>> CreateShipmentKindAsync(
        CreateShipmentKindRequest request)
    {
        ShipmentKind shipmentKind = new()
        {
            ShipmentKindId = new ShipmentKindId(Guid.NewGuid().ToString()),
            Name = request.Name,
            ShortCode = request.ShortCode,
        };

        await context.ShipmentKinds.AddAsync(shipmentKind);
        try
        {
            await context.SaveChangesAsync();
            return StashMavenResult<ShipmentKindId>.Success(shipmentKind.ShipmentKindId);
        }
        catch (DbUpdateException e)
        {
            if (e.InnerException is PostgresException { SqlState: PostgresUniqueViolation })
            {
                return StashMavenResult<ShipmentKindId>.Error($"Shipment kind {request.ShortCode} already exists.");
            }

            throw;
        }
    }
}
