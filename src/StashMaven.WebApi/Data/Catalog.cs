using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StashMaven.WebApi.Data;

public record BrandId(
    string Value);

public record CatalogItemId(
    string Value);

public class Brand
{
    public int Id { get; set; }
    public required BrandId BrandId { get; set; }
    public required string Name { get; set; }
    public required string ShortCode { get; set; }
    public List<CatalogItem> CatalogItems { get; set; } = [];

    public class TypeConfig : IEntityTypeConfiguration<Brand>
    {
        public void Configure(EntityTypeBuilder<Brand> builder)
        {
            builder.ToTable("Brand", "cat");
            builder.OwnsOne(e => e.BrandId)
                .Property(e => e.Value)
                .HasColumnName("BrandId");
        }
    }
}

public class CatalogItemTaxReference
{
    public required string Name { get; set; }
    public decimal Rate { get; set; }
    public required string TaxDefinitionIdValue { get; set; }
}

public class CatalogItem
{
    private const int MaxSkuLength = 50;
    private const int MaxNameLength = 256;

    public int Id { get; set; }
    public required CatalogItemId CatalogItemId { get; set; }
    public required string Name { get; set; }
    public required string Sku { get; set; }
    public required UnitOfMeasure UnitOfMeasure { get; set; }
    public required CatalogItemTaxReference BuyTax { get; set; }
    public required CatalogItemTaxReference SellTax { get; set; }
    public string? BarCode { get; set; }
    public Brand? Brand { get; set; } = null!;
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }

    public class TypeConfig : IEntityTypeConfiguration<CatalogItem>
    {
        public void Configure(EntityTypeBuilder<CatalogItem> builder)
        {
            builder.ToTable("CatalogItem", "cat");
            builder.OwnsOne(e => e.CatalogItemId)
                .Property(e => e.Value)
                .HasColumnName("CatalogItemId");
            builder.Property(e => e.Sku)
                .HasMaxLength(MaxSkuLength);
            builder.HasIndex(e => e.Sku)
                .IsUnique();
            builder.Property(e => e.Name)
                .HasMaxLength(MaxNameLength);
            builder.ComplexProperty(e => e.BuyTax);
            builder.ComplexProperty(e => e.SellTax);
        }
    }
}

