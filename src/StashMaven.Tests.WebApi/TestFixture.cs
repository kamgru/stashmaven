using Microsoft.EntityFrameworkCore;
using StashMaven.WebApi.Data;

namespace StashMaven.Tests.WebApi;

public abstract class TestFixture
{
    private readonly TestWebAppFactory _webAppFactory;
    private readonly string _dbName;

    protected TestFixture(string dbName)
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
        : base("stashmaventests")
    {
    }
}

