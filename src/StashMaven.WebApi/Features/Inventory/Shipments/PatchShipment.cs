namespace StashMaven.WebApi.Features.Inventory.Shipments;

public partial class ShipmentController
{
    [HttpPatch]
    [Route("{shipmentId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PatchShipmentAsync(
        string shipmentId,
        PatchShipmentHandler.PatchShipmentRequest request,
        [FromServices]
        PatchShipmentHandler handler)
    {
        StashMavenResult response = await handler.PatchShipmentAsync(shipmentId, request);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Message);
        }

        return Ok();
    }
}

[Injectable]
public class PatchShipmentHandler(StashMavenRepository repository, UnitOfWork unitOfWork)
{
    public class PatchShipmentRequest
    {
        public string? SourceReferenceIdentifier { get; set; }
        public DateTime? IssuedOn { get; set; }
    }

    public async Task<StashMavenResult> PatchShipmentAsync(
        string shipmentId,
        PatchShipmentRequest request)
    {
        Shipment? shipment = await repository.GetShipmentAsync(new ShipmentId(shipmentId));

        if (shipment is null)
        {
            return StashMavenResult.Error(ErrorCodes.ShipmentNotFound);
        }

        if (request.SourceReferenceIdentifier is not null)
        {
            if (shipment.SourceReference is null)
            {
                shipment.SourceReference = new SourceReference
                {
                    Identifier = request.SourceReferenceIdentifier
                };
            }
            else
            {
                shipment.SourceReference.Identifier = request.SourceReferenceIdentifier;
            }
        }

        if (request.IssuedOn is not null)
        {
            shipment.IssuedOn = request.IssuedOn.Value;
        }

        await unitOfWork.SaveChangesAsync();

        return StashMavenResult.Success();
    }
}
