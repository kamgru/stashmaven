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

public class AcceptShipmentTests(
    AcceptShipmentTestFixture fixture,
    ITestOutputHelper testOutputHelper) : IClassFixture<AcceptShipmentTestFixture>
{
    [Fact]
    public async Task WhenConcurrentShipmentsUpdateItemQuantity_QuantityShouldBeCorrect()
    {
        const int numberOfConcurrentRequests = 20;
        const string inventoryItemId = "concurrent-inventory-item-1";
        const string shipmentIdPrefix = "concurrent-shipment-";

        async Task AcceptShipmentTask(
            int index)
        {
            string shipmentId = $"{shipmentIdPrefix}{index}";
            HttpClient client = fixture.CreateClient();
            HttpResponseMessage res =
                await client.PatchAsJsonAsync($"api/v1/inventory/shipment/{shipmentId}/accept", new { });

            if (!res.IsSuccessStatusCode)
            {
                testOutputHelper.WriteLine(await res.Content.ReadAsStringAsync());
            }
        }

        await using StashMavenContext context = fixture.CreateDbContext();
        context.InventoryItems.Add(new InventoryItem
        {
            InventoryItemId = new InventoryItemId(inventoryItemId),
            Sku = "sku-1",
            Name = "name-1",
            Quantity = 0,
            UnitOfMeasure = UnitOfMeasure.Pc,
            UnitPrice = 1,
            TaxDefinitionId = new TaxDefinitionId("tax-definition-1"),
        });

        Random random = new();
        for (int i = 0; i < 20; i++)
        {
            context.Shipments.Add(new Shipment
            {
                ShipmentId = new ShipmentId($"{shipmentIdPrefix}{i}"),
                SupplierId = new SupplierId("supplier-1"),
                ShipmentDirection =
                    random.Next(0, 100) >= 50 ? ShipmentDirection.In : ShipmentDirection.Out,
                ShipmentAcceptance = ShipmentAcceptance.Pending,
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow,
                Currency = Currency.Pln,
                ShipmentRecords =
                [
                    new ShipmentRecord
                    {
                        InventoryItemId = new InventoryItemId(inventoryItemId),
                        Quantity = random.Next(1, 50),
                        UnitOfMeasure = UnitOfMeasure.Pc,
                        UnitPrice = 1,
                    },
                    new ShipmentRecord
                    {
                        InventoryItemId = new InventoryItemId(inventoryItemId),
                        Quantity = random.Next(1, 50),
                        UnitOfMeasure = UnitOfMeasure.Pc,
                        UnitPrice = 1,
                    }
                ]
            });
        }

        await context.SaveChangesAsync();

        List<Task> tasks = Enumerable.Range(0, numberOfConcurrentRequests)
            .Select(AcceptShipmentTask)
            .ToList();

        await Task.WhenAll(tasks);

        await using StashMavenContext assertContext = fixture.CreateDbContext();
        decimal expectedQuantity = 0;
        var shipmentRecords = assertContext.Shipments
            .Where(x =>x.ShipmentId.Value.StartsWith(shipmentIdPrefix))
            .Include(x => x.ShipmentRecords)
            .Select(x => new { x.ShipmentDirection, Quantity = x.ShipmentRecords.Sum(c => c.Quantity) })
            .ToList();

        expectedQuantity += shipmentRecords.Sum(x => x.Quantity * (int)x.ShipmentDirection);

        InventoryItem? inventoryItem = await assertContext.InventoryItems
            .SingleOrDefaultAsync(c => c.InventoryItemId.Value == inventoryItemId);

        Assert.Equal(expectedQuantity, inventoryItem!.Quantity);
    }

    [Fact]
    public async Task WhenRequestValid_ShipmentShouldBeAccepted()
    {
        await using StashMavenContext context = fixture.CreateDbContext();
        context.InventoryItems.Add(new InventoryItem
        {
            InventoryItemId = new InventoryItemId("inventory-item-3"),
            Sku = "sku-1",
            Name = "name-1",
            Quantity = 0,
            UnitOfMeasure = UnitOfMeasure.Pc,
            UnitPrice = 1,
            TaxDefinitionId = new TaxDefinitionId("tax-definition-1"),
        });
        context.Shipments.Add(new Shipment
        {
            ShipmentId = new ShipmentId("test-accept-shipment-1"),
            SupplierId = new SupplierId("supplier-1"),
            ShipmentDirection = ShipmentDirection.In,
            ShipmentAcceptance = ShipmentAcceptance.Pending,
            ShipmentRecords =
            [
                new ShipmentRecord
                {
                    InventoryItemId = new InventoryItemId("inventory-item-3"),
                    Quantity = 1,
                    UnitOfMeasure = UnitOfMeasure.Pc,
                    UnitPrice = 1,
                }
            ]
        });
        await context.SaveChangesAsync();

        HttpClient client = fixture.CreateClient();
        HttpResponseMessage res =
            await client.PatchAsJsonAsync("api/v1/inventory/shipment/test-accept-shipment-1/accept", new { });

        res.EnsureSuccessStatusCode();

        await using StashMavenContext assertContext = fixture.CreateDbContext();
        Shipment? shipment = await assertContext.Shipments
            .SingleOrDefaultAsync(s => s.ShipmentId.Value == "test-accept-shipment-1");

        Assert.Equal(ShipmentAcceptance.Accepted, shipment!.ShipmentAcceptance);
    }

    [Fact]
    public async Task GivenMultipleRecordsForSingleItem_WhenShipmentAccepted_QuantityShouldBeCorrect()
    {
        await using StashMavenContext context = fixture.CreateDbContext();
        context.InventoryItems.Add(new InventoryItem
        {
            InventoryItemId = new InventoryItemId("inventory-item-2"),
            Sku = "sku-2",
            Name = "name-2",
            Quantity = 0,
            UnitOfMeasure = UnitOfMeasure.Pc,
            UnitPrice = 1,
            TaxDefinitionId = new TaxDefinitionId("tax-definition-1"),
        });
        context.Shipments.Add(new Shipment
        {
            ShipmentId = new ShipmentId("test-accept-shipment-2"),
            SupplierId = new SupplierId("supplier-1"),
            ShipmentDirection = ShipmentDirection.In,
            ShipmentAcceptance = ShipmentAcceptance.Pending,
            ShipmentRecords =
            {
                new ShipmentRecord
                {
                    InventoryItemId = new InventoryItemId("inventory-item-2"),
                    Quantity = 1,
                    UnitOfMeasure = UnitOfMeasure.Pc,
                    UnitPrice = 1,
                },
                new ShipmentRecord
                {
                    InventoryItemId = new InventoryItemId("inventory-item-2"),
                    Quantity = 1,
                    UnitOfMeasure = UnitOfMeasure.Pc,
                    UnitPrice = 1,
                }
            }
        });
        await context.SaveChangesAsync();

        HttpClient client = fixture.CreateClient();
        HttpResponseMessage res =
            await client.PatchAsJsonAsync("api/v1/inventory/shipment/test-accept-shipment-2/accept", new { });

        res.EnsureSuccessStatusCode();

        await using StashMavenContext assertContext = fixture.CreateDbContext();
        InventoryItem? inventoryItem = await assertContext.InventoryItems
            .SingleOrDefaultAsync(c => c.InventoryItemId.Value == "inventory-item-2");

        Assert.Equal(2, inventoryItem!.Quantity);
    }
}
