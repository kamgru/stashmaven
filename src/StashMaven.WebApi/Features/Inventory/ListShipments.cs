using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StashMaven.WebApi.Data;

namespace StashMaven.WebApi.Features.Inventory;

public partial class InventoryController
{
    [HttpGet]
    [Route("shipment")]
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
        public int Page { get; set; }
        public int PageSize { get; set; }
    }

    public class ShipmentListItem
    {
        public string ShipmentId { get; set; } = null!;
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
            .OrderByDescending(s => s.CreatedOn)
            .Skip(request.Page * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        response.Shipments = shipments.Select(
                s => new ShipmentListItem
                {
                    ShipmentId = s.ShipmentId.Value,
                })
            .ToList();

        return response;
    }
}
