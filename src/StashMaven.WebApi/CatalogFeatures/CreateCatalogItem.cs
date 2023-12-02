using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using StashMaven.WebApi.Data;

namespace StashMaven.WebApi.CatalogFeatures;

[Injectable]
public class CreateCatalogItemHandler(
    StashMavenContext context)
{
    public class CreateCatalogItemRequest
    {
        [MinLength(5)]
        public required string Sku { get; set; }

        [MinLength(3)]
        public required string Name { get; set; } = null!;

        public UnitOfMeasure UnitOfMeasure { get; set; }
        public required string TaxDefinitionId { get; set; }
    }

    public async Task<StashMavenResult<CatalogItemId>> CreateCatalogItemAsync(
        CreateCatalogItemRequest request)
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
