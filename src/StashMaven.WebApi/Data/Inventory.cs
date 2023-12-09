using System.ComponentModel.DataAnnotations;

namespace StashMaven.WebApi.Data;

public record InventoryItemId(
    string Value);

public class InventoryItem
{
    public int Id { get; set; }
    public required InventoryItemId InventoryItemId { get; set; }
    public required string Sku { get; set; }
    public required string Name { get; set; }
    public decimal Quantity { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; }
    public decimal UnitPrice { get; set; }
    public required TaxDefinitionId TaxDefinitionId { get; set; }
    public ICollection<ShipmentRecord> ShipmentRecords { get; set; } = new List<ShipmentRecord>();
    public Stockpile Stockpile { get; set; } = null!;

    [Timestamp]
    public uint Version { get; set; }
}

public class ShipmentRecord
{
    public int Id { get; set; }
    public decimal Quantity { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TaxRate { get; set; }
    public InventoryItem InventoryItem { get; set; } = null!;
    public Shipment Shipment { get; set; } = null!;
}

public enum ShipmentDirection
{
    Out = -1,
    In = 1,
}

public enum ShipmentAcceptance
{
    Pending = 0,
    Accepted = 1,
    Rejected = 2,
}

public record ShipmentKindId(
    string Value);

public class ShipmentKind
{
    public int Id { get; set; }
    public required ShipmentKindId ShipmentKindId { get; set; }
    public required SequenceGeneratorId SequenceGeneratorId { get; set; }
    public required string Name { get; set; }
    public required string ShortCode { get; set; }
    public ShipmentDirection ShipmentDirection { get; set; }
    public List<Shipment> Shipments { get; set; } = [];
}

public record ShipmentId(
    string Value);

public record SupplierId(
    string Value);

public record ShipmentSeqId(
    string Value);

public class Shipment
{
    public int Id { get; set; }
    public required ShipmentId ShipmentId { get; set; }
    public SupplierId? SupplierId { get; set; }
    public ShipmentSeqId? ShipmentSeqId { get; set; }
    public ShipmentAcceptance ShipmentAcceptance { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
    public Currency Currency { get; set; }
    public List<ShipmentRecord> Records { get; set; } = new();
    public ShipmentKind Kind { get; set; } = null!;
    public SourceReference? SourceReference { get; set; }
    public Stockpile Stockpile { get; set; } = null!;
}

public record SequenceGeneratorId(
    string Value);

public class SequenceGenerator
{
    public int Id { get; set; }
    public required SequenceGeneratorId SequenceGeneratorId { get; set; }
    public int NextValue { get; set; }

    [Timestamp]
    public uint Version { get; set; }
}

public class SourceReference
{
    public int Id { get; set; }
    public required string Identifier { get; set; }
    public ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();
}

public record StockpileId(
    string Value);

public class Stockpile
{
    public int Id { get; set; }
    public required StockpileId StockpileId { get; set; }
    public required string Name { get; set; }
    public required string ShortCode { get; set; }
    public ICollection<InventoryItem> InventoryItems { get; set; } = new List<InventoryItem>();
    public ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();
}
