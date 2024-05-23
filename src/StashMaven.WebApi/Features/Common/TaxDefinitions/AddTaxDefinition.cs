using StashMaven.WebApi.Data.Services;

namespace StashMaven.WebApi.Features.Common.TaxDefinitions;

public partial class TaxDefinitionController
{
    [HttpPost]
    public async Task<IActionResult> AddTaxDefinitionAsync(
        AddTaxDefinitionHandler.AddTaxDefinitionRequest request,
        [FromServices]
        AddTaxDefinitionHandler handler)
    {
        StashMavenResult<AddTaxDefinitionHandler.AddTaxDefinitionResponse> result =
            await handler.AddTaxDefinitionAsync(request);

        if (!result.IsSuccess || result.Data is null)
        {
            return BadRequest(result.Message);
        }

        return Created($"api/v1/catalog/TaxDefinition/{result.Data}", result.Data);
    }
}

[Injectable]
public class AddTaxDefinitionHandler(
    StashMavenContext context,
    CacheReader cacheReader)
{
    public class AddTaxDefinitionRequest
    {
        public required string Name { get; set; }
        public required decimal Rate { get; set; }
    }

    public record AddTaxDefinitionResponse(string TaxDefinitionId);

    public async Task<StashMavenResult<AddTaxDefinitionResponse>> AddTaxDefinitionAsync(
        AddTaxDefinitionRequest request)
    {
        TaxDefinition taxDefinition = new()
        {
            TaxDefinitionId = new TaxDefinitionId(Guid.NewGuid().ToString()),
            Name = request.Name,
            Rate = request.Rate,
        };

        await context.TaxDefinitions.AddAsync(taxDefinition);
        await context.SaveChangesAsync();
        
        cacheReader.InvalidateKey(CacheReader.Keys.TaxDefinitions);

        return StashMavenResult<AddTaxDefinitionResponse>.Success(
            new AddTaxDefinitionResponse(taxDefinition.TaxDefinitionId.Value));
    }
}
