namespace StashMaven.WebApi.Data;

public class StashMavenContext : DbContext
{
    public const string PostgresUniqueViolation = "23505";

    public StashMavenContext(
        DbContextOptions<StashMavenContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new Partner.TypeConfig());
        modelBuilder.ApplyConfiguration(new Address.TypeConfig());
        modelBuilder.ApplyConfiguration(new TaxIdentifier.TypeConfig());

        modelBuilder.ApplyConfiguration(new CatalogItem.TypeConfig());
        modelBuilder.ApplyConfiguration(new Brand.TypeConfig());

        modelBuilder.ApplyConfiguration(new TaxDefinition.TypeConfig());
        modelBuilder.ApplyConfiguration(new CompanyOption.TypeConfig());
        modelBuilder.ApplyConfiguration(new StashMavenOption.TypeConfig());

        modelBuilder.ApplyConfiguration(new Shipment.TypeConfig());
        modelBuilder.ApplyConfiguration(new ShipmentRecord.TypeConfig());
        modelBuilder.ApplyConfiguration(new ShipmentKind.TypeConfig());
        modelBuilder.ApplyConfiguration(new SourceReference.TypeConfig());
        modelBuilder.ApplyConfiguration(new InventoryItem.TypeConfig());
        modelBuilder.ApplyConfiguration(new Stockpile.TypeConfig());
        modelBuilder.ApplyConfiguration(new SequenceGenerator.TypeConfig());
        modelBuilder.ApplyConfiguration(new SequenceEntry.TypeConfig());
    }

    public DbSet<Partner> Partners => Set<Partner>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<TaxIdentifier> TaxIdentifiers => Set<TaxIdentifier>();

    public DbSet<TaxDefinition> TaxDefinitions => Set<TaxDefinition>();
    public DbSet<CompanyOption> CompanyOptions => Set<CompanyOption>();
    public DbSet<StashMavenOption> StashMavenOptions => Set<StashMavenOption>();

    public DbSet<CatalogItem> CatalogItems => Set<CatalogItem>();
    public DbSet<Brand> Brands => Set<Brand>();

    public DbSet<Shipment> Shipments => Set<Shipment>();
    public DbSet<ShipmentRecord> ShipmentRecords => Set<ShipmentRecord>();
    public DbSet<ShipmentKind> ShipmentKinds => Set<ShipmentKind>();
    public DbSet<SourceReference> SourceReferences => Set<SourceReference>();
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
    public DbSet<Stockpile> Stockpiles => Set<Stockpile>();
    public DbSet<SequenceGenerator> SequenceGenerators => Set<SequenceGenerator>();
}
