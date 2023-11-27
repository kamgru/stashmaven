using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using StashMaven.WebApi.Data;

namespace StashMaven.WebApi.CatalogFeatures;

public class UpdateBrandHandler
{
    public class UpdateBrandRequest
    {
        public required string BrandId { get; set; }

        [MinLength(3)]
        public string? Name { get; set; }

        [MinLength(2)]
        public string? ShortCode { get; set; }
    }

    private readonly StashMavenContext _context;

    public UpdateBrandHandler(
        StashMavenContext context)
    {
        _context = context;
    }

    public async Task<StashMavenResult> UpdateBrandAsync(
        UpdateBrandRequest request)
    {
        Brand? brand = await _context.Brands
            .FirstOrDefaultAsync(b => b.BrandId.Value == request.BrandId);

        if (brand == null)
        {
            return StashMavenResult.Error($"Brand {request.BrandId} not found");
        }

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            brand.Name = request.Name;
        }

        if (!string.IsNullOrWhiteSpace(request.ShortCode))
        {
            brand.ShortCode = request.ShortCode;
        }

        if (_context.ChangeTracker.HasChanges())
        {
            await _context.SaveChangesAsync();
        }

        return StashMavenResult.Success();
    }
}
