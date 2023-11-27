namespace StashMaven.WebApi.Data;

public class Record
{
    public int Id { get; set; }
    public Guid CatalogItemId { get; set; }
    public decimal Quantity { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TaxRate { get; set; }
}

public enum ShipmentDirection
{
    Inbound = 0,
    Outbound = 1,
}

public record ShipmentId(
    string Value);

public class Shipment
{
    public int Id { get; set; }
    public required ShipmentId ShipmentId { get; set; }
    public required PartnerId SupplierId { get; set; }
    public ShipmentDirection ShipmentDirection { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
    public Currency Currency { get; set; }
    public List<Record> Records { get; set; } = new();
}

public class Stock
{
    public int Id { get; set; }
    public required CatalogItemId CatalogItemId { get; set; }
    public decimal Quantity { get; set; }
}
