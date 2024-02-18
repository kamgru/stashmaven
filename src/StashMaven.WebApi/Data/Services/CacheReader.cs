using Microsoft.Extensions.Caching.Memory;

namespace StashMaven.WebApi.Data.Services;

public class CacheReader(StashMavenContext context, IMemoryCache cache)
{
    public static class Keys
    {
        public const string DefaultStockpile = "DefaultStockpile";
        public const string Stockpiles = "Stockpiles";
        public const string ShipmentKinds = "ShipmentKinds";
    }

    public void InvalidateKey(
        string key) =>
        cache.Remove(key);

    public async Task<Stockpile?> GetDefaultStockpileAsync()
    {
        cache.TryGetValue(Keys.DefaultStockpile, out Stockpile? stockpile);
        
        if (stockpile != null)
        {
            return stockpile;
        }
        
        StashMavenOption? defaultStockpileOption = await context.StashMavenOptions
            .FirstOrDefaultAsync(x => x.Key == StashMavenOption.Keys.DefaultStockpileShortCode);
        
        if (defaultStockpileOption == null)
        {
            return null;
        }
        
        stockpile = await context.Stockpiles
            .FirstOrDefaultAsync(x => x.ShortCode == defaultStockpileOption.Value);

        if (stockpile != null)
        {
            cache.Set(Keys.DefaultStockpile, stockpile, TimeSpan.FromMinutes(60));
        }
        
        return stockpile;
    }

    public async Task<IReadOnlyList<Stockpile>> GetStockpilesAsync()
    {
        cache.TryGetValue(Keys.Stockpiles, out IReadOnlyList<Stockpile>? stockpiles);

        if (stockpiles != null)
        {
            return stockpiles;
        }
        
        stockpiles = await context.Stockpiles.ToListAsync();
        cache.Set(Keys.Stockpiles, stockpiles, TimeSpan.FromMinutes(60));

        return stockpiles;
    }
    
    public async Task<IReadOnlyList<ShipmentKind>> GetKindsAsync()
    {
        cache.TryGetValue(Keys.ShipmentKinds, out IReadOnlyList<ShipmentKind>? kinds);

        if (kinds != null)
        {
            return kinds;
        }
        
        kinds = await context.ShipmentKinds.ToListAsync();
        cache.Set(Keys.ShipmentKinds, kinds, TimeSpan.FromMinutes(60));

        return kinds;
    }
}
