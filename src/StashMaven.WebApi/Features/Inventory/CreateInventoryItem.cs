namespace StashMaven.WebApi.Features.Inventory;

public partial class InventoryController
{
    [HttpPost]
    [Route("inventory-item")]
    public async Task<IActionResult> CreateInventoryItemAsync(
        CreateInventoryItemHandler.CreateInventoryItemRequest request,
        [FromServices]
        CreateInventoryItemHandler handler)
    {
        StashMavenResult<InventoryItemId> response = await handler.CreateInventoryItemAsync(request);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Message);
        }

        return Ok(response.Data);
    }
}

[Injectable]
public class CreateInventoryItemHandler(
    StashMavenContext context)
{
    public class CreateInventoryItemRequest
    {
        public required string CatalogItemId { get; set; }
    }

    public async Task<StashMavenResult<InventoryItemId>> CreateInventoryItemAsync(
        CreateInventoryItemRequest request)
    {
        CatalogItem? catalogItem = await context.CatalogItems
            .Include(c => c.CatalogItemId)
            .FirstOrDefaultAsync(c => c.CatalogItemId.Value == request.CatalogItemId);

        if (catalogItem == null)
        {
            return StashMavenResult<InventoryItemId>.Error($"Catalog item {request.CatalogItemId} not found");
        }

        InventoryItem inventoryItem = new()
        {
            InventoryItemId = new InventoryItemId(catalogItem.CatalogItemId.Value),
            Sku = catalogItem.Sku,
            Name = catalogItem.Name,
            TaxDefinitionId = catalogItem.TaxDefinitionId,
            Version = 0
        };

        context.InventoryItems.Add(inventoryItem);
        await context.SaveChangesAsync();

        return StashMavenResult<InventoryItemId>.Success(inventoryItem.InventoryItemId);
    }
}
