namespace StashMaven.WebApi.Features.Common.TaxDefinitions;

public partial class TaxDefinitionController
{
    [HttpPatch]
    [Route("{taxDefinitionId}")]
    public async Task<IActionResult> PatchTaxDefinitionAsync(
        string taxDefinitionId,
        PatchTaxDefinitionHandler.PatchTaxDefinitionRequest request,
        [FromServices]
        PatchTaxDefinitionHandler handler)
    {
        StashMavenResult response = await handler.PatchTaxDefinitionAsync(taxDefinitionId, request);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Message);
        }

        return Ok();
    }
}

[Injectable]
public class PatchTaxDefinitionHandler(StashMavenContext context)
{
    public class PatchTaxDefinitionRequest
    {
        [MinLength(3)]
        public required string Name { get; set; }
    }

    public async Task<StashMavenResult> PatchTaxDefinitionAsync(
        string taxDefinitionId,
        PatchTaxDefinitionRequest request)
    {
        TaxDefinition? taxDefinition = await context.TaxDefinitions
            .FirstOrDefaultAsync(x => x.TaxDefinitionId.Value == taxDefinitionId);

        if (taxDefinition is null)
        {
            return StashMavenResult.Error("Tax definition not found");
        }

        taxDefinition.Name = request.Name;

        await context.SaveChangesAsync();

        return StashMavenResult.Success();
    }
}
