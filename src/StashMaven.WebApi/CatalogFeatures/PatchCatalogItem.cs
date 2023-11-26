using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using StashMaven.WebApi.Data;

namespace StashMaven.WebApi.CatalogFeatures;

public class PatchCatalogItemHandler
{
    public class PatchCatalogItemRequest
    {
        public Guid CatalogItemId { get; set; }
        [MinLength(5)]
        public string? Sku { get; set; }
        [MinLength(3)]
        public string? Name { get; set; }
        public UnitOfMeasure? UnitOfMeasure { get; set; }
        public Guid? TaxDefinitionId { get; set; }
    }
    
    private readonly StashMavenContext _context;
    
    public PatchCatalogItemHandler(
        StashMavenContext context)
    {
        _context = context;
    }
    
    public async Task<StashMavenResult> PatchCatalogItemAsync(
        PatchCatalogItemRequest request)
    {
        CatalogItem? catalogItem = await _context.CatalogItems
            .SingleOrDefaultAsync(c => c.CatalogItemId == request.CatalogItemId);
    
        if (catalogItem == null)
        {
            return StashMavenResult.Error($"Catalog item {request.CatalogItemId} not found");
        }
    
        if (request.Sku != null)
        {
            catalogItem.Sku = request.Sku;
        }
    
        if (request.Name != null)
        {
            catalogItem.Name = request.Name;
        }
    
        if (request.UnitOfMeasure != null)
        {
            catalogItem.UnitOfMeasure = request.UnitOfMeasure.Value;
        }
    
        if (request.TaxDefinitionId != null)
        {
            TaxDefinition? taxDefinition = await _context.TaxDefinitions
                .SingleOrDefaultAsync(t => t.TaxDefinitionId == request.TaxDefinitionId);
    
            if (taxDefinition == null)
            {
                return StashMavenResult.Error($"Tax definition {request.TaxDefinitionId} not found");
            }
    
            catalogItem.TaxDefinition = taxDefinition;
        }
    
        catalogItem.UpdatedOn = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    
        return StashMavenResult.Success();
    }
}
