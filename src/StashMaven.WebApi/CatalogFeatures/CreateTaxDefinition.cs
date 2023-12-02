using StashMaven.WebApi.Data;

namespace StashMaven.WebApi.CatalogFeatures;

[Injectable]
public class CreateTaxDefinitionHandler(
    StashMavenContext context)
{
    public class CreateTaxDefinitionRequest
    {
        public required string Name { get; set; }
        public required decimal Rate { get; set; }
    }

    public async Task<TaxDefinitionId> CreateTaxDefinitionAsync(
        CreateTaxDefinitionRequest request)
    {
        TaxDefinition taxDefinition = new()
        {
            TaxDefinitionId = new TaxDefinitionId(Guid.NewGuid().ToString()),
            Name = request.Name,
            Rate = request.Rate
        };

        await context.TaxDefinitions.AddAsync(taxDefinition);
        await context.SaveChangesAsync();

        return taxDefinition.TaxDefinitionId;
    }
}
