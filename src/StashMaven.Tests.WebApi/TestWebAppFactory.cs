using System.Diagnostics;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using StashMaven.WebApi.Data;
using StashMaven.WebApi.Features.Catalog.Brands;

namespace StashMaven.Tests.WebApi;

public class TestWebAppFactory(string dbName) : WebApplicationFactory<Program>
{
    private ILoggerFactory TestLoggerFactory =>
        LoggerFactory.Create(builder =>
        {
            builder.ClearProviders();
            builder.AddConsole();
        });

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
                opt.UseLoggerFactory(TestLoggerFactory);
            });

            services.AddAuthentication("StashMavenTestScheme")
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("StashMavenTestScheme", _ => { });
        });

        builder.UseEnvironment("Development");
    }
}
