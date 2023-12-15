namespace StashMaven.WebApi.Features.Catalog.CatalogItems;

public partial class CatalogItemController
{
    [HttpPost]
    public async Task<IActionResult> AddCatalogItemAsync(
        AddCatalogItemHandler.ADdCatalogItemRequest request,
        [FromServices]
        AddCatalogItemHandler handler)
    {
        StashMavenResult<CatalogItemId> response = await handler.AddCatalogItemAsync(request);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Message);
        }

        return Created($"api/v1/catalog/catalog-item/{response.Data?.Value}", response.Data);
    }
}

[Injectable]
public class AddCatalogItemHandler(
    StashMavenContext context)
{
    public class ADdCatalogItemRequest
    {
        [MinLength(5)]
        public required string Sku { get; set; }

        [MinLength(3)]
        public required string Name { get; set; } = null!;

        public UnitOfMeasure UnitOfMeasure { get; set; }
        public required string TaxDefinitionId { get; set; }
    }

    public async Task<StashMavenResult<CatalogItemId>> AddCatalogItemAsync(
        ADdCatalogItemRequest request)
    {
        TaxDefinition? taxDefinition = await context.TaxDefinitions
            .SingleOrDefaultAsync(t => t.TaxDefinitionId.Value == request.TaxDefinitionId);

        if (taxDefinition == null)
        {
            return StashMavenResult<CatalogItemId>.Error($"Tax definition {request.TaxDefinitionId} not found");
        }

        CatalogItemId catalogItemId = new(Guid.NewGuid().ToString());

        CatalogItem catalogItem = new()
        {
            CatalogItemId = catalogItemId,
            Sku = request.Sku,
            Name = request.Name,
            UnitOfMeasure = request.UnitOfMeasure,
            TaxDefinition = taxDefinition,
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow
        };

        await context.CatalogItems.AddAsync(catalogItem);
        await context.SaveChangesAsync();

        return StashMavenResult<CatalogItemId>.Success(catalogItemId);
    }
}
