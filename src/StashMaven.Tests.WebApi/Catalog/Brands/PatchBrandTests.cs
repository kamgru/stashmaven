using System.Net;
using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;
using StashMaven.WebApi.Data;
using StashMaven.WebApi.Features.Catalog.Brands;

namespace StashMaven.Tests.WebApi.Catalog.Brands;

public class PatchBrandTests(
    DefaultTestFixture fixture) : IClassFixture<DefaultTestFixture>
{
    [Fact]
    public async Task WhenValidRequest_BrandUpdated()
    {
        BrandId brandId = new(Guid.NewGuid().ToString());
        Brand brand = new()
        {
            BrandId = brandId,
            Name = "Test Brand",
            ShortCode = Guid.NewGuid().ToString()
        };

        await using StashMavenContext context = fixture.CreateDbContext();
        await context.Brands.AddAsync(brand);
        await context.SaveChangesAsync();
        context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        HttpClient client = fixture.CreateClient();

        string newName = "Test Brand 2";
        string newShortCode = Guid.NewGuid().ToString();

        HttpResponseMessage response = await client.PatchAsJsonAsync($"/api/v1/brand/{brandId.Value}",
            new PatchBrandHandler.PatchBrandRequest
            {
                Name = newName,
                ShortCode = newShortCode
            });

        response.EnsureSuccessStatusCode();

        Brand? updatedBrand = await context.Brands.FirstOrDefaultAsync(x => x.BrandId.Value == brandId.Value);

        Assert.NotNull(updatedBrand);
        Assert.Equal(newName, updatedBrand.Name);
        Assert.Equal(newShortCode, updatedBrand.ShortCode);
    }

    [Fact]
    public async Task WhenNameTooShort_BrandNotUpdated()
    {
        BrandId brandId = new(Guid.NewGuid().ToString());
        Brand brand = new()
        {
            BrandId = brandId,
            Name = "Test Brand",
            ShortCode = "TB"
        };

        await using StashMavenContext context = fixture.CreateDbContext();
        await context.Brands.AddAsync(brand);
        await context.SaveChangesAsync();

        HttpClient client = fixture.CreateClient();

        HttpResponseMessage response = await client.PatchAsJsonAsync($"/api/v1/brand/{brandId.Value}",
            new PatchBrandHandler.PatchBrandRequest
            {
                Name = "Te",
                ShortCode = "TB2"
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task WhenShortCodeTooShort_BrandNotUpdated()
    {
        BrandId brandId = new(Guid.NewGuid().ToString());
        Brand brand = new()
        {
            BrandId = brandId,
            Name = "Test Brand",
            ShortCode = "TB"
        };

        await using StashMavenContext context = fixture.CreateDbContext();
        await context.Brands.AddAsync(brand);
        await context.SaveChangesAsync();

        HttpClient client = fixture.CreateClient();

        HttpResponseMessage response = await client.PatchAsJsonAsync($"/api/v1/brand/{brandId.Value}",
            new PatchBrandHandler.PatchBrandRequest
            {
                Name = "Test Brand 2",
                ShortCode = "T"
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
