using StashMaven.WebApi.Features.Inventory;

namespace StashMaven.Tests.WebApi.Inventory;

public class CreateStockpileTests(DefaultTestFixture fixture) : IClassFixture<DefaultTestFixture>
{
    [Fact]
    public async Task WhenRequestValid_ShouldCreateStockpile()
    {
        HttpClient client = fixture.CreateClient();

        HttpResponseMessage result = await client.PostAsJsonAsync("api/v1/inventory/stockpile",
            new CreateStockpileHandler.CreateStockpileRequest
            {
                Name = "Test Stockpile",
                ShortCode = "TEST"
            });

        result.EnsureSuccessStatusCode();

        StockpileId? stockpileId = await result.Content.ReadFromJsonAsync<StockpileId>();

        Assert.NotNull(stockpileId);

        await using StashMavenContext context = fixture.CreateDbContext();

        Stockpile? stockpile = await context.Stockpiles
            .FirstOrDefaultAsync(x => x.StockpileId.Value == stockpileId.Value);

        Assert.NotNull(stockpile);
        Assert.Equal("Test Stockpile", stockpile.Name);
    }
}
