// using StashMaven.WebApi.Features.Catalog.Brands;
//
// namespace StashMaven.Tests.WebApi.Catalog.Brands;
//
// public class AddProductToBrandTests(
//     AddProductToBrandTestFixture fixture) : IClassFixture<AddProductToBrandTestFixture>
// {
//     [Fact]
//     public async Task WhenRequestValid_AddsProductToBrand()
//     {
//         HttpClient client = fixture.CreateClient();
//
//         const string brandId = "brand-1";
//         const string ProductId = "catalog-item-1";
//
//         HttpResponseMessage response = await client.PostAsJsonAsync(
//             "/api/v1/brand/add-catalog-item",
//             new AddProductToBrandHandler.AddProductToBrandRequest
//             {
//                 BrandId = brandId,
//                 ProductId = ProductId
//             });
//
//         response.EnsureSuccessStatusCode();
//
//         await using StashMavenContext context = fixture.CreateDbContext();
//         Brand? brand = await context.Brands
//             .Include(b => b.Products)
//             .SingleOrDefaultAsync(b => b.BrandId.Value == brandId);
//
//         Assert.NotNull(brand);
//         Assert.Single(brand.Products);
//         Assert.Equal(ProductId, brand.Products.Single().ProductId.Value);
//     }
//
//     [Fact]
//     public async Task WhenBrandNotFound_ReturnsBadRequest()
//     {
//         HttpClient client = fixture.CreateClient();
//
//         const string brandId = "brand-2";
//         const string ProductId = "catalog-item-1";
//
//         HttpResponseMessage response = await client.PostAsJsonAsync(
//             "/api/v1/brand/add-catalog-item",
//             new AddProductToBrandHandler.AddProductToBrandRequest
//             {
//                 BrandId = brandId,
//                 ProductId = ProductId
//             });
//
//         Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
//     }
//
//     [Fact]
//     public async Task WhenProductNotFound_ReturnsBadRequest()
//     {
//         HttpClient client = fixture.CreateClient();
//
//         const string brandId = "brand-1";
//         const string ProductId = "catalog-item-2";
//
//         HttpResponseMessage response = await client.PostAsJsonAsync(
//             "/api/v1/brand/add-catalog-item",
//             new AddProductToBrandHandler.AddProductToBrandRequest
//             {
//                 BrandId = brandId,
//                 ProductId = ProductId
//             });
//
//         Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
//     }
// }
//
// public class AddProductToBrandTestFixture: TestFixture
// {
//     public AddProductToBrandTestFixture()
//         : base("addProducttobrandtests")
//     {
//         using StashMavenContext context = CreateDbContext();
//         context.Database.EnsureCreated();
//         context.Brands.RemoveRange(context.Brands);
//         context.Products.RemoveRange(context.Products);
//
//         context.Brands.Add(new Brand
//         {
//             BrandId = new BrandId("brand-1"),
//             Name = "Brand 1",
//             ShortCode = "B1"
//         });
//
//         TaxDefinition taxDefinition = new()
//         {
//             TaxDefinitionId = new TaxDefinitionId("tax-1"),
//             Name = "Tax 1",
//             Rate = 0.1m
//         };
//         context.TaxDefinitions.Add(taxDefinition);
//
//         context.Products.Add(new Product
//         {
//             ProductId = new ProductId("catalog-item-1"),
//             Name = "Catalog Item 1",
//             Sku = "CI1A",
//             TaxDefinitionId = taxDefinition.TaxDefinitionId,
//             UnitOfMeasure = UnitOfMeasure.Pc
//         });
//
//         context.SaveChanges();
//     }
// }
