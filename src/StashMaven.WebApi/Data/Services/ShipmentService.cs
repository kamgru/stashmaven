namespace StashMaven.WebApi.Data.Services;

[Injectable]
public class ShipmentService(
    StashMavenContext context)
{
    public async Task<bool> CheckIfStockpileHasShipmentsAsync(
        StockpileId stockpileId)
    {
        Stockpile? stockpile = await context.Stockpiles
            .FirstOrDefaultAsync(x => x.StockpileId.Value == stockpileId.Value);

        if (stockpile is null)
        {
            return false;
        }

        return await context.Shipments
            .AnyAsync(x => x.Stockpile == stockpile);
    }
}
