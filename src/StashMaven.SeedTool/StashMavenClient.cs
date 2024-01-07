using System.Net.Http.Json;

namespace StashMaven.SeedTool;

public class StashMavenClient(HttpClient client)
{
    public class AddTaxDefinitionResponse
    {
        public required string TaxDefinitionId { get; set; }
    }

    public async Task<string> AddTaxDefinitionAsync(
        string name,
        decimal rate)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync("api/v1/TaxDefinition", new
        {
            name,
            rate
        });

        response.EnsureSuccessStatusCode();

        AddTaxDefinitionResponse? result = await response.Content.ReadFromJsonAsync<AddTaxDefinitionResponse>();
        return result?.TaxDefinitionId
               ?? throw new InvalidOperationException("Unable to deserialize tax definition");
    }

    public class AddStockpileResponse
    {
        public required string StockpileId { get; set; }
    }

    public async Task<string> AddStockpileAsync(
        string name,
        string shortCode)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync("api/v1/Stockpile", new
        {
            name,
            shortCode
        });

        response.EnsureSuccessStatusCode();

        AddStockpileResponse? result = await response.Content.ReadFromJsonAsync<AddStockpileResponse>();
        return result?.StockpileId
               ?? throw new InvalidOperationException("Unable to deserialize stockpile");
    }

    public class AddInventoryItemResponse
    {
        public required string InventoryItemId { get; set; }
    }

    public async Task<string> AddInventoryItemAsync(
        string catalogItemId,
        string stockpileId)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync("api/v1/InventoryItem", new
        {
            catalogItemId,
            stockpileId
        });

        response.EnsureSuccessStatusCode();

        AddInventoryItemResponse? result = await response.Content.ReadFromJsonAsync<AddInventoryItemResponse>();
        return result?.InventoryItemId
               ?? throw new InvalidOperationException("Unable to deserialize inventory item");
    }

    public class AddCatalogItemResponse
    {
        public required string CatalogItemId { get; set; }
    }

    public async Task<string> AddCatalogItemAsync(
        string sku,
        string name,
        string unitOfMeasure,
        string buyTaxDefinitionId,
        string sellTaxDefinitionId)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync("api/v1/CatalogItem", new
        {
            sku,
            name,
            unitOfMeasure,
            buyTaxDefinitionId,
            sellTaxDefinitionId
        });

        response.EnsureSuccessStatusCode();

        AddCatalogItemResponse? result = await response.Content.ReadFromJsonAsync<AddCatalogItemResponse>();
        return result?.CatalogItemId
               ?? throw new InvalidOperationException("Unable to deserialize catalog item");
    }

    public class AddShipmentKindResponse
    {
        public required string ShipmentKindId { get; set; }
    }

    public async Task<string> AddShipmentKindAsync(
        string name,
        string shortCode,
        string direction)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync("api/v1/Shipment/shipment-kind", new
        {
            name,
            shortCode,
            direction
        });

        response.EnsureSuccessStatusCode();

        AddShipmentKindResponse? result = await response.Content.ReadFromJsonAsync<AddShipmentKindResponse>();
        return result?.ShipmentKindId
               ?? throw new InvalidOperationException("Unable to deserialize shipment kind");
    }

    public async Task UpsertOptionAsync(
        string key,
        string value,
        string type)
    {
        HttpResponseMessage response = await client.PutAsJsonAsync("api/v1/option", new
        {
            key,
            value,
            type
        });

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }
        response.EnsureSuccessStatusCode();
    }
}
