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
        public List<TaxDefinition> TaxDefinitions { get; set; } = new();
        public int TotalCount { get; set; }
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

        List<TaxDefinition> taxDefinitions = await _context.TaxDefinitions
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
