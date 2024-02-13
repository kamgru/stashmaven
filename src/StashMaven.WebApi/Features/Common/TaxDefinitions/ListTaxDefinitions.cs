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
public class ListTaxDefinitionsHandler(
    StashMavenContext context)
{
    public class ListTaxDefinitionsRequest
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
    }

    public class ListTaxDefinitionsResponse
    {
        public List<TaxDefinitionItem> Items { get; set; } = new();
        public int TotalCount { get; set; }
    }

    public class TaxDefinitionItem
    {
        public required string TaxDefinitionId { get; set; }
        public required string Name { get; set; }
        public required decimal Rate { get; set; }
    }

    public async Task<ListTaxDefinitionsResponse> ListTaxDefinitionsAsync(
        ListTaxDefinitionsRequest request)
    {
        int page = Math.Max(1, request.Page);
        int pageSize = Math.Clamp(request.PageSize, 10, 100);

        List<TaxDefinitionItem> taxDefinitions = await context.TaxDefinitions
            .Select(x => new TaxDefinitionItem
            {
                Name = x.Name,
                Rate = x.Rate,
                TaxDefinitionId = x.TaxDefinitionId.Value
            })
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        int totalCount = await context.TaxDefinitions.CountAsync();

        return new ListTaxDefinitionsResponse
        {
            Items = taxDefinitions,
            TotalCount = totalCount
        };
    }
}
