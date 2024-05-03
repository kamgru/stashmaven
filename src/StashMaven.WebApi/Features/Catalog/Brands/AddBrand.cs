using Npgsql;

namespace StashMaven.WebApi.Features.Catalog.Brands;

public partial class BrandController
{
    [HttpPost]
    [ProducesResponseType<BrandId>(StatusCodes.Status201Created)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddBrandAsync(
        AddBrandHandler.AddBrandRequest request,
        [FromServices]
        AddBrandHandler handler)
    {
        StashMavenResult<AddBrandHandler.AddBrandResponse> response =
            await handler.CreateBrandAsync(request);
        
        if (!response.IsSuccess)
        {
            return BadRequest(response.ErrorCode);
        }
        
        return Created($"api/v1/catalog/brand/{response.Data?.BrandId}", response.Data);
    }
}

[Injectable]
public class AddBrandHandler(
    StashMavenContext context)
{
    public class AddBrandRequest
    {
        [MinLength(3)]
        public required string Name { get; set; }
        
        [MinLength(2)]
        public required string ShortCode { get; set; }
    }
    
    public record AddBrandResponse(string BrandId);
    
    public async Task<StashMavenResult<AddBrandResponse>> CreateBrandAsync(
        AddBrandRequest request)
    {
        BrandId brandId = new(Guid.NewGuid().ToString());
        Brand brand = new()
        {
            BrandId = brandId,
            Name = request.Name,
            ShortCode = request.ShortCode,
        };
        
        context.Brands.Add(brand);
        
        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
            {
                return StashMavenResult<AddBrandResponse>.Error(ErrorCodes.BrandShortCodeNotUnique);
            }
            
            throw;
        }
        
        return StashMavenResult<AddBrandResponse>.Success(
            new AddBrandResponse(brandId.Value));
    }
}
