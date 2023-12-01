using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StashMaven.WebApi.Data;

namespace StashMaven.WebApi.Features.Catalog.Brands;

public partial class BrandController
{
    [HttpPatch]
    [Route("{brandId}")]
    public async Task<IActionResult> PatchBrandAsync(
        string brandId,
        PatchBrandHandler.PatchBrandRequest request,
        [FromServices]
        PatchBrandHandler handler)
    {
        StashMavenResult response = await handler.PatchBrandAsync(brandId, request);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Message);
        }

        return Ok();
    }
}

public class PatchBrandHandler(
    StashMavenContext context)
{
    public class PatchBrandRequest
    {
        [MinLength(3)]
        public string? Name { get; set; }

        [MinLength(2)]
        public string? ShortCode { get; set; }
    }

    public async Task<StashMavenResult> PatchBrandAsync(
        string brandId,
        PatchBrandRequest request)
    {
        Brand? brand = await context.Brands
            .FirstOrDefaultAsync(b => b.BrandId.Value == brandId);

        if (brand == null)
        {
            return StashMavenResult.Error($"Brand {brandId} not found");
        }

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            brand.Name = request.Name;
        }

        if (!string.IsNullOrWhiteSpace(request.ShortCode))
        {
            brand.ShortCode = request.ShortCode;
        }

        if (context.ChangeTracker.HasChanges())
        {
            await context.SaveChangesAsync();
        }

        return StashMavenResult.Success();
    }
}
