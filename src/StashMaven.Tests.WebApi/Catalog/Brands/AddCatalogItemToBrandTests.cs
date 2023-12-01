using System.Net;
using Microsoft.EntityFrameworkCore;
using StashMaven.WebApi.Data;
using StashMaven.WebApi.Features.Catalog.Brands;

namespace StashMaven.Tests.WebApi.Catalog.Brands;

public class AddCatalogItemToBrandTests(
    AddCatalogItemToBrandTestFixture fixture) : IClassFixture<AddCatalogItemToBrandTestFixture>
{
    [Fact]
    public async Task WhenRequestValid_AddsCatalogItemToBrand()
    {
        HttpClient client = fixture.CreateClient();

        const string brandId = "brand-1";
        const string catalogItemId = "catalog-item-1";

        HttpResponseMessage response = await client.PostAsJsonAsync(
            "/api/v1/brand/add-catalog-item",
            new AddCatalogItemToBrandHandler.AddCatalogItemToBrandRequest
            {
                BrandId = brandId,
                CatalogItemId = catalogItemId
            });

        response.EnsureSuccessStatusCode();

        await using StashMavenContext context = fixture.CreateDbContext();
        Brand? brand = await context.Brands
            .Include(b => b.CatalogItems)
            .SingleOrDefaultAsync(b => b.BrandId.Value == brandId);

        Assert.NotNull(brand);
        Assert.Single(brand.CatalogItems);
        Assert.Equal(catalogItemId, brand.CatalogItems.Single().CatalogItemId.Value);
    }

    [Fact]
    public async Task WhenBrandNotFound_ReturnsBadRequest()
    {
        HttpClient client = fixture.CreateClient();

        const string brandId = "brand-2";
        const string catalogItemId = "catalog-item-1";

        HttpResponseMessage response = await client.PostAsJsonAsync(
            "/api/v1/brand/add-catalog-item",
            new AddCatalogItemToBrandHandler.AddCatalogItemToBrandRequest
            {
                BrandId = brandId,
                CatalogItemId = catalogItemId
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task WhenCatalogItemNotFound_ReturnsBadRequest()
    {
        HttpClient client = fixture.CreateClient();

        const string brandId = "brand-1";
        const string catalogItemId = "catalog-item-2";

        HttpResponseMessage response = await client.PostAsJsonAsync(
            "/api/v1/brand/add-catalog-item",
            new AddCatalogItemToBrandHandler.AddCatalogItemToBrandRequest
            {
                BrandId = brandId,
                CatalogItemId = catalogItemId
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}

public class AddCatalogItemToBrandTestFixture: TestFixture
{
    public AddCatalogItemToBrandTestFixture()
        : base("addcatalogitemtobrandtests")
    {
        using StashMavenContext context = CreateDbContext();
        context.Database.EnsureCreated();
        context.Brands.RemoveRange(context.Brands);
        context.CatalogItems.RemoveRange(context.CatalogItems);

        context.Brands.Add(new Brand
        {
            BrandId = new BrandId("brand-1"),
            Name = "Brand 1",
            ShortCode = "B1"
        });

        context.CatalogItems.Add(new CatalogItem
        {
            CatalogItemId = new CatalogItemId("catalog-item-1"),
            Name = "Catalog Item 1",
            Sku = "CI1A",
            TaxDefinition = new TaxDefinition
            {
                TaxDefinitionId = new TaxDefinitionId("tax-1"),
                Name = "Tax 1",
                Rate = 0.1m
            },
            UnitOfMeasure = UnitOfMeasure.Pc
        });

        context.SaveChanges();
    }
}
