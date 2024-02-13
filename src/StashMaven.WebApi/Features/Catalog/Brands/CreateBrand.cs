namespace StashMaven.WebApi.Features.Catalog.Brands;

public partial class BrandController
{
    [HttpPost]
    [ProducesResponseType<BrandId>(StatusCodes.Status201Created)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateBrandAsync(
        CreateBrandHandler.CreateBrandRequest request,
        [FromServices]
        CreateBrandHandler handler)
    {
        StashMavenResult<BrandId> response = await handler.CreateBrandAsync(request);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Message);
        }

        return Created($"api/v1/catalog/brand/{response.Data?.Value}", response.Data);
    }
}

[Injectable]
public class CreateBrandHandler(
    StashMavenContext context)
{
    public class CreateBrandRequest
    {
        [MinLength(3)]
        public required string Name { get; set; }

        [MinLength(2)]
        public required string ShortCode { get; set; }
    }

    public async Task<StashMavenResult<BrandId>> CreateBrandAsync(
        CreateBrandRequest request)
    {
        int count = await context.Brands.CountAsync(x => x.ShortCode == request.ShortCode);
        if (count > 0)
        {
            return StashMavenResult<BrandId>.Error("ShortCode must be unique");
        }

        BrandId brandId = new(Guid.NewGuid().ToString());
        Brand brand = new()
        {
            BrandId = brandId,
            Name = request.Name,
            ShortCode = request.ShortCode,
        };

        await context.Brands.AddAsync(brand);
        await context.SaveChangesAsync();

        return StashMavenResult<BrandId>.Success(brandId);
    }
}
