using System.Net.Http.Headers;
using Microsoft.Identity.Client;

namespace StashMaven.SeedTool;

public class StashMavenClientFactory
{
    public static async Task<StashMavenClient> Create()
    {
        IPublicClientApplication app = PublicClientApplicationBuilder
            .Create("ede350e5-2bdf-41df-966d-55a96fb4cad4")
            .WithTenantId("50e6b2a2-7d3b-4cfe-9f2a-93ba754dd7f8")
            .WithDefaultRedirectUri()
            .Build();

        TokenCacheHelper.EnableSerialization(app.UserTokenCache);
        IEnumerable<IAccount>? accounts = await app.GetAccountsAsync();
        string[] scopes = ["user.read", "api://569781c4-ad95-4aec-863a-604e6c9f9a13/access_as_user"];
        AuthenticationResult result;
        try
        {
            result = await app.AcquireTokenSilent(scopes, accounts.FirstOrDefault())
                .ExecuteAsync();
        }
        catch (MsalUiRequiredException)
        {
            result = await app.AcquireTokenInteractive(scopes)
                .ExecuteAsync();
        }

        HttpClient client = new();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
        client.BaseAddress = new Uri("http://localhost:5253/api/v1/");

        return new StashMavenClient(client);
    }
}
