using System.Text.Json;

namespace StashMaven.SeedTool;

public class TaxDefinition
{
    public string? TaxDefinitionId { get; set; }
    public string? Name { get; set; }
    public decimal Rate { get; set; }
}

public class CatalogItem
{
    public string? Name { get; set; }
    public string? Sku { get; set; }
    public string? UnitOfMeasure { get; set; }
    public string? TaxDefinitionId { get; set; }
}


public class ShipmentKind
{
    public string? Name { get; set; }
    public string? ShortCode { get; set; }
    public string? Direction { get; set; }
}

class Program
{
    static async Task Main()
    {
        StashMavenClient client = await StashMavenClientFactory.Create();

        string shipmentKindsJson = await File.ReadAllTextAsync("shipment_kinds.json");

        List<ShipmentKind> shipmentKinds = JsonSerializer.Deserialize<List<ShipmentKind>>(shipmentKindsJson,
                                                 new JsonSerializerOptions
                                                 {
                                                     PropertyNameCaseInsensitive = true
                                                 })
                                             ?? throw new InvalidOperationException(
                                                 "Unable to deserialize shipment kinds");

        foreach (ShipmentKind shipmentKind in shipmentKinds)
        {
            await client.AddShipmentKind(shipmentKind.Name!, shipmentKind.ShortCode!, shipmentKind.Direction!);
        }

        return;

        string taxesJson = await File.ReadAllTextAsync("tax_definitions.json");

        List<TaxDefinition> taxDefinitions = JsonSerializer.Deserialize<List<TaxDefinition>>(taxesJson,
                                                 new JsonSerializerOptions
                                                 {
                                                     PropertyNameCaseInsensitive = true
                                                 })
                                             ?? throw new InvalidOperationException(
                                                 "Unable to deserialize tax definitions");

        foreach (TaxDefinition taxDefinition in taxDefinitions)
        {
            taxDefinition.TaxDefinitionId = await client.AddTaxDefinition(taxDefinition.Name!, taxDefinition.Rate);
        }

        string catalogItemsJson = await File.ReadAllTextAsync("fake_catalog_items.json");

        List<CatalogItem> catalogItems = JsonSerializer.Deserialize<List<CatalogItem>>(catalogItemsJson,
                                             new JsonSerializerOptions
                                             {
                                                 PropertyNameCaseInsensitive = true
                                             })
                                         ?? throw new InvalidOperationException(
                                             "Unable to deserialize catalog items");

        Random random = new();
        string[] units = ["Pc", "Kg", "L"];
        List<string> catalogItemIds = [];
        foreach (CatalogItem catalogItem in catalogItems)
        {
            string catalogItemId = await client.AddCatalogItem(catalogItem.Sku!, catalogItem.Name!, units[random.Next(0, 3)],
                taxDefinitions[random.Next(0, taxDefinitions.Count)].TaxDefinitionId!);
            catalogItemIds.Add(catalogItemId);
        }

        string stockpileId = await client.AddStockpile("Default", "DEF");
        foreach(string catalogItemId in catalogItemIds)
        {
            await client.AddInventoryItem(catalogItemId, stockpileId);
        }


    }
}
