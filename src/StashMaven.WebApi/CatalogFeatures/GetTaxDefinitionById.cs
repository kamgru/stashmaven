using Microsoft.EntityFrameworkCore;
using StashMaven.WebApi.Data;

namespace StashMaven.WebApi.CatalogFeatures;

public class GetTaxDefinitionByIdHandler
{
    public class GetTaxDefinitionByIdResponse
    {
        public required string Name { get; set; }
        public required decimal Rate { get; set; }
    }

    private readonly StashMavenContext _context;

    public GetTaxDefinitionByIdHandler(
        StashMavenContext context)
    {
        _context = context;
    }

    public async Task<StashMavenResult<GetTaxDefinitionByIdResponse>> GetTaxDefinitionByIdAsync(
        string taxDefinitionId)
    {
        TaxDefinition? taxDefinition = await _context.TaxDefinitions
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
