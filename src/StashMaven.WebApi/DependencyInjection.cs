using System.Reflection;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;

namespace StashMaven.WebApi;

public static class ServiceCollectionExtensions
{
    public static void AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<StashMavenContext>(opt =>
        {
            string writeModelConnectionString = configuration.GetConnectionString("WriteModel")
                                                ?? throw new InvalidOperationException(
                                                    "WriteModel connection string is missing.");
            opt.UseNpgsql(writeModelConnectionString)
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });
    }

    public static void AddFeatures(
        this IServiceCollection services)
    {
        Assembly assembly = Assembly.GetAssembly(typeof(Program))
                            ?? throw new InvalidOperationException(
                                "Could not find StashMaven.WebApi assembly.");

        List<Type> injectables = assembly
            .GetTypes()
            .Where(x => x.IsDefined(typeof(InjectableAttribute)))
            .ToList();

        foreach (Type injectable in injectables)
        {
            services.AddScoped(injectable);
        }
    }

    public static void AddSwagger(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        AadConfig aadConfig = configuration.GetAadConfiguration();

        services.AddSwaggerGen(opt =>
        {
            opt.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Description = "OAuth2.0 Auth Code with PKCE",
                Name = "oauth2",
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = aadConfig.AuthorizationUrl,
                        TokenUrl = aadConfig.TokenUrl,
                        Scopes = aadConfig.GetApiScopesForSwagger()
                    }
                }
            });

            opt.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
                    },
                    aadConfig.GetApiScopesForSwagger().Keys.ToList()
                }
            });
        });
    }

    public static void AddWebApiAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        AadConfig aadConfig = configuration.GetAadConfiguration();

        services.AddAuthentication()
            .AddMicrosoftIdentityWebApi(aadConfig.RawConfiguration);
    }
}

public static class ConfigurationExtensions
{
    public static AadConfig GetAadConfiguration(
        this IConfiguration configuration)
    {
        IConfigurationSection aadSection = configuration.GetSection("AzureAd");
        return new AadConfig(aadSection);
    }
}

public class AadConfig(IConfigurationSection aadSection)
{
    private readonly string _apiScope = aadSection["ApiScope"]
                                        ?? throw new InvalidOperationException(
                                            "ApiScope is missing from the configuration.");

    public string Instance { get; } = aadSection["Instance"]
                                      ?? throw new InvalidOperationException(
                                          "Instance is missing from the configuration.");

    public string TenantId { get; } = aadSection["TenantId"]
                                      ?? throw new InvalidOperationException(
                                          "TenantId is missing from the configuration.");

    public string ClientId { get; } = aadSection["ClientId"]
                                      ?? throw new InvalidOperationException(
                                          "ClientId is missing from the configuration.");

    public IConfigurationSection RawConfiguration => aadSection;
    public Uri AuthorizationUrl => new($"{Instance}{TenantId}/oauth2/v2.0/authorize");
    public Uri TokenUrl => new($"{Instance}{TenantId}/oauth2/v2.0/token");

    public IDictionary<string, string> GetApiScopesForSwagger()
    {
        return new Dictionary<string, string>
        {
            { $"api://{ClientId}/{_apiScope}", $"{_apiScope}" }
        };
    }
}
