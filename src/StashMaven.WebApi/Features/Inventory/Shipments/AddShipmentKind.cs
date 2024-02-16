using Npgsql;

namespace StashMaven.WebApi.Features.Inventory.Shipments;

public partial class ShipmentController
{
    [HttpPost]
    [Route("shipment-kind")]
    [ProducesResponseType<AddShipmentKindHandler.AddShipmentKindResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddShipmentKindAsync(
        AddShipmentKindHandler.AddShipmentKindRequest request,
        [FromServices]
        AddShipmentKindHandler handler)
    {
        StashMavenResult<AddShipmentKindHandler.AddShipmentKindResponse> response =
            await handler.AddShipmentKindAsync(request);

        if (!response.IsSuccess || response.Data is null)
        {
            return BadRequest(response.Message);
        }

        return Created($"api/v1/shipment-kind/{response.Data.ShipmentKindId}", response.Data);
    }
}

[Injectable]
public class AddShipmentKindHandler(StashMavenContext context)
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
        List<Stockpile> stockpiles = await context.Stockpiles.ToListAsync();

        SequenceGenerator sequenceGenerator = new()
        {
            SequenceGeneratorId = new SequenceGeneratorId(Guid.NewGuid().ToString()),
            Version = 0,
        };

        sequenceGenerator.Entries = stockpiles.Select(x => new SequenceEntry
            {
                Group = request.ShortCode,
                Delimiter = x.ShortCode,
                NextValue = 1,
                SequenceGenerator = sequenceGenerator,
                Version = 0
            })
            .ToList();

        ShipmentKind shipmentKind = new()
        {
            ShipmentKindId = new ShipmentKindId(Guid.NewGuid().ToString()),
            SequenceGeneratorId = sequenceGenerator.SequenceGeneratorId,
            Name = request.Name,
            ShortCode = request.ShortCode,
            Direction = request.Direction
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
