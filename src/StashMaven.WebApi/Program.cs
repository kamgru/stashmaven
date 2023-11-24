using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using StashMaven.WebApi.Data;
using StashMaven.WebApi.PartnerFeatures;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

IConfigurationSection aadSection = builder.Configuration.GetSection("AzureAd");
AadConfig aadConfig = new(aadSection);

builder.Services.AddSwaggerGen(opt =>
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

builder.Services.AddAuthentication()
    .AddMicrosoftIdentityWebApi(aadSection);

builder.Services.AddDbContext<StashMavenContext>(opt =>
{
    string writeModelConnectionString = builder.Configuration.GetConnectionString("WriteModel")
                                        ?? throw new InvalidOperationException(
                                            "WriteModel connection string is missing.");
    opt.UseNpgsql(writeModelConnectionString);
});
builder.Services.AddScoped<CreatePartnerHandler>();
builder.Services.AddScoped<GetPartnerByIdHandler>();
builder.Services.AddScoped<ListPartnersHandler>();
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("allow-all", policyBuilder =>
        policyBuilder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(opt =>
    {
        opt.SwaggerEndpoint("/swagger/v1/swagger.json", "StashMaven.WebApi v1");
        opt.OAuthClientId(aadConfig.ClientId);
        opt.OAuthUsePkce();
        opt.OAuthScopeSeparator(" ");
    });
    app.UseCors("allow-all");
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public class AadConfig
{
    private readonly IConfigurationSection _aadSection;
    private readonly string _apiScope;

    public AadConfig(
        IConfigurationSection aadSection)
    {
        _aadSection = aadSection;
        Instance = aadSection["Instance"]
                   ?? throw new InvalidOperationException(
                       "Instance is missing from the configuration.");
        TenantId = aadSection["TenantId"]
                   ?? throw new InvalidOperationException(
                       "TenantId is missing from the configuration.");
        ClientId = aadSection["ClientId"]
                   ?? throw new InvalidOperationException(
                       "ClientId is missing from the configuration.");
        _apiScope = aadSection["ApiScope"]
                    ?? throw new InvalidOperationException(
                        "ApiScope is missing from the configuration.");
    }

    public string Instance { get; }
    public string TenantId { get; }
    public string ClientId { get; }
    public IConfigurationSection AadSection => _aadSection;
    public Uri AuthorizationUrl => new($"{Instance}{TenantId}/oauth2/v2.0/authorize");
    public Uri TokenUrl => new($"{Instance}{TenantId}/oauth2/v2.0/token");

    public IDictionary<string, string> GetApiScopesForSwagger()
    {
        return new Dictionary<string, string>
        {
            {$"api://{ClientId}/{_apiScope}", $"{_apiScope}"}
        };
    }
}
