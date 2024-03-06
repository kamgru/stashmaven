using StashMaven.WebApi.Data.Services;

namespace StashMaven.WebApi.Features.Common.TaxDefinitions;

public partial class TaxDefinitionController
{
    [HttpGet]
    [Route("list")]
    public async Task<IActionResult> ListTaxDefinitionsAsync(
        [FromServices]
        ListTaxDefinitionsHandler handler)
    {
        ListTaxDefinitionsHandler.ListTaxDefinitionsResponse response =
            await handler.ListTaxDefinitionsAsync();
        return Ok(response);
    }
}

[Injectable]
public class ListTaxDefinitionsHandler(CacheReader cacheReader)
{
    public class ListTaxDefinitionsResponse
    {
        public List<TaxDefinitionItem> Items { get; set; } = [];
    }

    public class TaxDefinitionItem
    {
        public required string TaxDefinitionId { get; set; }
        public required string Name { get; set; }
        public required decimal Rate { get; set; }
        public required string CountryCode { get; set; }
    }

    public async Task<ListTaxDefinitionsResponse> ListTaxDefinitionsAsync()
    {
        IReadOnlyList<TaxDefinition> taxDefinitions = await cacheReader.GetTaxDefinitionsAsync();

        return new ListTaxDefinitionsResponse
        {
            Items = taxDefinitions
                .Select(x => new TaxDefinitionItem
                {
                    TaxDefinitionId = x.TaxDefinitionId.Value,
                    Name = x.Name,
                    Rate = x.Rate,
                    CountryCode = x.CountryCode
                })
                .ToList(),
        };
    }
}
