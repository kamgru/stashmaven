using Microsoft.EntityFrameworkCore.Metadata;
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
    public int Id { get; set; }
    public required TaxDefinitionId TaxDefinitionId { get; set; }
    public required string Name { get; set; }
    public required decimal Rate { get; set; }

    public class TypeConfig : IEntityTypeConfiguration<TaxDefinition>
    {
        public void Configure(
            EntityTypeBuilder<TaxDefinition> builder)
        {
            builder.ToTable("TaxDefinition", "com");
            builder.OwnsOne(e => e.TaxDefinitionId)
                .Property(e => e.Value)
                .HasColumnName("TaxDefinitionId");
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
