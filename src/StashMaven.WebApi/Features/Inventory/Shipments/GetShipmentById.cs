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

    public class GetShipmentByIdResponse
    {
        public ShipmentPartner? Partner { get; set; }
        public Currency Currency { get; set; }
        public required ShipmentKindInfo Kind { get; set; }
        public required ShipmentHeader Header { get; set; }
        public required ShipmentStockpile Stockpile { get; set; }
        public List<ShipmentRecord> Records { get; set; } = [];
    }
    
    public class ShipmentRecord
    {
        public required string InventoryItemId { get; init; }
        public required decimal Quantity { get; init; }
        public required decimal UnitPrice { get; init; }
        public required string Sku { get; init; }
        public required string Name { get; init; }
        public required decimal TaxRate { get; init; }
        public required string TaxDefinitionId { get; init; }
        public required string TaxName { get; init; }
    }

    public class ShipmentStockpile
    {
        public required string StockpileId { get; set; }
        public required string Name { get; set; }
        public required string ShortCode { get; set; }
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
        public string? TaxId { get; set; }
    }

    public class ShipmentHeader
    {
        public string? SourceReference { get; init; }
        public string? SequenceNumber { get; init; }
        public DateTime IssuedOn { get; init; }
        public DateTime ShippedOn { get; init; }
    }

    public async Task<StashMavenResult<GetShipmentByIdResponse>> GetShipmentByIdAsync(
        GetShipmentByIdRequest request)
    {
        Shipment? shipment = await context.Shipments
            .Include(s => s.Kind)
            .Include(s => s.Records)
            .ThenInclude(shipmentRecord => shipmentRecord.InventoryItem)
            .Include(s => s.PartnerRefSnapshot)
            .Include(s => s.Partner)
            .Include(s => s.SourceReference)
            .Include(s => s.Stockpile)
            .Include(shipment => shipment.Records)
            .ThenInclude(shipmentRecord => shipmentRecord.Tax)
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
                    TaxId = $"{shipment.PartnerRefSnapshot.PartnerTaxId.TaxIdType}: {shipment.PartnerRefSnapshot.PartnerTaxId.TaxIdValue}"
                },
            Currency = shipment.Currency,
            Header = new ShipmentHeader
            {
                SequenceNumber = shipment.SequenceNumber?.Value,
                SourceReference = shipment.SourceReference?.Identifier,
                IssuedOn = shipment.IssuedOn,
                ShippedOn = shipment.ShippedOn,
            },
            Kind = new ShipmentKindInfo
            {
                Direction = shipment.Kind.Direction.ToString(),
                Name = shipment.Kind.Name,
                ShortCode = shipment.Kind.ShortCode,
            },
            Stockpile = new ShipmentStockpile
            {
                StockpileId = shipment.Stockpile.StockpileId.Value,
                Name = shipment.Stockpile.Name,
                ShortCode = shipment.Stockpile.ShortCode,
            },
            Records = shipment.Records.Select(r => new ShipmentRecord
                {
                    InventoryItemId = r.InventoryItem.InventoryItemId.Value,
                    Quantity = r.Quantity,
                    UnitPrice = r.UnitPrice,
                    Sku = r.InventoryItem.Sku,
                    Name = r.InventoryItem.Name,
                    TaxRate = r.Tax.Rate,
                    TaxDefinitionId = r.Tax.TaxDefinitionId,
                    TaxName = r.Tax.Name,
                })
                .ToList(),
        });
    }
}
