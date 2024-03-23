using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StashMaven.WebApi.Data;

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

public record StockpileId(
    string Value);

public record ShipmentKindId(
    string Value);

public record InventoryItemId(
    string Value);

public record SequenceGeneratorId(
    string Value);

public record ShipmentId(
    string Value);

public record ShipmentSequenceNumber(
    string Value);

public class InventoryItemTaxReference
{
    public required string Name { get; set; }
    public decimal Rate { get; set; }
    public required string TaxDefintionIdValue { get; set; }
}

public class InventoryItem
{
    public int Id { get; set; }
    public required InventoryItemId InventoryItemId { get; set; }
    public required string Sku { get; set; }
    public required string Name { get; set; }
    public decimal Quantity { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; }
    public decimal LastPurchasePrice { get; set; }
    public ICollection<ShipmentRecord> ShipmentRecords { get; set; } = new List<ShipmentRecord>();
    public Stockpile Stockpile { get; set; } = null!;
    public CatalogItem CatalogItem { get; set; } = null!;

    [Timestamp]
    public uint Version { get; set; }

    public class TypeConfig : IEntityTypeConfiguration<InventoryItem>
    {
        public void Configure(
            EntityTypeBuilder<InventoryItem> builder)
        {
            builder.ToTable("InventoryItem", "inv");
            builder.OwnsOne(e => e.InventoryItemId)
                .Property(e => e.Value)
                .HasColumnName("InventoryItemId");
            builder.Property(e => e.Version)
                .IsRowVersion();
        }
    }
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

    public class TypeConfig : IEntityTypeConfiguration<ShipmentRecord>
    {
        public void Configure(
            EntityTypeBuilder<ShipmentRecord> builder)
        {
            builder.ToTable("ShipmentRecord", "inv");
        }
    }
}

public class ShipmentKind
{
    public const int ShortCodeMaxLength = 8;

    public int Id { get; set; }
    public required ShipmentKindId ShipmentKindId { get; set; }
    public required SequenceGeneratorId SequenceGeneratorId { get; set; }
    public required string Name { get; set; }
    public required string ShortCode { get; set; }
    public ShipmentDirection Direction { get; set; }
    public ICollection<Shipment> Shipments { get; set; } = [];

    public class TypeConfig : IEntityTypeConfiguration<ShipmentKind>
    {
        public void Configure(
            EntityTypeBuilder<ShipmentKind> builder)
        {
            builder.ToTable("ShipmentKind", "inv");
            builder.OwnsOne(e => e.ShipmentKindId)
                .Property(e => e.Value)
                .HasColumnName("ShipmentKindId");
            builder.HasIndex(e => e.ShortCode)
                .IsUnique();
            builder.OwnsOne(e => e.SequenceGeneratorId)
                .Property(e => e.Value)
                .HasColumnName("SequenceGeneratorId");
            builder.Property(e => e.ShortCode)
                .HasMaxLength(ShortCodeMaxLength);
        }
    }
}

public class PartnerRefSnapshot
{
    public int Id { get; set; }
    public required string LegalName { get; set; }
    public required string Address { get; set; }

    public class TypeConfig : IEntityTypeConfiguration<PartnerRefSnapshot>
    {
        public void Configure(
            EntityTypeBuilder<PartnerRefSnapshot> builder)
        {
            builder.ToTable("ShipmentPartnerReference", "inv");
        }
    }
}

public class Shipment
{
    public int Id { get; set; }
    public required ShipmentId ShipmentId { get; set; }
    public ShipmentSequenceNumber? SequenceNumber { get; set; }
    public ShipmentAcceptance Acceptance { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
    public Currency Currency { get; set; }
    public List<ShipmentRecord> Records { get; set; } = new();
    public ShipmentKind Kind { get; set; } = null!;
    public SourceReference? SourceReference { get; set; }
    public Stockpile Stockpile { get; set; } = null!;
    public PartnerRefSnapshot? PartnerRefSnapshot { get; set; }
    public Partner? Partner { get; set; }
    public DateTime IssuedOn { get; set; }

    public class TypeConfig : IEntityTypeConfiguration<Shipment>
    {
        public void Configure(
            EntityTypeBuilder<Shipment> builder)
        {
            builder.ToTable("Shipment", "inv");
            builder.OwnsOne(e => e.ShipmentId)
                .Property(e => e.Value)
                .HasColumnName("ShipmentId");
            builder.OwnsOne(e => e.SequenceNumber)
                .Property(e => e.Value)
                .HasColumnName("SequenceNumber");
        }
    }
}

public class Stockpile
{
    public const int ShortCodeMaxLength = 8;

    public int Id { get; set; }
    public required StockpileId StockpileId { get; set; }
    public required string Name { get; set; }
    public required string ShortCode { get; set; }
    public ICollection<InventoryItem> InventoryItems { get; set; } = new List<InventoryItem>();
    public ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();

    public class TypeConfig : IEntityTypeConfiguration<Stockpile>
    {
        public void Configure(
            EntityTypeBuilder<Stockpile> builder)
        {
            builder.ToTable("Stockpile", "inv");
            builder.Property(e => e.ShortCode)
                .HasMaxLength(ShortCodeMaxLength);
            builder.HasIndex(e => e.ShortCode)
                .IsUnique();
            builder.OwnsOne(e => e.StockpileId)
                .Property(e => e.Value)
                .HasColumnName("StockpileId");
        }
    }
}
