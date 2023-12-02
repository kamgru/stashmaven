using Microsoft.EntityFrameworkCore;
using StashMaven.WebApi.Data;

namespace StashMaven.WebApi.CatalogFeatures;

[Injectable]
public class GetTaxDefinitionByIdHandler(
    StashMavenContext context)
{
    public class GetTaxDefinitionByIdResponse
    {
        public required string Name { get; set; }
        public required decimal Rate { get; set; }
    }

    public async Task<StashMavenResult<GetTaxDefinitionByIdResponse>> GetTaxDefinitionByIdAsync(
        string taxDefinitionId)
    {
        TaxDefinition? taxDefinition = await context.TaxDefinitions
            .FirstOrDefaultAsync(td => td.TaxDefinitionId.Value == taxDefinitionId);

        if (taxDefinition == null)
        {
            return StashMavenResult<GetTaxDefinitionByIdResponse>.Error("Tax definition not found");
        }

        return StashMavenResult<GetTaxDefinitionByIdResponse>.Success(new GetTaxDefinitionByIdResponse
        {
            Name = taxDefinition.Name,
            Rate = taxDefinition.Rate
        });
    }
}
