namespace StashMaven.WebApi.Data;

public record BrandId(
    string Value);

public record CatalogItemId(
    string Value);

public record TaxDefinitionId(
    string Value);

public class Brand
{
    public int Id { get; set; }
    public required BrandId BrandId { get; set; }
    public required string Name { get; set; }
    public required string ShortCode { get; set; }
    public List<CatalogItem> CatalogItems { get; set; } = new();
}

public class CatalogItem
{
    public int Id { get; set; }
    public required CatalogItemId CatalogItemId { get; set; }
    public required string Name { get; set; }
    public required string Sku { get; set; }
    public required UnitOfMeasure UnitOfMeasure { get; set; }
    public TaxDefinition? TaxDefinition { get; set; }
    public string? BarCode { get; set; }
    public Brand? Brand { get; set; } = null!;
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
}

public class TaxDefinition
{
    public int Id { get; set; }
    public required TaxDefinitionId TaxDefinitionId { get; set; }
    public required string Name { get; set; }
    public required decimal Rate { get; set; }
}
