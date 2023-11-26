using Microsoft.EntityFrameworkCore;

namespace StashMaven.WebApi.Data;

public class Address
{
    public int Id { get; set; }
    public int PartnerId { get; set; }
    public required string Street { get; set; }
    public string? StreetAdditional { get; set; }
    public required string City { get; set; }
    public string? State { get; set; }
    public required string PostalCode { get; set; }
    public required string CountryCode { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
    public Partner Partner { get; set; } = null!;
}

public enum TaxIdentifierType
{
    Nip = 0,
    Regon = 1,
    Krs = 2
}
public class TaxIdentifier
{
    public int Id { get; set; }
    public int PartnerId { get; set; }
    public required TaxIdentifierType Type { get; set; }
    public required string Value { get; set; }
    public bool IsPrimary { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
    public Partner Partner { get; set; } = null!;
}

public class Partner
{
    public int Id { get; set; }
    public Guid PartnerId { get; set; }
    public required string LegalName { get; set; }
    public required string CustomIdentifier { get; set; }
    public Address? Address { get; set; }
    public List<TaxIdentifier> TaxIdentifiers { get; set; } = new();
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
}

public enum UnitOfMeasure
{
    Pc = 0,
    Kg = 1,
    L = 2
}

public class TaxDefinition
{
    public int Id { get; set; }
    public Guid TaxDefinitionId { get; set; }
    public required string Name { get; set; }
    public required decimal Rate { get; set; }
}

public class CatalogItem
{
    public int Id { get; set; }
    public Guid CatalogItemId { get; set; }
    public required string Name { get; set; }
    public required string Sku { get; set; }
    public required UnitOfMeasure UnitOfMeasure { get; set; }
    public TaxDefinition? TaxDefinition { get; set; }
    public string? BarCode { get; set; }
    public Brand? Brand { get; set; } = null!;
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
}

public class Brand
{
    public int Id { get; set; }
    public Guid BrandId { get; set; }
    public required string Name { get; set; }
    public required string ShortCode { get; set; }
    public List<CatalogItem> CatalogItems { get; set; } = new();
}

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

        modelBuilder.Entity<Partner>()
            .ToTable("Partner", "prt");

        modelBuilder.Entity<Address>()
            .ToTable("Address", "prt");

        modelBuilder.Entity<TaxIdentifier>()
            .ToTable("TaxIdentifier", "prt");

        modelBuilder.Entity<CatalogItem>()
            .ToTable("CatalogItem", "cat");

        modelBuilder.Entity<TaxDefinition>()
            .ToTable("TaxDefinition", "cat");

        modelBuilder.Entity<Brand>()
            .ToTable("Brand", "cat");
    }

    public DbSet<Partner> Partners => Set<Partner>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<TaxIdentifier> TaxIdentifiers => Set<TaxIdentifier>();

    public DbSet<CatalogItem> CatalogItems => Set<CatalogItem>();
    public DbSet<TaxDefinition> TaxDefinitions => Set<TaxDefinition>();
    public DbSet<Brand> Brands => Set<Brand>();
}
