namespace StashMaven.WebApi.Features.Common.TaxDefinitions;

public partial class TaxDefinitionController
{
    [HttpPost]
    public async Task<IActionResult> AddTaxDefinitionAsync(
        AddTaxDefinitionHandler.AddTaxDefinitionRequest request,
        [FromServices]
        AddTaxDefinitionHandler handler)
    {
        StashMavenResult<AddTaxDefinitionHandler.AddTaxDefinitionResponse> result = await handler.AddTaxDefinitionAsync(request);

        if (!result.IsSuccess || result.Data is null)
        {
            return BadRequest(result.Message);
        }

        return Created($"api/v1/catalog/TaxDefinition/{result.Data.TaxDefinitionId}", result.Data);
    }
}

[Injectable]
public class AddTaxDefinitionHandler(
    StashMavenContext context)
{
    public class AddTaxDefinitionRequest
    {
        public required string Name { get; set; }
        public required decimal Rate { get; set; }
    }

    public class AddTaxDefinitionResponse
    {
        public required string TaxDefinitionId { get; set; }
    }

    public async Task<StashMavenResult<AddTaxDefinitionResponse>> AddTaxDefinitionAsync(
        AddTaxDefinitionRequest request)
    {
        TaxDefinition taxDefinition = new()
        {
            TaxDefinitionId = new TaxDefinitionId(Guid.NewGuid().ToString()),
            Name = request.Name,
            Rate = request.Rate
        };

        await context.TaxDefinitions.AddAsync(taxDefinition);
        await context.SaveChangesAsync();

        return StashMavenResult<AddTaxDefinitionResponse>.Success(new AddTaxDefinitionResponse
        {
            TaxDefinitionId = taxDefinition.TaxDefinitionId.Value
        });
    }
}
