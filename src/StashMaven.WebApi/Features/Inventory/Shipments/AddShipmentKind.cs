using Npgsql;

namespace StashMaven.WebApi.Features.Inventory.Shipments;

public partial class ShipmentController
{
    [HttpPost]
    [Route("shipment-kind")]
    public async Task<IActionResult> AddShipmentKindAsync(
        AddShipmentKind.AddShipmentKindRequest request,
        [FromServices]
        AddShipmentKind handler)
    {
        StashMavenResult<AddShipmentKind.AddShipmentKindResponse> response =
            await handler.AddShipmentKindAsync(request);

        if (!response.IsSuccess || response.Data is null)
        {
            return BadRequest(response.Message);
        }

        return Created($"api/v1/shipment-kind/{response.Data.ShipmentKindId}", response.Data);
    }
}

[Injectable]
public class AddShipmentKind(StashMavenContext context)
{
    public class AddShipmentKindRequest
    {
        public required string Name { get; set; }
        public required string ShortCode { get; set; }
        public ShipmentDirection Direction { get; set; }
    }

    public record AddShipmentKindResponse(string ShipmentKindId);

    public async Task<StashMavenResult<AddShipmentKindResponse>> AddShipmentKindAsync(
        AddShipmentKindRequest request)
    {
        SequenceGenerator sequenceGenerator = new()
        {
            SequenceGeneratorId = new SequenceGeneratorId(Guid.NewGuid().ToString()),
            NextValue = 1
        };

        ShipmentKind shipmentKind = new()
        {
            ShipmentKindId = new ShipmentKindId(Guid.NewGuid().ToString()),
            SequenceGeneratorId = sequenceGenerator.SequenceGeneratorId,
            Name = request.Name,
            ShortCode = request.ShortCode,
            ShipmentDirection = request.Direction
        };

        context.ShipmentKinds.Add(shipmentKind);
        context.SequenceGenerators.Add(sequenceGenerator);

        try
        {
            await context.SaveChangesAsync();
            return StashMavenResult<AddShipmentKindResponse>.Success(
                new AddShipmentKindResponse(shipmentKind.ShipmentKindId.Value));
        }
        catch (DbUpdateException e)
        {
            if (e.InnerException is PostgresException { SqlState: StashMavenContext.PostgresUniqueViolation })
            {
                return StashMavenResult<AddShipmentKindResponse>.Error(
                    $"Shipment kind {request.ShortCode} already exists.");
            }

            throw;
        }
    }
}
