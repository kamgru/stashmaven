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

        modelBuilder.ApplyConfiguration(new Product.TypeConfig());
        modelBuilder.ApplyConfiguration(new Brand.TypeConfig());

        modelBuilder.ApplyConfiguration(new TaxDefinition.TypeConfig());
        modelBuilder.ApplyConfiguration(new CompanyOption.TypeConfig());
        modelBuilder.ApplyConfiguration(new StashMavenOption.TypeConfig());

        modelBuilder.ApplyConfiguration(new Shipment.TypeConfig());
        modelBuilder.ApplyConfiguration(new ShipmentRecord.TypeConfig());
        modelBuilder.ApplyConfiguration(new ShipmentKind.TypeConfig());
        modelBuilder.ApplyConfiguration(new PartnerRefSnapshot.TypeConfig());
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

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Brand> Brands => Set<Brand>();

    public DbSet<Shipment> Shipments => Set<Shipment>();
    public DbSet<PartnerRefSnapshot> PartnerRefSnapshots => Set<PartnerRefSnapshot>();
    public DbSet<ShipmentRecord> ShipmentRecords => Set<ShipmentRecord>();
    public DbSet<ShipmentKind> ShipmentKinds => Set<ShipmentKind>();
    public DbSet<SourceReference> SourceReferences => Set<SourceReference>();
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
    public DbSet<Stockpile> Stockpiles => Set<Stockpile>();
    public DbSet<SequenceGenerator> SequenceGenerators => Set<SequenceGenerator>();
}

public class UnitOfWork(StashMavenContext context)
{
    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}

public class StashMavenRepository(StashMavenContext context)
{
    public async Task<Stockpile?> GetStockpileAsync(
        StockpileId stockpileId) =>
        await context.Stockpiles
            .AsTracking()
            .SingleOrDefaultAsync(s => s.StockpileId.Value == stockpileId.Value);

    public async Task<Shipment?> GetShipmentAsync(
        ShipmentId shipmentId) =>
        await context.Shipments
            .Include(s => s.Kind)
            .Include(s => s.Records)
            .ThenInclude(r => r.InventoryItem)
            .Include(s => s.Stockpile)
            .Include(s => s.Partner)
            .Include(s => s.PartnerRefSnapshot)
            .Include(s => s.SourceReference)
            .AsTracking()
            .FirstOrDefaultAsync(s => s.ShipmentId.Value == shipmentId.Value);

    public async Task<StashMavenOption?> GetStashMavenOptionAsync(
        string key) =>
        await context.StashMavenOptions
            .AsTracking()
            .FirstOrDefaultAsync(x => x.Key == key);

    public void InsertStashMavenOption(
        StashMavenOption option) =>
        context.StashMavenOptions.Add(option);

    public async Task<CompanyOption?> GetCompanyOptionAsync(
        string key) =>
        await context.CompanyOptions
            .AsTracking()
            .FirstOrDefaultAsync(x => x.Key == key);

    public void InsertCompanyOption(
        CompanyOption option) =>
        context.CompanyOptions.Add(option);
    
    public async Task<Product?> GetProductAsync(
        ProductId productId) =>
        await context.Products
            .AsTracking()
            .Include(x => x.Brand)
            .FirstOrDefaultAsync(ci => ci.ProductId.Value == productId.Value);
    
    public void InsertStockpile(
        Stockpile stockpile) =>
        context.Stockpiles.Add(stockpile);
}
