using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using StashMaven.WebApi.Data;
using StashMaven.WebApi.Features.Catalog.Brands;

namespace StashMaven.Tests.WebApi.Catalog.Brands;

public class CreateBrandTests(
    DefaultTestFixture fixture) : IClassFixture<DefaultTestFixture>
{
    [Fact]
    public async Task WhenValidRequest_BrandCreated()
    {
        HttpClient client = fixture.CreateClient();

        string name = Guid.NewGuid().ToString();
        string shortCode = Guid.NewGuid().ToString();
        HttpResponseMessage response = await client.PostAsJsonAsync("/api/v1/brand",
            new CreateBrandHandler.CreateBrandRequest
            {
                Name = name,
                ShortCode = shortCode
            });

        response.EnsureSuccessStatusCode();

        BrandId? result =
            await response.Content.ReadFromJsonAsync<BrandId>(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        Assert.NotNull(result);

        await using StashMavenContext context = fixture.CreateDbContext();
        Brand? brand = await context.Brands
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.BrandId.Value == result.Value);
        await context.DisposeAsync();

        Assert.NotNull(brand);
        Assert.Equal(name, brand.Name);
        Assert.Equal(shortCode, brand.ShortCode);
    }

    [Fact]
    public async Task WhenNameTooShort_BrandNotCreated()
    {
        HttpClient client = fixture.CreateClient();

        HttpResponseMessage response = await client.PostAsJsonAsync("/api/v1/brand",
            new CreateBrandHandler.CreateBrandRequest
            {
                Name = "Te",
                ShortCode = "TB"
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        await using StashMavenContext context = fixture.CreateDbContext();
        Brand? brand = await context.Brands
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Name == "Te");
        await context.DisposeAsync();

        Assert.Null(brand);
    }

    [Fact]
    public async Task WhenShortCodeTooShort_BrandNotCreated()
    {
        HttpClient client = fixture.CreateClient();

        HttpResponseMessage response = await client.PostAsJsonAsync("/api/v1/brand",
            new CreateBrandHandler.CreateBrandRequest
            {
                Name = "Test Brand",
                ShortCode = "T"
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        await using StashMavenContext context = fixture.CreateDbContext();
        Brand? brand = await context.Brands
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ShortCode == "T");
        await context.DisposeAsync();

        Assert.Null(brand);
    }

    [Fact]
    public async Task WhenShortCodeNotUnique_BrandNotCreated()
    {
        HttpClient client = fixture.CreateClient();

        string shortCode = Guid.NewGuid().ToString();
        HttpResponseMessage response = await client.PostAsJsonAsync("/api/v1/brand",
            new CreateBrandHandler.CreateBrandRequest
            {
                Name = "Test Brand",
                ShortCode = shortCode
            });

        response.EnsureSuccessStatusCode();

        response = await client.PostAsJsonAsync("/api/v1/brand",
            new CreateBrandHandler.CreateBrandRequest
            {
                Name = "Test Brand 2",
                ShortCode = shortCode
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
