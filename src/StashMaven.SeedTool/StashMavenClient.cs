using System.Net.Http.Json;

namespace StashMaven.SeedTool;

public class StashMavenClient(HttpClient client)
{
    public class AddTaxDefinitionResponse
    {
        public required string TaxDefinitionId { get; set; }
    }

    public async Task<string> AddTaxDefinition(
        string name,
        decimal rate)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync("taxdefinition", new
        {
            name,
            rate
        });

        response.EnsureSuccessStatusCode();

        AddTaxDefinitionResponse? result = await response.Content.ReadFromJsonAsync<AddTaxDefinitionResponse>();
        return result?.TaxDefinitionId
               ?? throw new InvalidOperationException("Unable to deserialize tax definition");
    }

    public async Task<string> AddCatalogItem(
        string sku,
        string name,
        string unitOfMeasure,
        string taxDefinitionId)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync("catalogitem", new
        {
            sku,
            name,
            unitOfMeasure,
            taxDefinitionId
        });

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }
}
