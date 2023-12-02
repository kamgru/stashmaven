using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StashMaven.WebApi.Data;

namespace StashMaven.WebApi.Features.Catalog.Brands;

public partial class BrandController
{
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
}

[Injectable]
public class ListBrandsHandler(
    StashMavenContext context)
{
    private const int MinPageSize = 5;
    private const int MaxPageSize = 100;
    private const int MinPage = 1;
    private const int MinSearchLength = 3;

    public class Brand
    {
        public required string BrandId { get; set; }
        public required string Name { get; set; }
        public required string ShortCode { get; set; }
    }

    public class ListBrandsRequest
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string? Search { get; set; }
        public bool IsAscending { get; set; }
    }

    public class ListBrandsResponse
    {
        public List<Brand> Brands { get; set; } = new();
        public int TotalCount { get; set; }
    }

    public async Task<ListBrandsResponse> ListBrandsAsync(
        ListBrandsRequest request)
    {
        request.PageSize = Math.Clamp(request.PageSize, MinPageSize, MaxPageSize);
        request.Page = Math.Max(request.Page, MinPage);

        IQueryable<Brand> brands = context.Brands
            .Select(b => new Brand
            {
                BrandId = b.BrandId.Value,
                Name = b.Name,
                ShortCode = b.ShortCode,
            });

        brands = request.IsAscending ? brands.OrderBy(x => x.Name) : brands.OrderByDescending(x => x.Name);

        if (!string.IsNullOrWhiteSpace(request.Search) && request.Search.Length >= MinSearchLength)
        {
            string searchPhrase = $"%{request.Search}%";
            brands = brands.Where(
                x => EF.Functions.Like(x.Name, searchPhrase));
        }

        int totalCount = await brands.CountAsync();

        brands = brands
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize);

        return new ListBrandsResponse
        {
            Brands = await brands.ToListAsync(),
            TotalCount = totalCount,
        };
    }
}
