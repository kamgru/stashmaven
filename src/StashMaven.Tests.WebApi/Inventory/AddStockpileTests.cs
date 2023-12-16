using StashMaven.WebApi.Features.Inventory.Stockpiles;

namespace StashMaven.Tests.WebApi.Inventory;

public class AddStockpileTests(DefaultTestFixture fixture) : IClassFixture<DefaultTestFixture>
{
    [Fact]
    public async Task WhenRequestValid_ShouldAddStockpile()
    {
        HttpClient client = fixture.CreateClient();

        HttpResponseMessage result = await client.PostAsJsonAsync("api/v1/stockpile",
            new AddStockpileHandler.AddStockpileRequest
            {
                Name = "Test Stockpile",
                ShortCode = Guid.NewGuid().ToString()[..Stockpile.ShortCodeMaxLength]
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
