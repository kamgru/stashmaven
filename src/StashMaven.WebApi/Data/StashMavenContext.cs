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

            x.OwnsOne(e => e.ShipmentKindId)
                .Property(e => e.Value)
                .HasColumnName("ShipmentKindId");
        });

        modelBuilder.Entity<ShipmentRecord>(x =>
        {
            x.ToTable("ShipmentRecord", "inv");
            x.OwnsOne(e => e.InventoryItemId)
                .Property(e => e.Value)
                .HasColumnName("InventoryItemId");
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
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
}
