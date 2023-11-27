using System.ComponentModel.DataAnnotations;
using StashMaven.WebApi.Data;

namespace StashMaven.WebApi.CatalogFeatures;

public class CreateBrandHandler
{
    public class CreateBrandRequest
    {
        [MinLength(3)]
        public required string Name { get; set; }

        [MinLength(2)]
        public required string ShortCode { get; set; }
    }

    private readonly StashMavenContext _context;

    public CreateBrandHandler(
        StashMavenContext context)
    {
        _context = context;
    }

    public async Task<StashMavenResult<BrandId>> CreateBrandAsync(
        CreateBrandRequest request)
    {
        BrandId brandId = new(Guid.NewGuid().ToString());
        Brand brand = new()
        {
            BrandId = brandId,
            Name = request.Name,
            ShortCode = request.ShortCode,
        };

        await _context.Brands.AddAsync(brand);
        await _context.SaveChangesAsync();

        return StashMavenResult<BrandId>.Success(brandId);
    }
}
