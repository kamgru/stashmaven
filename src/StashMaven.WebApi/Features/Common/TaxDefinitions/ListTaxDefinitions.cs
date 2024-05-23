using StashMaven.WebApi.Data.Services;

namespace StashMaven.WebApi.Features.Common.TaxDefinitions;

public partial class TaxDefinitionController
{
    [HttpGet]
    [Route("list")]
    public async Task<IActionResult> ListTaxDefinitionsAsync(
        [FromQuery]
        ListTaxDefinitionsHandler.ListTaxDefinitionsRequest request,
        [FromServices]
        ListTaxDefinitionsHandler handler)
    {
        ListTaxDefinitionsHandler.ListTaxDefinitionsResponse response =
            await handler.ListTaxDefinitionsAsync(request);
        return Ok(response);
    }
}

[Injectable]
public class ListTaxDefinitionsHandler(CacheReader cacheReader)
{
    private const int MinPageSize = 5;
    private const int MaxPageSize = 100;
    private const int MinPage = 1;
    private const int MinSearchLength = 3;
    
    public class ListTaxDefinitionsRequest
    {
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public string? Search { get; set; }
        public bool IsAscending { get; set; }
        public string? SortBy { get; set; }
    }
    
    public class TaxDefinitionItem
    {
        public required string TaxDefinitionId { get; set; }
        public required string Name { get; set; }
        public decimal Rate { get; set; }
    }
    
    public class ListTaxDefinitionsResponse
    {
        public List<TaxDefinitionItem> Items { get; set; } = [];
        public int TotalCount { get; set; }
    }
    
    public async Task<ListTaxDefinitionsResponse> ListTaxDefinitionsAsync(
        ListTaxDefinitionsRequest request)
    {
        IReadOnlyList<TaxDefinition> taxDefinitions = await cacheReader.GetTaxDefinitionsAsync();
        
        if (request.Search is not null && request.Search.Length >= MinSearchLength)
        {
            taxDefinitions = taxDefinitions
                .Where(x => x.Name.Contains(request.Search, StringComparison.InvariantCultureIgnoreCase))
                .ToList();
        }
        
        if (request.SortBy is not null)
        {
            taxDefinitions = request.SortBy.ToLowerInvariant() switch
            {
                "name" => request.IsAscending
                    ? taxDefinitions.OrderBy(x => x.Name).ToList()
                    : taxDefinitions.OrderByDescending(x => x.Name).ToList(),
                _ => taxDefinitions
            };
        }
        
        int totalCount = taxDefinitions.Count;
        
        if (request is { Page: { } page, PageSize: { } pageSize })
        {
            request.Page = Math.Max(page, MinPage);
            request.PageSize = Math.Clamp(pageSize, MinPageSize, MaxPageSize);
            
            taxDefinitions = taxDefinitions.Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }
        
        List<TaxDefinitionItem> stockpileItems = taxDefinitions
            .Select(x => new TaxDefinitionItem
            {
                TaxDefinitionId = x.TaxDefinitionId.Value,
                Name = x.Name,
                Rate = x.Rate
            })
            .ToList();
        
        return new ListTaxDefinitionsResponse
        {
            Items = stockpileItems,
            TotalCount = totalCount
        };
    }
}
