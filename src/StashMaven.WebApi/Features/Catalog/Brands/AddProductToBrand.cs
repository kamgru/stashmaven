namespace StashMaven.WebApi.Features.Catalog.Brands;

public partial class BrandController
{
    [HttpPost]
    [Route("add-product-to-brand")]
    public async Task<IActionResult> AddProductToBrandAsync(
        AddProductToBrandHandler.AddProductToBrandRequest request,
        [FromServices]
        AddProductToBrandHandler handler)
    {
        StashMavenResult response = await handler.AddProductToBrandAsync(request);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Message);
        }

        return Ok();
    }
}

[Injectable]
public class AddProductToBrandHandler(
    StashMavenContext context)
{
    public class AddProductToBrandRequest
    {
        public required string BrandId { get; set; }
        public required string ProductId { get; set; }
    }

    public async Task<StashMavenResult> AddProductToBrandAsync(
        AddProductToBrandRequest request)
    {
        Brand? brand = await context.Brands
            .Include(b => b.Products)
            .SingleOrDefaultAsync(b => b.BrandId.Value == request.BrandId);

        if (brand == null)
        {
            return StashMavenResult.Error($"Brand {request.BrandId} not found");
        }

        Product? product = await context.Products
            .SingleOrDefaultAsync(c => c.ProductId.Value == request.ProductId);

        if (product == null)
        {
            return StashMavenResult.Error($"Catalog item {request.ProductId} not found");
        }

        brand.Products.Add(product);
        await context.SaveChangesAsync();

        return StashMavenResult.Success();
    }
}
