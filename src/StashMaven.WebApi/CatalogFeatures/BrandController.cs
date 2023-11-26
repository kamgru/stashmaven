using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StashMaven.WebApi.CatalogFeatures;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class BrandController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateBrandAsync(
        CreateBrandHandler.CreateBrandRequest request,
        [FromServices]
        CreateBrandHandler handler)
    {
        StashMavenResult<Guid> response = await handler.CreateBrandAsync(request);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Message);
        }

        return Created($"api/v1/catalog/brand/{response.Data}", response.Data.ToString());
    }

    [HttpGet]
    [Route("list")]
    public async Task<IActionResult> ListBrandsAsync(
        [FromQuery]
        ListBrandsHandler.ListBrandsRequest request,
        [FromServices]
        ListBrandsHandler handler)
    {
        ListBrandsHandler.ListBrandsResponse response =
            await handler.ListBrandsAsync(request);
        return Ok(response);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateBrandAsync(
        UpdateBrandHandler.UpdateBrandRequest request,
        [FromServices]
        UpdateBrandHandler handler)
    {
        StashMavenResult response = await handler.UpdateBrandAsync(request);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Message);
        }

        return Ok();
    }

    [HttpPost]
    [Route("add-catalog-item")]
    public async Task<IActionResult> AddCatalogItemToBrandAsync(
        AddCatalogItemToBrandHandler.AddCatalogItemToBrandRequest request,
        [FromServices]
        AddCatalogItemToBrandHandler handler)
    {
        StashMavenResult response = await handler.AddCatalogItemToBrandAsync(request);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Message);
        }

        return Ok();
    }
}
