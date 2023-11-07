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
            .ToTable("Partner", "smvn");

        modelBuilder.Entity<Address>()
            .ToTable("Address", "smvn");

        modelBuilder.Entity<TaxIdentifier>()
            .ToTable("TaxIdentifier", "smvn");
    }

    public DbSet<Partner> Partners => Set<Partner>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<TaxIdentifier> TaxIdentifiers => Set<TaxIdentifier>();
}
