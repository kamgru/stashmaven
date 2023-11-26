using Microsoft.EntityFrameworkCore;
using StashMaven.WebApi.Data;

namespace StashMaven.WebApi.CatalogFeatures;

public class AddCatalogItemToBrandHandler
{
    public class AddCatalogItemToBrandRequest
    {
        public Guid BrandId { get; set; }
        public Guid CatalogItemId { get; set; }
    }
    
    private readonly StashMavenContext _context;
    
    public AddCatalogItemToBrandHandler(
        StashMavenContext context)
    {
        _context = context;
    }
    
    public async Task<StashMavenResult> AddCatalogItemToBrandAsync(
        AddCatalogItemToBrandRequest request)
    {
        Brand? brand = await _context.Brands
            .Include(b => b.CatalogItems)
            .SingleOrDefaultAsync(b => b.BrandId == request.BrandId);
    
        if (brand == null)
        {
            return StashMavenResult.Error($"Brand {request.BrandId} not found");
        }
    
        CatalogItem? catalogItem = await _context.CatalogItems
            .SingleOrDefaultAsync(c => c.CatalogItemId == request.CatalogItemId);
    
        if (catalogItem == null)
        {
            return StashMavenResult.Error($"Catalog item {request.CatalogItemId} not found");
        }
    
        brand.CatalogItems.Add(catalogItem);
        await _context.SaveChangesAsync();
    
        return StashMavenResult.Success();
    }
}
