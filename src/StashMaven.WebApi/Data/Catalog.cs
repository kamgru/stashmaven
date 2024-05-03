using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StashMaven.WebApi.Data;

public record BrandId(
    string Value);

public record ProductId(
    string Value);

public class Brand
{
    public const int NameMaxLength = 256;
    public const int ShortCodeMaxLength = 10;
    
    public int Id { get; set; }
    public required BrandId BrandId { get; set; }
    public required string Name { get; set; }
    public required string ShortCode { get; set; }
    public List<Product> Products { get; set; } = [];
    
    public class TypeConfig : IEntityTypeConfiguration<Brand>
    {
        public void Configure(
            EntityTypeBuilder<Brand> builder)
        {
            builder.ToTable("Brand", "cat");
            builder.OwnsOne(e => e.BrandId)
                .Property(e => e.Value)
                .HasColumnName("BrandId");
            builder.HasIndex(e => e.ShortCode)
                .IsUnique();
            builder.Property(e => e.Name)
                .HasMaxLength(NameMaxLength);
            builder.Property(e => e.ShortCode)
                .HasMaxLength(ShortCodeMaxLength);
        }
    }
}

public class ProductTaxReference
{
    public required string Name { get; set; }
    public decimal Rate { get; set; }
    public required string TaxDefinitionIdValue { get; set; }
}

public class Product
{
    public const int SkuMaxLength = 10;
    public const int NameMaxLength = 256;
    
    public int Id { get; set; }
    public required ProductId ProductId { get; set; }
    public required string Name { get; set; }
    public required string Sku { get; set; }
    public required UnitOfMeasure UnitOfMeasure { get; set; }
    public string? BarCode { get; set; }
    public Brand? Brand { get; set; } = null!;
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
    public ICollection<InventoryItem> InventoryItems { get; set; } = [];
    
    public class TypeConfig : IEntityTypeConfiguration<Product>
    {
        public void Configure(
            EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Product", "cat");
            builder.OwnsOne(e => e.ProductId)
                .Property(e => e.Value)
                .HasColumnName("ProductId");
            builder.Property(e => e.Sku)
                .HasMaxLength(SkuMaxLength);
            builder.HasIndex(e => e.Sku)
                .IsUnique();
            builder.Property(e => e.Name)
                .HasMaxLength(NameMaxLength);
        }
    }
}
