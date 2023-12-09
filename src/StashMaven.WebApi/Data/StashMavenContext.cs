using Microsoft.EntityFrameworkCore;

namespace StashMaven.WebApi.Data;

public class StashMavenContext : DbContext
{
    public StashMavenContext(
        DbContextOptions<StashMavenContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Partner>(x =>
        {
            x.ToTable("Partner", "prt");
            x.OwnsOne(e => e.PartnerId)
                .Property(e => e.Value)
                .HasColumnName("PartnerId");
        });

        modelBuilder.Entity<Address>()
            .ToTable("Address", "prt");

        modelBuilder.Entity<TaxIdentifier>()
            .ToTable("TaxIdentifier", "prt");

        modelBuilder.Entity<CatalogItem>(x =>
        {
            x.ToTable("CatalogItem", "cat");
            x.OwnsOne(e => e.CatalogItemId)
                .Property(e => e.Value)
                .HasColumnName("CatalogItemId");
        });

        modelBuilder.Entity<TaxDefinition>(x =>
        {
            x.ToTable("TaxDefinition", "cat");
            x.OwnsOne(e => e.TaxDefinitionId)
                .Property(e => e.Value)
                .HasColumnName("TaxDefinitionId");
        });

        modelBuilder.Entity<Brand>(x =>
        {
            x.ToTable("Brand", "cat");
            x.OwnsOne(e => e.BrandId)
                .Property(e => e.Value)
                .HasColumnName("BrandId");
        });

        modelBuilder.Entity<Shipment>(x =>
        {
            x.ToTable("Shipment", "inv");
            x.OwnsOne(e => e.ShipmentId)
                .Property(e => e.Value)
                .HasColumnName("ShipmentId");
            x.OwnsOne(e => e.SupplierId)
                .Property(e => e.Value)
                .HasColumnName("SupplierId");
            x.OwnsOne(e => e.ShipmentSeqId)
                .Property(e => e.Value)
                .HasColumnName("ShipmentSeqId");
        });

        modelBuilder.Entity<ShipmentRecord>(x =>
        {
            x.ToTable("ShipmentRecord", "inv");
        });

        modelBuilder.Entity<InventoryItem>(x =>
        {
            x.ToTable("InventoryItem", "inv");
            x.OwnsOne(e => e.InventoryItemId)
                .Property(e => e.Value)
                .HasColumnName("InventoryItemId");
            x.Property(e => e.TaxDefinitionId)
                .HasConversion(c => c.Value, c => new TaxDefinitionId(c))
                .HasColumnName("TaxDefinitionId");
            x.Property(e => e.Version)
                .IsRowVersion();
        });

        modelBuilder.Entity<ShipmentKind>(x =>
        {
            x.ToTable("ShipmentKind", "inv");
            x.OwnsOne(e => e.ShipmentKindId)
                .Property(e => e.Value)
                .HasColumnName("ShipmentKindId");
            x.HasIndex(e => e.ShortCode)
                .IsUnique();
            x.OwnsOne(e => e.SequenceGeneratorId)
                .Property(e => e.Value)
                .HasColumnName("SequenceGeneratorId");
        });

        modelBuilder.Entity<SourceReference>(x =>
        {
            x.ToTable("SourceReference", "inv");
            x.HasMany(e => e.Shipments)
                .WithOne(e => e.SourceReference);
        });

        modelBuilder.Entity<Stockpile>(x =>
        {
            x.ToTable("Stockpile", "inv");
            x.OwnsOne(e => e.StockpileId)
                .Property(e => e.Value)
                .HasColumnName("StockpileId");
        });

        modelBuilder.Entity<SequenceGenerator>(x =>
        {
            x.ToTable("SequenceGenerator", "inv");
            x.OwnsOne(e => e.SequenceGeneratorId)
                .Property(e => e.Value)
                .HasColumnName("SequenceGeneratorId");
            x.Property(e => e.Version)
                .IsRowVersion();
        });
    }

    public DbSet<Partner> Partners => Set<Partner>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<TaxIdentifier> TaxIdentifiers => Set<TaxIdentifier>();

    public DbSet<CatalogItem> CatalogItems => Set<CatalogItem>();
    public DbSet<TaxDefinition> TaxDefinitions => Set<TaxDefinition>();
    public DbSet<Brand> Brands => Set<Brand>();

    public DbSet<Shipment> Shipments => Set<Shipment>();
    public DbSet<ShipmentRecord> ShipmentRecords => Set<ShipmentRecord>();
    public DbSet<ShipmentKind> ShipmentKinds => Set<ShipmentKind>();
    public DbSet<SourceReference> SourceReferences => Set<SourceReference>();
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
    public DbSet<Stockpile> Stockpiles => Set<Stockpile>();
    public DbSet<SequenceGenerator> SequenceGenerators => Set<SequenceGenerator>();
}
