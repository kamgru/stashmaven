namespace StashMaven.WebApi.Features.Catalog.Products;

public partial class ProductController
{
    [HttpGet]
    [Route("{productId}")]
    [ProducesResponseType<GetProductDetailsHandler.GetProductDetailsResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetProductDetailsAsync(
        string productId,
        [FromServices]
        GetProductDetailsHandler handler)
    {
        StashMavenResult<GetProductDetailsHandler.GetProductDetailsResponse> result =
            await handler.GetProductDetailsAsync(productId);

        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorCode);
        }

        return Ok(result.Data);
    }

    [Injectable]
    public class GetProductDetailsHandler(StashMavenContext context)
    {
        public class GetProductDetailsResponse
        {
            public required string Sku { get; set; }
            public required string Name { get; set; }
            public required UnitOfMeasure UnitOfMeasure { get; set; }
        }

        public async Task<StashMavenResult<GetProductDetailsResponse>> GetProductDetailsAsync(
            string productId)
        {
            Product? product = await context.Products
                .FirstOrDefaultAsync(ci => ci.ProductId.Value == productId);

            if (product is null)
            {
                return StashMavenResult<GetProductDetailsResponse>.Error(ErrorCodes.ProductNotFound);
            }

            return StashMavenResult<GetProductDetailsResponse>.Success(
                new GetProductDetailsResponse
                {
                    Sku = product.Sku,
                    Name = product.Name,
                    UnitOfMeasure = product.UnitOfMeasure
                });
        }
    }
}
