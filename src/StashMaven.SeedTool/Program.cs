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

public class Option
{
    public string? Key { get; set; }
    public string? Value { get; set; }
}

public class OptionsCollection
{
    public List<Option> Company { get; set; } = [];
    public List<Option> StashMaven { get; set; } = [];
}

class Program
{
    static async Task Main()
    {
        StashMavenClient client = await StashMavenClientFactory.Create();

        string optionsJson = await File.ReadAllTextAsync("options.json");

        OptionsCollection options = JsonSerializer.Deserialize<OptionsCollection>(optionsJson,
                                        new JsonSerializerOptions
                                        {
                                            PropertyNameCaseInsensitive = true
                                        })
                                    ?? throw new InvalidOperationException(
                                        "Unable to deserialize options");

        foreach (Option option in options.Company)
        {
            await client.UpsertOptionAsync(option.Key!, option.Value!, "Company");
        }

        foreach (Option option in options.StashMaven)
        {
            await client.UpsertOptionAsync(option.Key!, option.Value!, "StashMaven");
        }

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
            await client.AddShipmentKindAsync(shipmentKind.Name!, shipmentKind.ShortCode!, shipmentKind.Direction!);
        }

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
            taxDefinition.TaxDefinitionId = await client.AddTaxDefinitionAsync(taxDefinition.Name!, taxDefinition.Rate);
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
            string catalogItemId = await client.AddCatalogItemAsync(
                catalogItem.Sku!,
                catalogItem.Name!,
                units[random.Next(0, 3)],
                taxDefinitions[random.Next(0, taxDefinitions.Count)].TaxDefinitionId!,
                taxDefinitions[random.Next(0, taxDefinitions.Count)].TaxDefinitionId!);
            catalogItemIds.Add(catalogItemId);
        }

        string stockpileId = await client.AddStockpileAsync("Default", "M1");
        foreach (string catalogItemId in catalogItemIds)
        {
            await client.AddInventoryItemAsync(catalogItemId, stockpileId);
        }
    }
}
