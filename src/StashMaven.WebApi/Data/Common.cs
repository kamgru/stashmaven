using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StashMaven.WebApi.Data;

public enum UnitOfMeasure
{
    Pc = 0,
    Kg = 1,
    L = 2
}

public enum Currency
{
    Pln = 0,
    Eur = 1,
}

public record TaxDefinitionId(
    string Value);

public class TaxDefinition
{
    public const int NameMaxLength = 100;
    public const int CountryCodeMaxLength = 2;
    
    public int Id { get; set; }
    public required TaxDefinitionId TaxDefinitionId { get; set; }
    public required string Name { get; set; }
    public required decimal Rate { get; set; }
    public required string CountryCode { get; set; }

    public class TypeConfig : IEntityTypeConfiguration<TaxDefinition>
    {
        public void Configure(
            EntityTypeBuilder<TaxDefinition> builder)
        {
            builder.ToTable("TaxDefinition", "com");
            builder.OwnsOne(e => e.TaxDefinitionId)
                .Property(e => e.Value)
                .HasColumnName("TaxDefinitionId");
            builder.Property(e => e.Name)
                .HasMaxLength(NameMaxLength);
            builder.Property(e => e.CountryCode)
                .HasMaxLength(CountryCodeMaxLength);
        }
    }
}

public class SourceReference
{
    public int Id { get; set; }
    public required string Identifier { get; set; }

    public class TypeConfig : IEntityTypeConfiguration<SourceReference>
    {
        public void Configure(
            EntityTypeBuilder<SourceReference> builder)
        {
            builder.ToTable("SourceReference", "com");
        }
    }
}

public class CompanyOption
{
    public int Id { get; set; }
    public required string Key { get; set; }
    public required string Value { get; set; }

    public class TypeConfig : IEntityTypeConfiguration<CompanyOption>
    {
        public void Configure(
            EntityTypeBuilder<CompanyOption> builder)
        {
            builder.ToTable("CompanyOption", "com");
        }
    }
}

public class StashMavenOption
{
    public static class Keys
    {
        public const string DefaultStockpileShortCode = "defaultStockpileShortCode";
        public const string AvailableCountries = "availableCountries";
    }

    public int Id { get; set; }
    public required string Key { get; set; }
    public required string Value { get; set; }

    public class TypeConfig : IEntityTypeConfiguration<StashMavenOption>
    {
        public void Configure(
            EntityTypeBuilder<StashMavenOption> builder)
        {
            builder.ToTable("StashMavenOption", "com");
        }
    }
}

public class SequenceEntry
{
    public int Id { get; set; }
    public SequenceGenerator SequenceGenerator { get; set; } = null!;
    public required string Delimiter { get; set; }
    public required string Group { get; set; }
    public int NextValue { get; set; }

    [Timestamp]
    public uint Version { get; set; }

    public class TypeConfig : IEntityTypeConfiguration<SequenceEntry>
    {
        public void Configure(
            EntityTypeBuilder<SequenceEntry> builder)
        {
            builder.ToTable("SequenceEntry", "inv");
            builder.HasIndex(e => new { e.Group, e.Delimiter })
                .IsUnique();
            builder.Property(e => e.Version)
                .IsRowVersion();
        }
    }
}

public class SequenceGenerator
{
    public int Id { get; set; }
    public required SequenceGeneratorId SequenceGeneratorId { get; set; }
    public ICollection<SequenceEntry> Entries { get; set; } = new List<SequenceEntry>();

    [Timestamp]
    public uint Version { get; set; }

    public class TypeConfig : IEntityTypeConfiguration<SequenceGenerator>
    {
        public void Configure(
            EntityTypeBuilder<SequenceGenerator> builder)
        {
            builder.ToTable("SequenceGenerator", "inv");
            builder.OwnsOne(e => e.SequenceGeneratorId)
                .Property(e => e.Value)
                .HasColumnName("SequenceGeneratorId");
            builder.Property(e => e.Version)
                .IsRowVersion();
        }
    }
}

