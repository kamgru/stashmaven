namespace StashMaven.WebApi.Features.Common.TaxDefinitions;

public partial class TaxDefinitionController
{
    [HttpDelete]
    [Route("{taxDefinitionId}")]
    public async Task<IActionResult> DeleteTaxDefinitionAsync(
        [FromRoute] string taxDefinitionId,
        [FromServices] DeleteTaxDefinitionHandler handler)
    {
        StashMavenResult result = await handler.DeleteTaxDefinitionAsync(
            new DeleteTaxDefinitionHandler.DeleteTaxDefinitionRequest
            {
                TaxDefinitionId = taxDefinitionId
            });

        if (!result.IsSuccess)
        {
            return BadRequest(result.Message);
        }

        return Ok();
    }
}

[Injectable]
public class DeleteTaxDefinitionHandler(StashMavenContext context)
{
    public class DeleteTaxDefinitionRequest
    {
        public required string TaxDefinitionId { get; set; }
    }

    public async Task<StashMavenResult> DeleteTaxDefinitionAsync(
        DeleteTaxDefinitionRequest request)
    {
        if (await context.CatalogItems
                .AnyAsync(x => x.TaxDefinitionId.Value == request.TaxDefinitionId))
        {
            return StashMavenResult.Error("Tax definition is in use and cannot be deleted");
        }

        TaxDefinition? taxDefinition = await context.TaxDefinitions
            .FirstOrDefaultAsync(x => x.TaxDefinitionId.Value == request.TaxDefinitionId);

        if (taxDefinition is null)
        {
            return StashMavenResult.Error("Tax definition not found");
        }

        context.TaxDefinitions.Remove(taxDefinition);
        await context.SaveChangesAsync();

        return StashMavenResult.Success();
    }
}
