namespace StashMaven.WebApi.Features.Inventory.Shipments;

public partial class ShipmentController
{
    [HttpGet]
    [Route("{shipmentId}")]
    [ProducesResponseType<GetShipmentByIdHandler.GetShipmentByIdResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetShipmentByIdAsync(
        string shipmentId,
        [FromServices]
        GetShipmentByIdHandler handler)
    {
        StashMavenResult<GetShipmentByIdHandler.GetShipmentByIdResponse> response = await handler.GetShipmentByIdAsync(
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

    public class ShipmentRecord
    {
        public required string InventoryItemId { get; init; }
        public required decimal Quantity { get; init; }
        public required decimal UnitPrice { get; init; }
        public required string Sku { get; init; }
        public required string Name { get; init; }
        public decimal TaxRate { get; init; }
    }

    public class GetShipmentByIdResponse
    {
        public ShipmentPartner? Partner { get; set; }
        public Currency Currency { get; set; }
        public required ShipmentKindInfo Kind { get; set; }
        public List<ShipmentRecord> Records { get; set; } = [];
    }

    public class ShipmentKindInfo
    {
        public required string Name { get; set; }
        public required string Direction { get; set; }
        public required string ShortCode { get; set; }
    }

    public class ShipmentPartner
    {
        public required string PartnerId { get; set; }
        public required string CustomIdentifier { get; set; }
        public required string LegalName { get; set; }
        public required string Address { get; set; }
    }

    public async Task<StashMavenResult<GetShipmentByIdResponse>> GetShipmentByIdAsync(
        GetShipmentByIdRequest request)
    {
        Shipment? shipment = await context.Shipments
            .Include(s => s.Kind)
            .Include(s => s.Records)
            .ThenInclude(shipmentRecord => shipmentRecord.InventoryItem)
            .Include(shipment => shipment.PartnerRefSnapshot)
            .Include(shipment => shipment.Partner)
            .SingleOrDefaultAsync(s => s.ShipmentId.Value == request.ShipmentId);

        if (shipment == null)
        {
            return StashMavenResult<GetShipmentByIdResponse>.Error($"Shipment {request.ShipmentId} not found");
        }

        return StashMavenResult<GetShipmentByIdResponse>.Success(new GetShipmentByIdResponse
        {
            Partner = shipment.PartnerRefSnapshot is null || shipment.Partner is null
                ? null
                : new ShipmentPartner
                {
                    PartnerId = shipment.Partner.PartnerId.Value,
                    CustomIdentifier = shipment.Partner.CustomIdentifier,
                    LegalName = shipment.PartnerRefSnapshot.LegalName,
                    Address = shipment.PartnerRefSnapshot.Address,
                },
            Currency = shipment.Currency,
            Kind = new ShipmentKindInfo
            {
                Direction = shipment.Kind.Direction.ToString(),
                Name = shipment.Kind.Name,
                ShortCode = shipment.Kind.ShortCode,
            },
            Records = shipment.Records.Select(r => new ShipmentRecord
                {
                    InventoryItemId = r.InventoryItem.InventoryItemId.Value,
                    Quantity = r.Quantity,
                    UnitPrice = r.UnitPrice,
                    Sku = r.InventoryItem.Sku,
                    Name = r.InventoryItem.Name,
                    TaxRate = r.TaxRate,
                })
                .ToList(),
        });
    }
}
