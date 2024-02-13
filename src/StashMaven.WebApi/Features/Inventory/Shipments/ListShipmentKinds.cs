using StashMaven.WebApi.Data.Services;

namespace StashMaven.WebApi.Features.Inventory.Shipments;

public partial class ShipmentController
{
    [HttpGet]
    [Route("ShipmentKind/list")]
    [ProducesResponseType<ListShipmentKinds.ListShipmentKindsResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListShipmentKindsAsync(
        [FromServices]
        ListShipmentKinds handler)
    {
        StashMavenResult<ListShipmentKinds.ListShipmentKindsResponse> response =
            await handler.ListShipmentKindsAsync();

        return Ok(response.Data);
    }
}

[Injectable]
public class ListShipmentKinds(CacheReader cacheReader)
{
    public class ShipmentKindItem
    {
        public required string ShipmentKindId { get; set; }
        public required string Name { get; set; }
        public required string ShortCode { get; set; }
        public required string Direction { get; set; }
    }

    public class ListShipmentKindsResponse
    {
        public IReadOnlyList<ShipmentKindItem> Items { get; set; } = [];
    }

    public async Task<StashMavenResult<ListShipmentKindsResponse>> ListShipmentKindsAsync()
    {
        IReadOnlyList<ShipmentKind> shipmentKinds = await cacheReader.GetKindsAsync();
        List<ShipmentKindItem> items = shipmentKinds.Select(x => new ShipmentKindItem
            {
                ShipmentKindId = x.ShipmentKindId.Value,
                Name = x.Name,
                ShortCode = x.ShortCode,
                Direction = x.Direction.ToString()
            })
            .ToList();

        return StashMavenResult<ListShipmentKindsResponse>.Success(new ListShipmentKindsResponse
        {
            Items = items
        });
    }
}
