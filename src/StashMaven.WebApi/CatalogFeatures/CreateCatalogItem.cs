using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using StashMaven.WebApi.Data;

namespace StashMaven.WebApi.CatalogFeatures;

public class CreateCatalogItemHandler
{
    public class CreateCatalogItemRequest
    {
        [MinLength(5)]
        public required string Sku { get; set; }
        [MinLength(3)]
        public required string Name { get; set; } = null!;
        public UnitOfMeasure UnitOfMeasure { get; set; }
        public Guid TaxDefinitionId { get; set; }
    }

    private readonly StashMavenContext _context;

    public CreateCatalogItemHandler(
        StashMavenContext context)
    {
        _context = context;
    }

    public async Task<StashMavenResult<Guid>> CreateCatalogItemAsync(
        CreateCatalogItemRequest request)
    {
        Guid catalogItemId = Guid.NewGuid();
        TaxDefinition? taxDefinition = await _context.TaxDefinitions
            .SingleOrDefaultAsync(t => t.TaxDefinitionId == request.TaxDefinitionId);

        if (taxDefinition == null)
        {
            return StashMavenResult<Guid>.Error($"Tax definition {request.TaxDefinitionId} not found");
        }

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

        await _context.CatalogItems.AddAsync(catalogItem);
        await _context.SaveChangesAsync();

        return StashMavenResult<Guid>.Success(catalogItemId);
    }
}
