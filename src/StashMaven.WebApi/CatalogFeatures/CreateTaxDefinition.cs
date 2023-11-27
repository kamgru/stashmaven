using StashMaven.WebApi.Data;

namespace StashMaven.WebApi.CatalogFeatures;

public class CreateTaxDefinitionHandler
{
    public class CreateTaxDefinitionRequest
    {
        public required string Name { get; set; }
        public required decimal Rate { get; set; }
    }

    private readonly StashMavenContext _context;

    public CreateTaxDefinitionHandler(
        StashMavenContext context)
    {
        _context = context;
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

        await _context.TaxDefinitions.AddAsync(taxDefinition);
        await _context.SaveChangesAsync();

        return taxDefinition.TaxDefinitionId;
    }
}
