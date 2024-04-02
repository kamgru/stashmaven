namespace StashMaven.WebApi.Features.Inventory.InventoryItems;

public partial class InventoryItemController
{
    [HttpPatch]
    [Route("availability")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<int>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangeInventoryItemStockpileAvailabilityAsync(
        ChangeInventoryItemStockpileAvailabilityHandler.ChangeInventoryItemStockpileAvailabilityRequest request,
        [FromServices]
        ChangeInventoryItemStockpileAvailabilityHandler handler)
    {
        StashMavenResult response = await handler.ChangeInventoryItemStockpileAvailabilityAsync(request);

        if (!response.IsSuccess)
        {
            return BadRequest(response.ErrorCode);
        }

        return Ok();
    }
}

[Injectable]
public class ChangeInventoryItemStockpileAvailabilityHandler(
    StashMavenContext context)
{
    public class ChangeInventoryItemStockpileAvailabilityRequest
    {
        public required string ProductId { get; init; }
        public required IReadOnlyList<StockpileAvailability> StockpileAvailabilities { get; init; } = [];
    }

    public class StockpileAvailability
    {
        public required string StockpileId { get; init; }
        public required bool IsAvailable { get; init; }
    }

    public async Task<StashMavenResult> ChangeInventoryItemStockpileAvailabilityAsync(
        ChangeInventoryItemStockpileAvailabilityRequest request)
    {
        Product? Product = await context.Products
            .AsTracking()
            .FirstOrDefaultAsync(c => c.ProductId.Value == request.ProductId);

        if (Product == null)
        {
            return StashMavenResult.Error(ErrorCodes.ProductNotFound);
        }

        foreach (StockpileAvailability stockpileAvailability in request.StockpileAvailabilities)
        {
            InventoryItem? inventoryItem = await context.InventoryItems
                .AsTracking()
                .FirstOrDefaultAsync(i => i.Product.Id == Product.Id
                                          && i.Stockpile.StockpileId.Value == stockpileAvailability.StockpileId);

            switch (inventoryItem)
            {
                case null when !stockpileAvailability.IsAvailable:
                    continue;

                case null when stockpileAvailability.IsAvailable:
                {
                    Stockpile? stockpile = await context.Stockpiles
                        .AsTracking()
                        .FirstOrDefaultAsync(s => s.StockpileId.Value == stockpileAvailability.StockpileId);

                    if (stockpile == null)
                    {
                        return StashMavenResult.Error(ErrorCodes.StockpileNotFound);
                    }

                    inventoryItem = new InventoryItem
                    {
                        InventoryItemId = new InventoryItemId(Guid.NewGuid().ToString()),
                        Sku = Product.Sku,
                        Name = Product.Name,
                        Version = 0,
                        Stockpile = stockpile,
                        Product = Product,
                    };

                    context.InventoryItems.Add(inventoryItem);

                    continue;
                }

                case { Quantity: > 0 } when !stockpileAvailability.IsAvailable:
                    return StashMavenResult.Error(ErrorCodes.InventoryItemHasQuantity);

                case { Quantity: 0 } when !stockpileAvailability.IsAvailable:
                {
                    bool hasRecords = await context.ShipmentRecords
                        .AnyAsync(sr => sr.InventoryItem.Id == inventoryItem.Id);

                    if (hasRecords)
                    {
                        return StashMavenResult.Error(ErrorCodes.InventoryItemHasRecords);
                    }
                    
                    context.InventoryItems.Remove(inventoryItem);
                    break;
                }
            }
        }

        await context.SaveChangesAsync();
        return StashMavenResult.Success();
    }
}
