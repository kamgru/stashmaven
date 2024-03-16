namespace StashMaven.WebApi.Features.Catalog.CatalogItems;

public partial class CatalogItemController
{
    [HttpGet]
    [Route("{catalogItemId}")]
    [ProducesResponseType<GetCatalogItemDetailsHandler.GetCatalogItemDetailsResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetCatalogItemDetailsAsync(
        string catalogItemId,
        [FromServices]
        GetCatalogItemDetailsHandler handler)
    {
        StashMavenResult<GetCatalogItemDetailsHandler.GetCatalogItemDetailsResponse> result =
            await handler.GetCatalogItemDetailsAsync(catalogItemId);

        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorCode);
        }

        return Ok(result.Data);
    }

    [Injectable]
    public class GetCatalogItemDetailsHandler(StashMavenContext context)
    {
        public class GetCatalogItemDetailsResponse
        {
            public required string Sku { get; set; }
            public required string Name { get; set; }
            public required UnitOfMeasure UnitOfMeasure { get; set; }
        }

        public async Task<StashMavenResult<GetCatalogItemDetailsResponse>> GetCatalogItemDetailsAsync(
            string catalogItemId)
        {
            CatalogItem? catalogItem = await context.CatalogItems
                .FirstOrDefaultAsync(ci => ci.CatalogItemId.Value == catalogItemId);

            if (catalogItem is null)
            {
                return StashMavenResult<GetCatalogItemDetailsResponse>.Error(ErrorCodes.CatalogItemNotFound);
            }

            return StashMavenResult<GetCatalogItemDetailsResponse>.Success(
                new GetCatalogItemDetailsResponse
                {
                    Sku = catalogItem.Sku,
                    Name = catalogItem.Name,
                    UnitOfMeasure = catalogItem.UnitOfMeasure
                });
        }
    }
}
