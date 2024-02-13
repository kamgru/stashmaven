// using StashMaven.WebApi.Features.Catalog.CatalogItems;
// using StashMaven.WebApi.Features.Common.TaxDefinitions;
// using StashMaven.WebApi.Features.Inventory.InventoryItems;
// using StashMaven.WebApi.Features.Inventory.Stockpiles;
//
// namespace StashMaven.Tests.WebApi.Inventory;
//
// public class AcceptShipmentTestFixture : TestFixture
// {
//     public AcceptShipmentTestFixture()
//         : base("accept-shipment-tests")
//     {
//         using StashMavenContext context = CreateDbContext();
//         context.Database.EnsureCreated();
//         context.Database.EnsureDeleted();
//         context.Database.EnsureCreated();
//     }
// }
//
// public class AcceptShipmentTests(
//     AcceptShipmentTestFixture fixture,
//     ITestOutputHelper testOutputHelper) : IClassFixture<AcceptShipmentTestFixture>
// {
//     [Fact]
//     public async Task WhenConcurrentShipmentsUpdateItemQuantity_QuantityShouldBeCorrect()
//     {
//         const int numberOfConcurrentRequests = 20;
//         const string inventoryItemId1 = "concurrent-inventory-item-1";
//         const string inventoryItemId2 = "concurrent-inventory-item-2";
//         const string shipmentIdPrefix = "concurrent-shipment-";
//
//         async Task AcceptShipmentTask(
//             int index)
//         {
//             string shipmentId = $"{shipmentIdPrefix}{index}";
//             HttpClient client = fixture.CreateClient();
//             HttpResponseMessage res =
//                 await client.PatchAsJsonAsync($"api/v1/inventory/shipment/{shipmentId}/accept", new { });
//
//             if (!res.IsSuccessStatusCode)
//             {
//                 testOutputHelper.WriteLine(await res.Content.ReadAsStringAsync());
//             }
//         }
//
//         await using StashMavenContext context = fixture.CreateDbContext();
//         Stockpile stockpile = new()
//         {
//             StockpileId = new StockpileId("stockpile-1"),
//             Name = "Stockpile 1",
//             ShortCode = Guid.NewGuid().ToString().Substring(0, Stockpile.ShortCodeMaxLength)
//         };
//         context.Stockpiles.Add(stockpile);
//         InventoryItem inventoryItem1 = new()
//         {
//             InventoryItemId = new InventoryItemId(inventoryItemId1),
//             Stockpile = stockpile,
//             Sku = "sku-1",
//             Name = "name-1",
//             Quantity = 0,
//             UnitOfMeasure = UnitOfMeasure.Pc,
//             LastPurchasePrice = 1,
//             TaxDefinitionId = new TaxDefinitionId("tax-definition-1"),
//         };
//         InventoryItem inventoryItem2 = new()
//         {
//             InventoryItemId = new InventoryItemId(inventoryItemId2),
//             Stockpile = stockpile,
//             Sku = "sku-2",
//             Name = "name-2",
//             Quantity = 0,
//             UnitOfMeasure = UnitOfMeasure.Pc,
//             LastPurchasePrice = 1,
//             TaxDefinitionId = new TaxDefinitionId("tax-definition-1"),
//         };
//         context.InventoryItems.AddRange([
//             inventoryItem1,
//             inventoryItem2
//         ]);
//         SequenceGenerator sequenceGenerator1 = new()
//         {
//             SequenceGeneratorId = new SequenceGeneratorId("sequence-generator-1"),
//             Entries =
//             [
//                 new SequenceEntry
//                 {
//                     Group = "SK1",
//                     Delimiter = stockpile.ShortCode,
//                     NextValue = 1,
//                     Version = 0
//                 }
//             ]
//         };
//         SequenceGenerator sequenceGenerator2 = new()
//         {
//             SequenceGeneratorId = new SequenceGeneratorId("sequence-generator-2"),
//             Entries =
//             [
//                 new SequenceEntry
//                 {
//                     Group = "SK2",
//                     Delimiter = stockpile.ShortCode,
//                     NextValue = 1,
//                     Version = 0
//                 }
//             ]
//         };
//         context.SequenceGenerators.AddRange([
//             sequenceGenerator1,
//             sequenceGenerator2
//         ]);
//
//         ShipmentKind inShipment = new()
//         {
//             ShipmentKindId = new ShipmentKindId("shipment-kind-1"),
//             SequenceGeneratorId = sequenceGenerator1.SequenceGeneratorId,
//             Name = "Shipment Kind 1",
//             ShortCode = "SK1",
//             Direction = ShipmentDirection.In
//         };
//         ShipmentKind outShipment = new()
//         {
//             ShipmentKindId = new ShipmentKindId("shipment-kind-2"),
//             SequenceGeneratorId = sequenceGenerator2.SequenceGeneratorId,
//             Name = "Shipment Kind 2",
//             ShortCode = "SK2",
//             Direction = ShipmentDirection.Out
//         };
//         context.ShipmentKinds.AddRange([
//             inShipment,
//             outShipment
//         ]);
//         Partner partner = new()
//         {
//             Address = new Address
//             {
//                 City = "City",
//                 PostalCode = "PostalCode",
//                 Street = "Street",
//                 CountryCode = "PL"
//             },
//             PartnerId = new PartnerId("partner-1"),
//             LegalName = "LegalName",
//             CustomIdentifier = "CustomIdentifier",
//         };
//         context.Partners.Add(partner);
//         Random random = new();
//         for (int i = 0; i < 20; i++)
//         {
//             context.Shipments.Add(new Shipment
//             {
//                 ShipmentId = new ShipmentId($"{shipmentIdPrefix}{i}"),
//                 PartnerReference = new ShipmentPartnerReference
//                 {
//                     Address = "",
//                     LegalName = ""
//                 },
//                 Partner = partner,
//                 Stockpile = stockpile,
//                 Kind = random.Next(0, 100) >= 50 ? inShipment : outShipment,
//                 Acceptance = ShipmentAcceptance.Pending,
//                 CreatedOn = DateTime.UtcNow,
//                 UpdatedOn = DateTime.UtcNow,
//                 Currency = Currency.Pln,
//                 Records =
//                 [
//                     new ShipmentRecord
//                     {
//                         InventoryItem = inventoryItem1,
//                         Quantity = random.Next(1, 50),
//                         UnitOfMeasure = UnitOfMeasure.Pc,
//                         UnitPrice = 1,
//                     },
//                     new ShipmentRecord
//                     {
//                         InventoryItem = inventoryItem1,
//                         Quantity = random.Next(1, 50),
//                         UnitOfMeasure = UnitOfMeasure.Pc,
//                         UnitPrice = 1,
//                     },
//                     new ShipmentRecord
//                     {
//                         InventoryItem = inventoryItem2,
//                         Quantity = random.Next(1, 50),
//                         UnitOfMeasure = UnitOfMeasure.Pc,
//                         UnitPrice = 1,
//                     },
//                     new ShipmentRecord
//                     {
//                         InventoryItem = inventoryItem2,
//                         Quantity = random.Next(1, 50),
//                         UnitOfMeasure = UnitOfMeasure.Pc,
//                         UnitPrice = 1,
//                     }
//                 ]
//             });
//         }
//
//         await context.SaveChangesAsync();
//
//         List<Task> tasks = Enumerable.Range(0, numberOfConcurrentRequests)
//             .Select(AcceptShipmentTask)
//             .ToList();
//
//         await Task.WhenAll(tasks);
//
//         await using StashMavenContext assertContext = fixture.CreateDbContext();
//         decimal expectedQuantity1 = 0;
//
//         expectedQuantity1 += assertContext.ShipmentRecords
//             .Include(x => x.Shipment)
//             .ThenInclude(x => x.Kind)
//             .Where(x => x.InventoryItem.InventoryItemId.Value == inventoryItemId1)
//             .Sum(x => x.Quantity * (int)x.Shipment.Kind.Direction);
//
//         InventoryItem? expectedItem1 = await assertContext.InventoryItems
//             .SingleOrDefaultAsync(c => c.InventoryItemId.Value == inventoryItemId1);
//
//         decimal expectedQuantity2 = 0;
//         expectedQuantity2 += assertContext.ShipmentRecords
//             .Include(x => x.Shipment)
//             .ThenInclude(x => x.Kind)
//             .Where(x => x.InventoryItem.InventoryItemId.Value == inventoryItemId2)
//             .Sum(x => x.Quantity * (int)x.Shipment.Kind.Direction);
//
//         InventoryItem? expectedItem2 = await assertContext.InventoryItems
//             .SingleOrDefaultAsync(c => c.InventoryItemId.Value == inventoryItemId2);
//
//         Assert.Equal(expectedQuantity1, expectedItem1!.Quantity);
//         Assert.Equal(expectedQuantity2, expectedItem2!.Quantity);
//     }
//
//     [Fact]
//     public async Task WhenRequestValid_ShipmentShouldBeAccepted()
//     {
//         await using StashMavenContext context = fixture.CreateDbContext();
//         Stockpile stockpile = new()
//         {
//             StockpileId = new StockpileId("stockpile-1"),
//             Name = "Stockpile 1",
//             ShortCode = "ST1"
//         };
//         context.Stockpiles.Add(stockpile);
//         InventoryItem inventoryItem = new()
//         {
//             InventoryItemId = new InventoryItemId("inventory-item-3"),
//             Stockpile = stockpile,
//             Sku = "sku-1",
//             Name = "name-1",
//             Quantity = 0,
//             UnitOfMeasure = UnitOfMeasure.Pc,
//             LastPurchasePrice = 1,
//             TaxDefinitionId = new TaxDefinitionId("tax-definition-1"),
//         };
//         context.InventoryItems.Add(inventoryItem);
//         SequenceGenerator sequenceGenerator = new()
//         {
//             SequenceGeneratorId = new SequenceGeneratorId("sequence-generator-3"),
//             Entries =
//             [
//                 new SequenceEntry
//                 {
//                     Group = "SK3",
//                     Delimiter = stockpile.ShortCode,
//                     NextValue = 1,
//                     Version = 0
//                 }
//             ]
//         };
//         Partner partner = new()
//         {
//             Address = new Address
//             {
//                 City = "City",
//                 PostalCode = "PostalCode",
//                 Street = "Street",
//                 CountryCode = "PL"
//             },
//             PartnerId = new PartnerId("partner-2"),
//             LegalName = "LegalName",
//             CustomIdentifier = "CustomIdentifier2",
//         };
//         context.Partners.Add(partner);
//         context.SequenceGenerators.Add(sequenceGenerator);
//         ShipmentKind inShipment = new()
//         {
//             ShipmentKindId = new ShipmentKindId("shipment-kind-3"),
//             SequenceGeneratorId = sequenceGenerator.SequenceGeneratorId,
//             Name = "Shipment Kind 3",
//             ShortCode = "SK3",
//             Direction = ShipmentDirection.In
//         };
//         context.ShipmentKinds.Add(inShipment);
//         context.Shipments.Add(new Shipment
//         {
//             ShipmentId = new ShipmentId("test-accept-shipment-1"),
//             Partner = partner,
//             PartnerReference = new ShipmentPartnerReference
//             {
//                 Address = "",
//                 LegalName = ""
//             },
//             Stockpile = stockpile,
//             Kind = inShipment,
//             Acceptance = ShipmentAcceptance.Pending,
//             Records =
//             [
//                 new ShipmentRecord
//                 {
//                     InventoryItem = inventoryItem,
//                     Quantity = 1,
//                     UnitOfMeasure = UnitOfMeasure.Pc,
//                     UnitPrice = 1,
//                 }
//             ]
//         });
//         await context.SaveChangesAsync();
//
//         HttpClient client = fixture.CreateClient();
//         HttpResponseMessage res =
//             await client.PatchAsJsonAsync("api/v1/inventory/shipment/test-accept-shipment-1/accept", new { });
//
//         res.EnsureSuccessStatusCode();
//
//         await using StashMavenContext assertContext = fixture.CreateDbContext();
//         Shipment? shipment = await assertContext.Shipments
//             .SingleOrDefaultAsync(s => s.ShipmentId.Value == "test-accept-shipment-1");
//
//         Assert.Equal(ShipmentAcceptance.Accepted, shipment!.Acceptance);
//     }
//
//     [Fact]
//     public async Task GivenMultipleRecordsForSingleItem_WhenShipmentAccepted_QuantityShouldBeCorrect2()
//     {
//         HttpClient client = fixture.CreateClient();
//         HttpResponseMessage addStockpileRes = await client.PostAsJsonAsync("api/v1/stockpile",
//             new
//             {
//                 Name = Guid.NewGuid().ToString(),
//                 ShortCode = Guid.NewGuid().ToString().Substring(0, Stockpile.ShortCodeMaxLength)
//             });
//         addStockpileRes.EnsureSuccessStatusCode();
//         AddStockpileHandler.AddStockpileResponse? addStockpileResponse =
//             await addStockpileRes.Content.ReadFromJsonAsync<AddStockpileHandler.AddStockpileResponse>();
//         Assert.NotNull(addStockpileResponse);
//
//         HttpResponseMessage addTaxDefinitionRes = await client.PostAsJsonAsync("api/v1/taxdefinition",
//             new
//             {
//                 Name = Guid.NewGuid().ToString(),
//                 Rate = 0.23m
//             });
//         addTaxDefinitionRes.EnsureSuccessStatusCode();
//         AddTaxDefinitionHandler.AddTaxDefinitionResponse? addTaxDefinitionResponse =
//             await addTaxDefinitionRes.Content.ReadFromJsonAsync<AddTaxDefinitionHandler.AddTaxDefinitionResponse>();
//         Assert.NotNull(addTaxDefinitionRes);
//
//         HttpResponseMessage addCatalogItemRes = await client.PostAsJsonAsync("api/v1/catalogitem",
//             new
//             {
//                 Name = Guid.NewGuid().ToString(),
//                 Sku = Guid.NewGuid().ToString(),
//                 addTaxDefinitionResponse!.TaxDefinitionId,
//                 UnitOfMeasure = UnitOfMeasure.Pc,
//             });
//         addCatalogItemRes.EnsureSuccessStatusCode();
//         AddCatalogItemHandler.AddCatalogItemResponse? addCatalogItemResponse =
//             await addCatalogItemRes.Content.ReadFromJsonAsync<AddCatalogItemHandler.AddCatalogItemResponse>();
//         Assert.NotNull(addCatalogItemResponse);
//
//         HttpResponseMessage addInventoryItemRes = await client.PostAsJsonAsync("api/v1/inventoryitem",
//             new
//             {
//                 addCatalogItemResponse.CatalogItemId, addStockpileResponse.StockpileId
//             });
//         addInventoryItemRes.EnsureSuccessStatusCode();
//         AddInventoryItemHandler.AddInventoryItemResponse? addInventoryItemResponse =
//             await addInventoryItemRes.Content.ReadFromJsonAsync<AddInventoryItemHandler.AddInventoryItemResponse>();
//         Assert.NotNull(addInventoryItemResponse);
//     }
//
//     [Fact]
//     public async Task GivenMultipleRecordsForSingleItem_WhenShipmentAccepted_QuantityShouldBeCorrect()
//     {
//         await using StashMavenContext context = fixture.CreateDbContext();
//         Stockpile stockpile = new()
//         {
//             StockpileId = new StockpileId("stockpile-1"),
//             Name = "Stockpile 1",
//             ShortCode = Guid.NewGuid().ToString()[..Stockpile.ShortCodeMaxLength]
//         };
//         context.Stockpiles.Add(stockpile);
//         InventoryItem item1 = new()
//         {
//             InventoryItemId = new InventoryItemId(Guid.NewGuid().ToString()),
//             Stockpile = stockpile,
//             Sku = "sku-2",
//             Name = "name-2",
//             Quantity = 0,
//             UnitOfMeasure = UnitOfMeasure.Pc,
//             LastPurchasePrice = 1,
//             TaxDefinitionId = new TaxDefinitionId("tax-definition-1"),
//         };
//         InventoryItem item2 = new()
//         {
//             InventoryItemId = new InventoryItemId(Guid.NewGuid().ToString()),
//             Stockpile = stockpile,
//             Sku = "sku-3",
//             Name = "name-3",
//             Quantity = 0,
//             UnitOfMeasure = UnitOfMeasure.Pc,
//             LastPurchasePrice = 1,
//             TaxDefinitionId = new TaxDefinitionId("tax-definition-1"),
//         };
//         context.InventoryItems.AddRange([
//             item1,
//             item2
//         ]);
//         SequenceGenerator sequenceGenerator = new()
//         {
//             SequenceGeneratorId = new SequenceGeneratorId("sequence-generator-4"),
//             Entries =
//             [
//                 new SequenceEntry
//                 {
//                     Group = "SK4",
//                     Delimiter = stockpile.ShortCode,
//                     NextValue = 1,
//                     Version = 0
//                 }
//             ]
//         };
//         context.SequenceGenerators.Add(sequenceGenerator);
//         ShipmentKind inShipment = new()
//         {
//             ShipmentKindId = new ShipmentKindId("shipment-kind-4"),
//             SequenceGeneratorId = sequenceGenerator.SequenceGeneratorId,
//             Name = "Shipment Kind 4",
//             ShortCode = "SK4",
//             Direction = ShipmentDirection.In
//         };
//         Partner partner = new()
//         {
//             Address = new Address
//             {
//                 Street = "Street",
//                 PostalCode = "PostalCode",
//                 City = "City",
//                 CountryCode = "PL"
//             },
//             PartnerId = new PartnerId("partner-3"),
//             LegalName = "LegalName",
//             CustomIdentifier = "CustomIdentifier3",
//         };
//
//         context.Shipments.Add(new Shipment
//         {
//             ShipmentId = new ShipmentId("test-accept-shipment-2"),
//             Partner = partner,
//             PartnerReference = new ShipmentPartnerReference
//             {
//                 LegalName = "",
//                 Address = ""
//             },
//             Stockpile = stockpile,
//             Kind = inShipment,
//             Acceptance = ShipmentAcceptance.Pending,
//             Records =
//             {
//                 new ShipmentRecord
//                 {
//                     InventoryItem = item1,
//                     Quantity = 1,
//                     UnitOfMeasure = UnitOfMeasure.Pc,
//                     UnitPrice = 1,
//                 },
//                 new ShipmentRecord
//                 {
//                     InventoryItem = item1,
//                     Quantity = 2,
//                     UnitOfMeasure = UnitOfMeasure.Pc,
//                     UnitPrice = 1
//                 },
//                 new ShipmentRecord
//                 {
//                     InventoryItem = item2,
//                     Quantity = 3,
//                     UnitOfMeasure = UnitOfMeasure.Pc,
//                     UnitPrice = 1
//                 },
//                 new ShipmentRecord
//                 {
//                     InventoryItem = item2,
//                     Quantity = 4,
//                     UnitOfMeasure = UnitOfMeasure.Pc,
//                     UnitPrice = 1
//                 }
//             }
//         });
//         await context.SaveChangesAsync();
//
//         HttpClient client = fixture.CreateClient();
//         HttpResponseMessage res =
//             await client.PatchAsJsonAsync("api/v1/inventory/shipment/test-accept-shipment-2/accept", new { });
//
//         res.EnsureSuccessStatusCode();
//
//         await using StashMavenContext assertContext = fixture.CreateDbContext();
//         InventoryItem? expectedItem1 = await assertContext.InventoryItems
//             .SingleOrDefaultAsync(c => c.InventoryItemId.Value == item1.InventoryItemId.Value);
//         InventoryItem? expectedItem2 = await assertContext.InventoryItems
//             .SingleOrDefaultAsync(c => c.InventoryItemId.Value == item2.InventoryItemId.Value);
//
//         Assert.Equal(3, expectedItem1!.Quantity);
//         Assert.Equal(7, expectedItem2!.Quantity);
//     }
// }
