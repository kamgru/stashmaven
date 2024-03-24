using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Options;
using StashMaven.WebApi.Features.Catalog.CatalogItems;
using StashMaven.WebApi.Features.Common.Countries;
using StashMaven.WebApi.Features.Common.TaxDefinitions;
using StashMaven.WebApi.Features.Inventory.InventoryItems;
using StashMaven.WebApi.Features.Inventory.Shipments;
using StashMaven.WebApi.Features.Inventory.Stockpiles;
using StashMaven.WebApi.Features.Partners;

namespace StashMaven.Tests.WebApi;

public abstract class TestFixture
{
    private readonly TestWebAppFactory _webAppFactory;
    private readonly string _dbName;

    protected TestFixture(
        string dbName)
    {
        _dbName = dbName;
        _webAppFactory = new TestWebAppFactory(dbName);

        using StashMavenContext context = CreateDbContext();
        context.Database.EnsureCreated();
    }

    public HttpClient CreateClient() => _webAppFactory.CreateClient();

    public StashMavenContext CreateDbContext() =>
        new(
            new DbContextOptionsBuilder<StashMavenContext>()
                .UseNpgsql($"Server=localhost;Database={_dbName};User Id=postgres;Password=mysecretpassword")
                .Options);

    public async Task<string> AddStockpile()
    {
        using HttpClient http = CreateClient();
        string name = Guid.NewGuid().ToString();
        AddStockpileHandler.AddStockpileRequest request = new(name, name[..Stockpile.ShortCodeMaxLength], false);
        HttpResponseMessage response = await http.PostAsJsonAsync("api/v1/stockpile", request);
        response.EnsureSuccessStatusCode();

        AddStockpileHandler.AddStockpileResponse? addStockpileResponse =
            await response.Content.ReadFromJsonAsync<AddStockpileHandler.AddStockpileResponse>();

        return addStockpileResponse?.StockpileId ?? string.Empty;
    }

    public async Task<string> AddShipmentKind(
        ShipmentDirection direction = ShipmentDirection.In)
    {
        using HttpClient http = CreateClient();
        string name = Guid.NewGuid().ToString();
        AddShipmentKindHandler.AddShipmentKindRequest request = new()
        {
            Name = name,
            ShortCode = name[..ShipmentKind.ShortCodeMaxLength],
            Direction = direction
        };
        HttpResponseMessage response = await http.PostAsJsonAsync("api/v1/shipment/shipment-kind", request);
        response.EnsureSuccessStatusCode();

        AddShipmentKindHandler.AddShipmentKindResponse? addShipmentKindResponse =
            await response.Content.ReadFromJsonAsync<AddShipmentKindHandler.AddShipmentKindResponse>();

        return addShipmentKindResponse?.ShipmentKindId ?? string.Empty;
    }

    public async Task<string> AddTaxDefinition()
    {
        using HttpClient http = CreateClient();
        string name = Guid.NewGuid().ToString();
        AddTaxDefinitionHandler.AddTaxDefinitionRequest request = new()
        {
            Name = name,
            Rate = 0.1m,
            CountryCode = "PL"
        };
        HttpResponseMessage response = await http.PostAsJsonAsync("api/v1/taxdefinition", request);
        response.EnsureSuccessStatusCode();

        AddTaxDefinitionHandler.AddTaxDefinitionResponse? addTaxDefinitionResponse =
            await response.Content.ReadFromJsonAsync<AddTaxDefinitionHandler.AddTaxDefinitionResponse>();

        return addTaxDefinitionResponse?.TaxDefinitionId ?? string.Empty;
    }

    public async Task<string> AddCatalogItem(
        string taxDefinitionId)
    {
        using HttpClient http = CreateClient();
        string name = Guid.NewGuid().ToString();
        AddCatalogItemHandler.AddCatalogItemRequest request = new()
        {
            Name = name,
            Sku = name[..CatalogItem.SkuMaxLength],
            UnitOfMeasure = UnitOfMeasure.Kg
        };
        HttpResponseMessage response = await http.PostAsJsonAsync("api/v1/catalogitem", request);
        response.EnsureSuccessStatusCode();

        AddCatalogItemHandler.AddCatalogItemResponse? addCatalogItemResponse =
            await response.Content.ReadFromJsonAsync<AddCatalogItemHandler.AddCatalogItemResponse>();

        return addCatalogItemResponse?.CatalogItemId ?? string.Empty;
    }

    public async Task<string> AddInventoryItem(
        string catalogItemId,
        string stockpileId)
    {
        using HttpClient http = CreateClient();
        AddInventoryItemHandler.AddInventoryItemRequest request = new()
        {
            CatalogItemId = catalogItemId,
            StockpileId = stockpileId
        };
        HttpResponseMessage response = await http.PostAsJsonAsync("api/v1/inventoryitem", request);
        response.EnsureSuccessStatusCode();

        AddInventoryItemHandler.AddInventoryItemResponse? addInventoryItemResponse =
            await response.Content.ReadFromJsonAsync<AddInventoryItemHandler.AddInventoryItemResponse>();

        return addInventoryItemResponse?.InventoryItemId ?? string.Empty;
    }

    public async Task<string> AddPartner()
    {
        using HttpClient http = CreateClient();
        AddPartnerHandler.AddPartnerRequest request = new()
        {
            Address = new AddPartnerHandler.AddPartnerRequest.PartnerAddress
            {
                City = "Test City",
                Street = "Test Street",
                State = "Test State",
                CountryCode = "PL",
                PostalCode = "00-000"
            },
            CustomIdentifier = Guid.NewGuid().ToString(),
            LegalName = Guid.NewGuid().ToString(),
            TaxIdentifiers =
            [
                new AddPartnerHandler.AddPartnerRequest.TaxIdentifier
                {
                    IsPrimary = true,
                    Type = TaxIdentifierType.Nip,
                    Value = Guid.NewGuid().ToString()[..10]
                }
            ]
        };
        HttpResponseMessage response = await http.PostAsJsonAsync("api/v1/partner", request);
        response.EnsureSuccessStatusCode();

        AddPartnerHandler.AddPartnerResponse? addPartnerResponse =
            await response.Content.ReadFromJsonAsync<AddPartnerHandler.AddPartnerResponse>();

        return addPartnerResponse?.PartnerId ?? string.Empty;
    }

    public async Task<string> AddShipment(
        string stockpileId,
        string shipmentKindId)
    {
        using HttpClient http = CreateClient();
        AddShipmentHandler.AddShipmentRequest request = new()
        {
            StockpileId = stockpileId,
            ShipmentKindId = shipmentKindId,
        };
        HttpResponseMessage response = await http.PostAsJsonAsync("api/v1/shipment", request);
        response.EnsureSuccessStatusCode();

        AddShipmentHandler.AddShipmentResponse? addShipmentResponse =
            await response.Content.ReadFromJsonAsync<AddShipmentHandler.AddShipmentResponse>();

        return addShipmentResponse?.ShipmentId ?? string.Empty;
    }

    public async Task ChangeShipmentPartner(
        string shipmentId,
        string partnerId)
    {
        using HttpClient http = CreateClient();
        ChangeShipmentPartnerHandler.ChangeShipmentPartnerRequest request = new()
        {
            PartnerId = partnerId
        };
        HttpResponseMessage response = await http.PatchAsJsonAsync($"api/v1/shipment/{shipmentId}/partner", request);
        response.EnsureSuccessStatusCode();
    }

    public async Task AddRecordToShipment(
        string shipmentId,
        string inventoryItemId,
        decimal quantity,
        decimal price)
    {
        using HttpClient http = CreateClient();
        AddRecordToShipmentHandler.AddRecordToShipmentRequest request = new()
        {
            InventoryItemId = inventoryItemId,
            Quantity = quantity,
            UnitPrice = price
        };
        HttpResponseMessage response = await http.PatchAsJsonAsync($"api/v1/shipment/{shipmentId}/record", request);
        response.EnsureSuccessStatusCode();
    }

    public async Task AcceptShipment(
        string shipmentId)
    {
        using HttpClient http = CreateClient();
        HttpResponseMessage response = await http.PostAsJsonAsync($"api/v1/shipment/{shipmentId}/accept", new { });
        response.EnsureSuccessStatusCode();
    }
    
    public async Task AddCountry()
    {
        using HttpClient http = CreateClient();
        AddAvailableCountryHandler.AddAvailableCountryRequest request = new()
        {
            Code = "PL",
            Name = "Poland"
        };
        HttpResponseMessage response = await http.PostAsJsonAsync("api/v1/country/available", request);
        response.EnsureSuccessStatusCode();
    }
    
    public async Task AddSourceReference(
        string shipmentId,
        string sourceReference)
    {
        using HttpClient http = CreateClient();
        PatchShipmentHandler.PatchShipmentRequest request = new()
        {
            SourceReferenceIdentifier = sourceReference
        };
        HttpResponseMessage response = await http.PatchAsJsonAsync($"api/v1/shipment/{shipmentId}", request);
        response.EnsureSuccessStatusCode();
    }
}

public class DefaultTestFixture : TestFixture
{
    public DefaultTestFixture()
        : base("stash-maven-tests")
    {
    }
}

public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        Claim[] claims = { new(ClaimTypes.Name, "StashMaven Test User") };
        ClaimsIdentity identity = new(claims, "StashMavenTestIdentity");
        ClaimsPrincipal principal = new(identity);
        AuthenticationTicket ticket = new(principal, "StashMavenTestScheme");

        AuthenticateResult result = AuthenticateResult.Success(ticket);

        return Task.FromResult(result);
    }
}

public class TestWebAppFactory(string dbName) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the app's DbContext registration and replace it with test specific one
            ServiceDescriptor? dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<StashMavenContext>));

            if (dbContextDescriptor != null)
            {
                services.Remove(dbContextDescriptor);
            }

            services.AddDbContext<StashMavenContext>(opt =>
            {
                opt.UseNpgsql($"Server=localhost;Database={dbName};User Id=postgres;Password=mysecretpassword");
            });

            services.AddAuthentication("StashMavenTestScheme")
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("StashMavenTestScheme", _ => { });
        });

        builder.UseEnvironment("Development");
    }
}
