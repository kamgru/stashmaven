namespace StashMaven.WebApi.Features.Inventory.Stockpiles;

public partial class StockpileController
{
    [HttpPost]
    [Route("{stockpileId}/inventory-items")]
    public async Task<IActionResult> AddInventoryItemAsync(
        string stockpileId,
        AddInventoryItemHandler.AddInventoryItemRequest request,
        [FromServices]
        AddInventoryItemHandler handler)
    {
        StashMavenResult<AddInventoryItemHandler.AddInventoryItemResponse> response =
            await handler.AddInventoryItemAsync(stockpileId, request);

        if (!response.IsSuccess || response.Data is null)
        {
            return BadRequest(response.Message);
        }

        return Created($"api/v1/inventoryitem/{response.Data.InventoryItemId}", response.Data);
    }
}

[Injectable]
public class AddInventoryItemHandler(
    StashMavenContext context)
{
    public class AddInventoryItemRequest
    {
        public required string CatalogItemId { get; set; }
    }

    public record AddInventoryItemResponse(string InventoryItemId);

    public async Task<StashMavenResult<AddInventoryItemResponse>> AddInventoryItemAsync(
        string stockpileId,
        AddInventoryItemRequest request)
    {
        CatalogItem? catalogItem = await context.CatalogItems
            .FirstOrDefaultAsync(c => c.CatalogItemId.Value == request.CatalogItemId);

        if (catalogItem == null)
        {
            return StashMavenResult<AddInventoryItemResponse>.Error($"Catalog item {request.CatalogItemId} not found");
        }

        Stockpile? stockpile = await context.Stockpiles
            .FirstOrDefaultAsync(s => s.StockpileId.Value == stockpileId);

        if (stockpile == null)
        {
            return StashMavenResult<AddInventoryItemResponse>.Error($"Stockpile {stockpileId} not found");
        }

        InventoryItem? existingItem = await context.InventoryItems
            .FirstOrDefaultAsync(i => i.InventoryItemId.Value == request.CatalogItemId
                                      && i.Stockpile.Id == stockpile.Id);

        if (existingItem != null)
        {
            return StashMavenResult<AddInventoryItemResponse>.Error(
                $"Inventory item {request.CatalogItemId} already exists in stockpile {stockpileId}");
        }

        InventoryItem inventoryItem = new()
        {
            InventoryItemId = new InventoryItemId(catalogItem.CatalogItemId.Value),
            Sku = catalogItem.Sku,
            Name = catalogItem.Name,
            BuyTax = new InventoryItemTaxReference
            {
                Name = catalogItem.BuyTax.Name,
                Rate = catalogItem.BuyTax.Rate,
                TaxDefintionIdValue = catalogItem.BuyTax.TaxDefinitionIdValue
            },
            SellTax = new InventoryItemTaxReference
            {
                Name = catalogItem.SellTax.Name,
                Rate = catalogItem.SellTax.Rate,
                TaxDefintionIdValue = catalogItem.SellTax.TaxDefinitionIdValue
            },
            Version = 0,
            Stockpile = stockpile
        };

        context.InventoryItems.Add(inventoryItem);
        await context.SaveChangesAsync();

        return StashMavenResult<AddInventoryItemResponse>.Success(
            new AddInventoryItemResponse(inventoryItem.InventoryItemId.Value));
    }
}
