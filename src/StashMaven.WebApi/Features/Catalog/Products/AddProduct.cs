using Npgsql;

namespace StashMaven.WebApi.Features.Catalog.Products;

public partial class ProductController
{
    [HttpPost]
    public async Task<IActionResult> AddProductAsync(
        AddProductHandler.AddProductRequest request,
        [FromServices]
        AddProductHandler handler)
    {
        StashMavenResult<AddProductHandler.AddProductResponse> response =
            await handler.AddProductAsync(request);

        if (!response.IsSuccess)
        {
            return BadRequest(response.ErrorCode);
        }

        return Created($"api/v1/product/{response.Data?.ProductId}", response.Data);
    }
}

[Injectable]
public class AddProductHandler(
    StashMavenContext context)
{
    public class AddProductRequest
    {
        [MinLength(5)]
        [MaxLength(50)]
        public required string Sku { get; set; }

        [MinLength(3)]
        [MaxLength(256)]
        public required string Name { get; set; } = null!;

        public UnitOfMeasure UnitOfMeasure { get; set; }
    }

    public record AddProductResponse(string ProductId);

    public async Task<StashMavenResult<AddProductResponse>> AddProductAsync(
        AddProductRequest request)
    {
        ProductId productId = new(Guid.NewGuid().ToString());

        Product product = new()
        {
            ProductId = productId,
            Sku = request.Sku,
            Name = request.Name,
            UnitOfMeasure = request.UnitOfMeasure,
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow
        };

        context.Products.Add(product);

        try
        {
            await context.SaveChangesAsync();
            return StashMavenResult<AddProductResponse>.Success(new AddProductResponse(productId.Value));
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
            {
                return StashMavenResult<AddProductResponse>.Error(ErrorCodes.SkuAlreadyExists);
            }

            throw;
        }
    }
}
