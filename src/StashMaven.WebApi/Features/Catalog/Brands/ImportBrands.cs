using System.Text.Json;

namespace StashMaven.WebApi.Features.Catalog.Brands;

public partial class BrandController
{
    [HttpPost]
    [Route("import")]
    public async Task<IActionResult> ImportBrandsAsync(
        IFormFile file,
        [FromServices]
        ImportBrandHandler handler)
    {
        if (file.Length == 0)
        {
            return BadRequest();
        }

        await using Stream stream = file.OpenReadStream();
        List<ImportBrandHandler.ImportedBrand>? brands = await JsonSerializer.DeserializeAsync<List<ImportBrandHandler.ImportedBrand>>(
            stream, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        if (brands == null)
        {
            return BadRequest("Could not deserialize input file.");
        }
        
        StashMavenResult result = await handler.ImportBrandsAsync(brands);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Message);
        }

        return Ok();
    }
}

[Injectable]
public class ImportBrandHandler(StashMavenContext context)
{
    public class ImportedBrand
    {
        public string? Name { get; set; }
        public string? ShortCode { get; set; }
    }

    public async Task<StashMavenResult> ImportBrandsAsync(
        List<ImportedBrand> brands)
    {
        foreach (ImportedBrand importedBrand in brands)
        {
            if (importedBrand.Name == null || importedBrand.ShortCode == null)
            {
                return StashMavenResult.Error("Name and ShortCode are required.");
            }

            Brand brand = new()
            {
                BrandId = new BrandId(Guid.NewGuid().ToString()),
                Name = importedBrand.Name,
                ShortCode = importedBrand.ShortCode,
            };

            context.Brands.Add(brand);
        }

        await context.SaveChangesAsync();

        return StashMavenResult.Success();
    }
}
