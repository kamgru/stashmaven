using Microsoft.EntityFrameworkCore;
using StashMaven.WebApi.Data;

namespace StashMaven.WebApi.CatalogFeatures;

public class ListTaxDefinitionsHandler
{
    public class ListTaxDefinitionsRequest
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
    }

    public class ListTaxDefinitionsResponse
    {
        public List<TaxDefinitionItem> TaxDefinitions { get; set; } = new();
        public int TotalCount { get; set; }
    }

    public class TaxDefinitionItem
    {
        public required string TaxDefinitionId { get; set; }
        public required string Name { get; set; }
        public required decimal Rate { get; set; }
    }

    private readonly StashMavenContext _context;

    public ListTaxDefinitionsHandler(
        StashMavenContext context)
    {
        _context = context;
    }

    public async Task<ListTaxDefinitionsResponse> ListTaxDefinitionsAsync(
        ListTaxDefinitionsRequest request)
    {
        int page = Math.Max(1, request.Page);
        int pageSize = Math.Clamp(request.PageSize, 10, 100);

        List<TaxDefinitionItem> taxDefinitions = await _context.TaxDefinitions
            .Select(x => new TaxDefinitionItem
            {
                Name = x.Name,
                Rate = x.Rate,
                TaxDefinitionId = x.TaxDefinitionId.Value
            })
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        int totalCount = await _context.TaxDefinitions.CountAsync();

        return new ListTaxDefinitionsResponse
        {
            TaxDefinitions = taxDefinitions,
            TotalCount = totalCount
        };
    }
}
