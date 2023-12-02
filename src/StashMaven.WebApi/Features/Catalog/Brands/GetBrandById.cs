using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StashMaven.WebApi.Data;

namespace StashMaven.WebApi.Features.Catalog.Brands;

public partial class BrandController
{
    [HttpGet]
    [Route("{brandId}")]
    public async Task<IActionResult> GetBrandByIdAsync(
        string brandId,
        [FromServices]
        GetBrandByIdHandler handler)
    {
        StashMavenResult<GetBrandByIdHandler.GetBrandByIdResponse> response =
            await handler.GetBrandByIdAsync(brandId);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Message);
        }

        return Ok(response.Data);
    }
}

[Injectable]
public class GetBrandByIdHandler(
    StashMavenContext context)
{
    public class GetBrandByIdResponse
    {
        public required string Name { get; set; }
        public required string ShortCode { get; set; }
    }

    public async Task<StashMavenResult<GetBrandByIdResponse>> GetBrandByIdAsync(
        string brandId)
    {
        Brand? brand = await context.Brands.FirstOrDefaultAsync(x => x.BrandId.Value == brandId);

        if (brand == null)
        {
            return StashMavenResult<GetBrandByIdResponse>.Error("Brand not found");
        }

        return StashMavenResult<GetBrandByIdResponse>.Success(new GetBrandByIdResponse
        {
            Name = brand.Name,
            ShortCode = brand.ShortCode
        });
    }
}
