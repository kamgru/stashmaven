namespace StashMaven.WebApi.Features.Inventory.Shipments;

public partial class ShipmentController
{
    [HttpDelete]
    [Route("{shipmentId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteShipmentAsync(
        string shipmentId,
        [FromServices]
        DeleteShipmentHandler handler)
    {
        StashMavenResult response = await handler.DeleteShipmentAsync(shipmentId);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Message);
        }

        return Ok();
    }
}

[Injectable]
public class DeleteShipmentHandler(StashMavenContext context)
{
    public async Task<StashMavenResult> DeleteShipmentAsync(string shipmentId)
    {
        Shipment? shipment = await context.Shipments
            .Include(x => x.PartnerRefSnapshot)
            .AsTracking()
            .SingleOrDefaultAsync(s => s.ShipmentId.Value == shipmentId);

        if (shipment is null)
        {
            return StashMavenResult.Error($"Shipment {shipmentId} not found");
        }
        
        if (shipment.Acceptance != ShipmentAcceptance.Pending)
        {
            return StashMavenResult.Error($"Shipment {shipmentId} has already been accepted");
        }
        
        if (shipment.PartnerRefSnapshot is not null)
        {
            context.PartnerRefSnapshots.Remove(shipment.PartnerRefSnapshot);
        }
        
        context.Shipments.Remove(shipment);
        await context.SaveChangesAsync();

        return StashMavenResult.Success();
    }
}
