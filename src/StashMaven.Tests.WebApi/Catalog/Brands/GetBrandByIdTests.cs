using StashMaven.WebApi.Features.Catalog.Brands;

namespace StashMaven.Tests.WebApi.Catalog.Brands;

public class GetBrandByIdTests(
    DefaultTestFixture fixture) : IClassFixture<DefaultTestFixture>
{
    [Fact]
    public async Task WhenBrandExists_BrandReturned()
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
        await context.DisposeAsync();

        HttpClient client = fixture.CreateClient();

        HttpResponseMessage response = await client.GetAsync($"/api/v1/brand/{brandId.Value}");

        response.EnsureSuccessStatusCode();

        GetBrandByIdHandler.GetBrandByIdResponse? result =
            await response.Content.ReadFromJsonAsync<GetBrandByIdHandler.GetBrandByIdResponse>(
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

        Assert.NotNull(result);
        Assert.Equal(brand.Name, result.Name);
        Assert.Equal(brand.ShortCode, result.ShortCode);
    }

    [Fact]
    public async Task WhenBrandDoesNotExist_NotFoundReturned()
    {
        string brandId = Guid.NewGuid().ToString();

        HttpClient client = fixture.CreateClient();

        HttpResponseMessage response = await client.GetAsync($"/api/v1/brand/{brandId}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
