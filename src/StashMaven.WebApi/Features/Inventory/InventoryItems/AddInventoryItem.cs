namespace StashMaven.WebApi.Features.Inventory.InventoryItems;

public partial class InventoryItemController
{
    [HttpPost]
    [ProducesResponseType<AddInventoryItemHandler.AddInventoryItemResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddInventoryItemAsync(
        AddInventoryItemHandler.AddInventoryItemRequest request,
        [FromServices]
        AddInventoryItemHandler handler)
    {
        StashMavenResult<AddInventoryItemHandler.AddInventoryItemResponse> response =
            await handler.AddInventoryItemAsync(request);

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
        public required string StockpileId { get; set; }
    }

    public class AddInventoryItemResponse
    {
        public required string InventoryItemId { get; set; }
    }

    public async Task<StashMavenResult<AddInventoryItemResponse>> AddInventoryItemAsync(
        AddInventoryItemRequest request)
    {
        CatalogItem? catalogItem = await context.CatalogItems
            .FirstOrDefaultAsync(c => c.CatalogItemId.Value == request.CatalogItemId);

        if (catalogItem == null)
        {
            return StashMavenResult<AddInventoryItemResponse>.Error($"Catalog item {request.CatalogItemId} not found");
        }

        Stockpile? stockpile = await context.Stockpiles
            .AsTracking()
            .FirstOrDefaultAsync(s => s.StockpileId.Value == request.StockpileId);

        if (stockpile == null)
        {
            return StashMavenResult<AddInventoryItemResponse>.Error($"Stockpile {request.StockpileId} not found");
        }

        InventoryItem? existingItem = await context.InventoryItems
            .FirstOrDefaultAsync(i => i.InventoryItemId.Value == request.CatalogItemId
                                      && i.Stockpile.Id == stockpile.Id);

        if (existingItem != null)
        {
            return StashMavenResult<AddInventoryItemResponse>.Error(
                $"Inventory item {request.CatalogItemId} already exists in stockpile {request.StockpileId}");
        }

        InventoryItem inventoryItem = new()
        {
            InventoryItemId = new InventoryItemId(catalogItem.CatalogItemId.Value),
            Sku = catalogItem.Sku,
            Name = catalogItem.Name,
            Version = 0,
            Stockpile = stockpile
        };

        context.InventoryItems.Add(inventoryItem);
        await context.SaveChangesAsync();

        return StashMavenResult<AddInventoryItemResponse>.Success(
            new AddInventoryItemResponse
            {
                InventoryItemId = inventoryItem.InventoryItemId.Value
            });
    }
}
