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
        public string? BuyTaxDefinitionId { get; set; }
        public string? SellTaxDefinitionId { get; set; }
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

        if (request.BuyTaxDefinitionId != null)
        {
            TaxDefinition? taxDefinition = await context.TaxDefinitions
                .SingleOrDefaultAsync(t => t.TaxDefinitionId.Value == request.BuyTaxDefinitionId);

            if (taxDefinition == null)
            {
                return StashMavenResult.Error($"Tax definition {request.BuyTaxDefinitionId} not found");
            }

            catalogItem.BuyTax = new CatalogItemTaxReference
            {
                Name = taxDefinition.Name,
                Rate = taxDefinition.Rate,
                TaxDefinitionIdValue = taxDefinition.TaxDefinitionId.Value
            };
        }

        if (request.SellTaxDefinitionId != null)
        {
            TaxDefinition? taxDefinition = await context.TaxDefinitions
                .SingleOrDefaultAsync(t => t.TaxDefinitionId.Value == request.SellTaxDefinitionId);

            if (taxDefinition == null)
            {
                return StashMavenResult.Error($"Tax definition {request.SellTaxDefinitionId} not found");
            }

            catalogItem.SellTax = new CatalogItemTaxReference
            {
                Name = taxDefinition.Name,
                Rate = taxDefinition.Rate,
                TaxDefinitionIdValue = taxDefinition.TaxDefinitionId.Value
            };
        }

        catalogItem.UpdatedOn = DateTime.UtcNow;
        await context.SaveChangesAsync();

        return StashMavenResult.Success();
    }
}
