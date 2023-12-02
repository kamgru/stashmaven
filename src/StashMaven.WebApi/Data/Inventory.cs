using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

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

    [Timestamp]
    public uint Version { get; set; }
}

public class ShipmentRecord
{
    public int Id { get; set; }
    public required InventoryItemId InventoryItemId { get; set; }
    public decimal Quantity { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TaxRate { get; set; }
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
    public required string Name { get; set; }
    public required string ShortCode { get; set; }
}

public record ShipmentId(
    string Value);

[Owned]
public record SupplierId(
    string Value);

public class Shipment
{
    public int Id { get; set; }
    public required ShipmentId ShipmentId { get; set; }
    public SupplierId? SupplierId { get; set; }
    public ShipmentKindId ShipmentKindId { get; set; } = null!;
    public ShipmentDirection ShipmentDirection { get; set; }
    public ShipmentAcceptance ShipmentAcceptance { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
    public Currency Currency { get; set; }
    public List<ShipmentRecord> ShipmentRecords { get; set; } = new();
}
