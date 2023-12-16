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
        public void Configure(EntityTypeBuilder<TaxDefinition> builder)
        {
            builder.ToTable("TaxDefinition", "com");
            builder.OwnsOne(e => e.TaxDefinitionId)
                .Property(e => e.Value)
                .HasColumnName("TaxDefinitionId");
        }
    }
}
