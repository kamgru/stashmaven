using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Options;

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
