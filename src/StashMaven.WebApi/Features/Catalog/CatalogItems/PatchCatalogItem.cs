namespace StashMaven.WebApi.Features.Catalog.CatalogItems;

public partial class CatalogItemController
{
    [HttpPatch]
    public async Task<IActionResult> PatchCatalogItemAsync(
        PatchCatalogItemHandler.PatchCatalogItemRequest request,
        [FromServices]
        PatchCatalogItemHandler handler)
    {
        StashMavenResult response = await handler.PatchCatalogItemAsync(request);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Message);
        }

        return Ok();
    }
}

[Injectable]
public class PatchCatalogItemHandler(
    StashMavenContext context)
{
    public class PatchCatalogItemRequest
    {
        public required string CatalogItemId { get; set; }

        [MinLength(5)]
        public string? Sku { get; set; }

        [MinLength(3)]
        public string? Name { get; set; }

        public UnitOfMeasure? UnitOfMeasure { get; set; }
        public string? TaxDefinitionId { get; set; }
    }

    public async Task<StashMavenResult> PatchCatalogItemAsync(
        PatchCatalogItemRequest request)
    {
        CatalogItem? catalogItem = await context.CatalogItems
            .SingleOrDefaultAsync(c => c.CatalogItemId.Value == request.CatalogItemId);

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
            TaxDefinition? taxDefinition = await context.TaxDefinitions
                .SingleOrDefaultAsync(t => t.TaxDefinitionId.Value == request.TaxDefinitionId);

            if (taxDefinition == null)
            {
                return StashMavenResult.Error($"Tax definition {request.TaxDefinitionId} not found");
            }

            catalogItem.TaxDefinitionId = taxDefinition.TaxDefinitionId;
        }

        catalogItem.UpdatedOn = DateTime.UtcNow;
        await context.SaveChangesAsync();

        return StashMavenResult.Success();
    }
}
