using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StashMaven.WebApi.CatalogFeatures;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class CatalogItemController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateCatalogItemAsync(
        CreateCatalogItemHandler.CreateCatalogItemRequest request,
        [FromServices]
        CreateCatalogItemHandler handler)
    {
        StashMavenResult<Guid> response = await handler.CreateCatalogItemAsync(request);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Message);
        }

        return Created($"api/v1/catalog/catalog-item/{response.Data}", response.Data.ToString());
    }

    [HttpGet]
    [Route("list")]
    public async Task<IActionResult> ListCatalogItemsAsync(
        [FromQuery]
        ListCatalogItemsHandler.ListCatalogItemsRequest request,
        [FromServices]
        ListCatalogItemsHandler handler)
    {
        ListCatalogItemsHandler.ListCatalogItemsResponse response =
            await handler.ListCatalogItemsAsync(request);
        return Ok(response);
    }
}
