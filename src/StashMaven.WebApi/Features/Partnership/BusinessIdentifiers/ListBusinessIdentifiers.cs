using StashMaven.WebApi.Data.Services;

namespace StashMaven.WebApi.Features.Partnership.BusinessIdentifiers;

public partial class BusinessIdentifierController
{
    [HttpGet]
    [Route("list")]
    [ProducesResponseType<ListBusinessIdentifiersHandler.ListBusinessIdentifiersResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListBusinessIdentifiersAsync(
        [FromQuery]
        ListBusinessIdentifiersHandler.ListBusinessIdentifiersRequest request,
        [FromServices]
        ListBusinessIdentifiersHandler handler)
    {
        ListBusinessIdentifiersHandler.ListBusinessIdentifiersResponse response =
            await handler.ListBusinessIdentifiersAsync(request);

        return Ok(response);
    }
}

[Injectable]
public class ListBusinessIdentifiersHandler(CacheReader cacheReader)
{
    private const int MinPageSize = 5;
    private const int MaxPageSize = 100;
    private const int MinPage = 1;
    private const int MinSearchLength = 3;
    
    public class ListBusinessIdentifiersRequest
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string? Search { get; set; }
        public bool IsAscending { get; set; }
        public string? SortBy { get; set; }
    }
    
    public class BusinessIdentifierItem
    {
        public required string BusinessIdentifierId { get; set; }
        public required string Name { get; set; }
        public required string ShortCode { get; set; }
    }
    
    public class ListBusinessIdentifiersResponse
    {
        public List<BusinessIdentifierItem> Items { get; set; } = [];
        public int TotalCount { get; set; }
    }
    
    public async Task<ListBusinessIdentifiersResponse> ListBusinessIdentifiersAsync(
        ListBusinessIdentifiersRequest request)
    {
        request.PageSize = Math.Clamp(request.PageSize, MinPageSize, MaxPageSize);
        request.Page = Math.Max(request.Page, MinPage);
        
        IReadOnlyList<BusinessIdentifier> businessIdentifiers = await cacheReader.GetBusinessIdentifiersAsync();
        
        IEnumerable<BusinessIdentifier> result = businessIdentifiers.AsEnumerable();
        
        if (request.Search is not null && request.Search.Length >= MinSearchLength)
        {
            result = result.Where(x => x.Name.Contains(request.Search, StringComparison.InvariantCultureIgnoreCase)
                                       || x.ShortCode.Contains(request.Search,
                                           StringComparison.InvariantCultureIgnoreCase));
        }
        
        if (request.SortBy is not null)
        {
            result = request.SortBy.ToLowerInvariant() switch
            {
                "name" => request.IsAscending
                    ? result.OrderBy(x => x.Name)
                    : result.OrderByDescending(x => x.Name),
                "shortcode" => request.IsAscending
                    ? result.OrderBy(x => x.ShortCode)
                    : result.OrderByDescending(x => x.ShortCode),
                _ => result
            };
        }
        
        int totalCount = result.Count();
        
        List<BusinessIdentifierItem> stockpileItems = result.Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new BusinessIdentifierItem
            {
                BusinessIdentifierId = x.BusinessIdentifierId.Value,
                Name = x.Name,
                ShortCode = x.ShortCode,
            })
            .ToList();
        
        return new ListBusinessIdentifiersResponse
        {
            Items = stockpileItems,
            TotalCount = totalCount
        };
    }
}
