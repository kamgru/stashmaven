using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StashMaven.WebApi.Data;

namespace StashMaven.WebApi.Features.Catalog.Brands;

public partial class BrandController
{
    [HttpPost]
    [Route("add-catalog-item")]
    public async Task<IActionResult> AddCatalogItemToBrandAsync(
        AddCatalogItemToBrandHandler.AddCatalogItemToBrandRequest request,
        [FromServices]
        AddCatalogItemToBrandHandler handler)
    {
        StashMavenResult response = await handler.AddCatalogItemToBrandAsync(request);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Message);
        }

        return Ok();
    }
}

[Injectable]
public class AddCatalogItemToBrandHandler(
    StashMavenContext context)
{
    public class AddCatalogItemToBrandRequest
    {
        public required string BrandId { get; set; }
        public required string CatalogItemId { get; set; }
    }

    public async Task<StashMavenResult> AddCatalogItemToBrandAsync(
        AddCatalogItemToBrandRequest request)
    {
        Brand? brand = await context.Brands
            .Include(b => b.CatalogItems)
            .SingleOrDefaultAsync(b => b.BrandId.Value == request.BrandId);

        if (brand == null)
        {
            return StashMavenResult.Error($"Brand {request.BrandId} not found");
        }

        CatalogItem? catalogItem = await context.CatalogItems
            .SingleOrDefaultAsync(c => c.CatalogItemId.Value == request.CatalogItemId);

        if (catalogItem == null)
        {
            return StashMavenResult.Error($"Catalog item {request.CatalogItemId} not found");
        }

        brand.CatalogItems.Add(catalogItem);
        await context.SaveChangesAsync();

        return StashMavenResult.Success();
    }
}
