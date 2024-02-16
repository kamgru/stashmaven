using StashMaven.WebApi.Features.Inventory.InventoryItems;

namespace StashMaven.Tests.WebApi.Inventory;

public class AcceptShipmentTestFixture : TestFixture
{
    public AcceptShipmentTestFixture()
        : base("accept-shipment-tests")
    {
        using StashMavenContext context = CreateDbContext();
        context.Database.EnsureCreated();
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
    }
}

public class AcceptShipmentHandlerTests(
    AcceptShipmentTestFixture fixture,
    ITestOutputHelper testOutputHelper) : IClassFixture<AcceptShipmentTestFixture>
{
    [Fact]
    public async Task WhenConcurrentShipmentsUpdateItemQuantity_QuantityShouldBeCorrect()
    {
        string stockpileId = await fixture.AddStockpile();
        string inShipmentKindId = await fixture.AddShipmentKind(ShipmentDirection.In);
        string outShipmentKindId = await fixture.AddShipmentKind(ShipmentDirection.Out);
        string taxDefinitionId = await fixture.AddTaxDefinition();
        string partnerId = await fixture.AddPartner();
        string catalogItem1Id = await fixture.AddCatalogItem(taxDefinitionId);
        string catalogItem2Id = await fixture.AddCatalogItem(taxDefinitionId);
        string inventoryItemId1 = await fixture.AddInventoryItem(catalogItem1Id, stockpileId);
        string inventoryItemId2 = await fixture.AddInventoryItem(catalogItem2Id, stockpileId);

        const int numberOfConcurrentRequests = 20;
        List<string> shipmentIds = [];

        for (int i = 0; i < numberOfConcurrentRequests; i++)
        {
            string shipmentId = i % 2 == 0
                ? await fixture.AddShipment(stockpileId, inShipmentKindId)
                : await fixture.AddShipment(stockpileId, outShipmentKindId);

            shipmentIds.Add(shipmentId);

            await fixture.ChangeShipmentPartner(shipmentId, partnerId);
            await fixture.AddRecordToShipment(shipmentId, inventoryItemId1, i, 1);
            await fixture.AddRecordToShipment(shipmentId, inventoryItemId2, 2 * i, 1);
        }

        List<Task> tasks = shipmentIds
            .Select(fixture.AcceptShipment)
            .ToList();

        await Task.WhenAll(tasks);

        using HttpClient client = fixture.CreateClient();
        GetInventoryItemByIdHandler.GetInventoryItemByIdResponse? inventoryItem1Response =
            await client.GetFromJsonAsync<GetInventoryItemByIdHandler.GetInventoryItemByIdResponse>(
                $"api/v1/InventoryItem/{inventoryItemId1}");
        GetInventoryItemByIdHandler.GetInventoryItemByIdResponse? inventoryItem2Response =
            await client.GetFromJsonAsync<GetInventoryItemByIdHandler.GetInventoryItemByIdResponse>(
                $"api/v1/InventoryItem/{inventoryItemId2}");

        Assert.NotNull(inventoryItem1Response);
        Assert.NotNull(inventoryItem2Response);
        Assert.Equal(numberOfConcurrentRequests / 2 * -1, inventoryItem1Response.Quantity);
        Assert.Equal(numberOfConcurrentRequests * -1, inventoryItem2Response.Quantity);
    }

    [Fact]
    public async Task WhenRequestValid_ShipmentShouldBeAccepted()
    {
        string stockpileId = await fixture.AddStockpile();
        string shipmentKindId = await fixture.AddShipmentKind();
        string taxDefinitionId = await fixture.AddTaxDefinition();
        string catalogItemId = await fixture.AddCatalogItem(taxDefinitionId);
        string inventoryItemId = await fixture.AddInventoryItem(catalogItemId, stockpileId);
        string partnerId = await fixture.AddPartner();
        string shipmentId = await fixture.AddShipment(stockpileId, shipmentKindId);
        await fixture.ChangeShipmentPartner(shipmentId, partnerId);
        await fixture.AddRecordToShipment(shipmentId, inventoryItemId, 1, 1);

        HttpClient client = fixture.CreateClient();
        HttpResponseMessage responseMessage =
            await client.PostAsJsonAsync($"api/v1/shipment/{shipmentId}/accept", new { });

        responseMessage.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task GivenMultipleRecordsForSingleItem_WhenShipmentAccepted_QuantityShouldBeCorrect()
    {
        string stockpileId = await fixture.AddStockpile();
        string shipmentKindId = await fixture.AddShipmentKind();
        string taxDefinitionId = await fixture.AddTaxDefinition();
        string catalogItemId = await fixture.AddCatalogItem(taxDefinitionId);
        string inventoryItemId = await fixture.AddInventoryItem(catalogItemId, stockpileId);
        string partnerId = await fixture.AddPartner();
        string shipmentId = await fixture.AddShipment(stockpileId, shipmentKindId);
        await fixture.ChangeShipmentPartner(shipmentId, partnerId);
        await fixture.AddRecordToShipment(shipmentId, inventoryItemId, 1, 1);
        await fixture.AddRecordToShipment(shipmentId, inventoryItemId, 1, 1);
        await fixture.AddRecordToShipment(shipmentId, inventoryItemId, 1, 1);
        await fixture.AcceptShipment(shipmentId);

        using HttpClient client = fixture.CreateClient();
        GetInventoryItemByIdHandler.GetInventoryItemByIdResponse? response =
            await client.GetFromJsonAsync<GetInventoryItemByIdHandler.GetInventoryItemByIdResponse>(
                $"api/v1/InventoryItem/{inventoryItemId}");

        Assert.NotNull(response);
        Assert.Equal(3, response.Quantity);
    }
}
