using Npgsql;

namespace StashMaven.WebApi.Features.Catalog.Brands;

public partial class BrandController
{
    [HttpPut]
    [Route("{brandId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<int>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateBrandAsync(
        string brandId,
        UpdateBrandHandler.UpdateBrandRequest request,
        [FromServices]
        UpdateBrandHandler handler)
    {
        StashMavenResult response = await handler.UpdateBrandAsync(brandId, request);

        if (!response.IsSuccess)
        {
            return BadRequest(response.ErrorCode);
        }

        return Ok();
    }
}

[Injectable]
public class UpdateBrandHandler(
    StashMavenContext context)
{
    public class UpdateBrandRequest
    {
        [MinLength(3)]
        public required string Name { get; init; }

        [MinLength(2)]
        public required string ShortCode { get; init; }
    }

    public async Task<StashMavenResult> UpdateBrandAsync(
        string brandId,
        UpdateBrandRequest request)
    {
        Brand? brand = await context.Brands
            .AsTracking()
            .FirstOrDefaultAsync(b => b.BrandId.Value == brandId);

        if (brand == null)
        {
            return StashMavenResult.Error(ErrorCodes.BrandNotFound);
        }

        brand.Name = request.Name;
        brand.ShortCode = request.ShortCode;
        
        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
            {
                return StashMavenResult.Error(ErrorCodes.BrandShortCodeNotUnique);
            }
            
            throw;
        }
        
        return StashMavenResult.Success();
    }
}
