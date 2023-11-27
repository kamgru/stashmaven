using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StashMaven.WebApi.Data;

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
        StashMavenResult<CatalogItemId> response = await handler.CreateCatalogItemAsync(request);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Message);
        }

        return Created($"api/v1/catalog/catalog-item/{response.Data?.Value}", response.Data?.ToString());
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

    [HttpPatch]
    public async Task<IActionResult> PatchCatalogItemAsync(
        PatchCatalogItemHandler.PatchCatalogItemRequest request,
        [FromServices]
        PatchCatalogItemHandler handler)
    {
        StashMavenResult response = await handler.PatchCatalogItemAsync(request);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Message);
        }

        return Ok();
    }
}
