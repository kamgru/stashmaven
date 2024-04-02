namespace StashMaven.WebApi.Features.Catalog.Products;

public partial class ProductController
{
    [HttpPatch]
    public async Task<IActionResult> PatchProductAsync(
        PatchProductHandler.PatchProductRequest request,
        [FromServices]
        PatchProductHandler handler)
    {
        StashMavenResult response = await handler.PatchProductAsync(request);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Message);
        }

        return Ok();
    }
}

[Injectable]
public class PatchProductHandler(
    StashMavenRepository repository,
    UnitOfWork unitOfWork)
{
    public class PatchProductRequest
    {
        public required string ProductId { get; set; }

        [MinLength(5)]
        public string? Sku { get; set; }

        [MinLength(3)]
        public string? Name { get; set; }

        public UnitOfMeasure? UnitOfMeasure { get; set; }
    }

    public async Task<StashMavenResult> PatchProductAsync(
        PatchProductRequest request)
    {
        Product? product = await repository.GetProductAsync(new ProductId(request.ProductId));

        if (product == null)
        {
            return StashMavenResult.Error(ErrorCodes.ProductNotFound);
        }

        if (request.Sku != null)
        {
            product.Sku = request.Sku;
        }

        if (request.Name != null)
        {
            product.Name = request.Name;
        }

        if (request.UnitOfMeasure != null)
        {
            product.UnitOfMeasure = request.UnitOfMeasure.Value;
        }

        product.UpdatedOn = DateTime.UtcNow;
        
        await unitOfWork.SaveChangesAsync();

        return StashMavenResult.Success();
    }
}
