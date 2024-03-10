namespace StashMaven.WebApi.Features.Inventory.InventoryItems;

public partial class InventoryItemController
{
    [HttpGet]
    [Route("{inventoryItemId}")]
    [ProducesResponseType<GetInventoryItemByIdHandler.GetInventoryItemByIdResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetInventoryItemByIdAsync(
        string inventoryItemId,
        [FromServices]
        GetInventoryItemByIdHandler handler)
    {
        StashMavenResult<GetInventoryItemByIdHandler.GetInventoryItemByIdResponse> response =
            await handler.GetInventoryItemByIdAsync(inventoryItemId);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Message);
        }

        return Ok(response.Data);
    }
}

[Injectable]
public class GetInventoryItemByIdHandler(StashMavenContext context)
{
    public class GetInventoryItemByIdResponse
    {
        public required string InventoryItemId { get; set; }
        public required string Sku { get; set; }
        public required string Name { get; set; }
        public decimal Quantity { get; set; }
        public required string UnitOfMeasure { get; set; }
        public decimal LastPurchasePrice { get; set; }
        public decimal SellPrice { get; set; }
    }

    public async Task<StashMavenResult<GetInventoryItemByIdResponse>> GetInventoryItemByIdAsync(
        string inventoryItemId)
    {
        InventoryItem? inventoryItem = await context.InventoryItems
            .SingleOrDefaultAsync(c => c.InventoryItemId.Value == inventoryItemId);

        if (inventoryItem == null)
        {
            return StashMavenResult<GetInventoryItemByIdResponse>.Error($"Inventory item {inventoryItemId} not found");
        }

        GetInventoryItemByIdResponse response = new()
        {
            InventoryItemId = inventoryItem.InventoryItemId.Value,
            Sku = inventoryItem.Sku,
            Name = inventoryItem.Name,
            Quantity = inventoryItem.Quantity,
            UnitOfMeasure = inventoryItem.UnitOfMeasure.ToString(),
            LastPurchasePrice = inventoryItem.LastPurchasePrice,
            SellPrice = inventoryItem.LastPurchasePrice * 1.2m,
        };

        return StashMavenResult<GetInventoryItemByIdResponse>.Success(response);
    }
}
