using StashMaven.WebApi.Data.Services;

namespace StashMaven.WebApi.Features.Inventory.ShipmentKinds;

public partial class ShipmentKindController
{
    [HttpGet]
    [Route("list")]
    [ProducesResponseType<ListShipmentKindsHandler.ListShipmentKindsResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListShipmentKindsAsync(
        [FromQuery]
        ListShipmentKindsHandler.ListShipmentKindsRequest request,
        [FromServices]
        ListShipmentKindsHandler handler)
    {
        ListShipmentKindsHandler.ListShipmentKindsResponse response =
            await handler.ListShipmentKindsAsync(request);

        return Ok(response);
    }
}

[Injectable]
public class ListShipmentKindsHandler(CacheReader cacheReader)
{
    private const int MinPageSize = 5;
    private const int MaxPageSize = 100;
    private const int MinPage = 1;
    private const int MinSearchLength = 3;
    
    public class ListShipmentKindsRequest
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string? Search { get; set; }
        public string? SortBy { get; set; }
        public bool IsAscending { get; set; }
    }
    
    public class ShipmentKindItem
    {
        public required string ShipmentKindId { get; set; }
        public required string Name { get; set; }
        public required string ShortCode { get; set; }
        public required string Direction { get; set; }
    }
    
    public class ListShipmentKindsResponse
    {
        public List<ShipmentKindItem> Items { get; set; } = [];
        public int TotalCount { get; set; }
    }
    
    public async Task<ListShipmentKindsResponse> ListShipmentKindsAsync(
        ListShipmentKindsRequest request)
    {
        request.PageSize = Math.Clamp(request.PageSize, MinPageSize, MaxPageSize);
        request.Page = Math.Max(request.Page, MinPage);
        
        IReadOnlyList<ShipmentKind> shipmentKinds = await cacheReader.GetKindsAsync();
        
        IEnumerable<ShipmentKind> result = shipmentKinds.AsEnumerable();
        
        if (!string.IsNullOrWhiteSpace(request.Search) && request.Search.Length >= MinSearchLength)
        {
            result = result.Where(
                x => x.Name.Contains(request.Search, StringComparison.OrdinalIgnoreCase)
                     || x.ShortCode.Contains(request.Search, StringComparison.OrdinalIgnoreCase));
        }
        
        if (request.SortBy is not null)
        {
            result = request.SortBy switch
            {
                "name" => request.IsAscending
                    ? result.OrderBy(x => x.Name)
                    : result.OrderByDescending(x => x.Name),
                "shortcode" => request.IsAscending
                    ? result.OrderBy(x => x.ShortCode)
                    : result.OrderByDescending(x => x.ShortCode),
                "direction" => request.IsAscending
                    ? result.OrderBy(x => x.Direction)
                    : result.OrderByDescending(x => x.Direction),
                _ => result
            };
        }
        
        int totalCount = result.Count();
        
        result = result.Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize);
        
        List<ShipmentKindItem> shipmentKindItems = result.Select(x => new ShipmentKindItem
            {
                ShipmentKindId = x.ShipmentKindId.Value,
                Name = x.Name,
                ShortCode = x.ShortCode,
                Direction = x.Direction.ToString()
            })
            .ToList();
        
        return new ListShipmentKindsResponse
        {
            Items = shipmentKindItems,
            TotalCount = totalCount
        };
    }
}
