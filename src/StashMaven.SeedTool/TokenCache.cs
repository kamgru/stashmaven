using Microsoft.Identity.Client;

namespace StashMaven.SeedTool;

internal static class TokenCache
{
    public static void EnableSerialization(
        ITokenCache tokenCache)
    {
        tokenCache.SetBeforeAccess(BeforeAccessNotification);
        tokenCache.SetAfterAccess(AfterAccessNotification);
    }

    private static readonly string CacheFilePath =
        System.Reflection.Assembly.GetExecutingAssembly().Location + ".msalcache.bin3";

    private static void BeforeAccessNotification(
        TokenCacheNotificationArgs args)
    {
        if (!File.Exists(CacheFilePath))
        {
            return;
        }

        args.TokenCache.DeserializeMsalV3(File.ReadAllBytes(CacheFilePath));
    }

    private static void AfterAccessNotification(
        TokenCacheNotificationArgs args)
    {
        if (args.HasStateChanged)
        {
            File.WriteAllBytes(CacheFilePath,
                args.TokenCache.SerializeMsalV3());
        }
    }
}
