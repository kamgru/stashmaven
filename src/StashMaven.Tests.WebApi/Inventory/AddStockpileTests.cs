using StashMaven.WebApi.Features.Inventory.Stockpiles;

namespace StashMaven.Tests.WebApi.Inventory;

public class AddStockpileTests(DefaultTestFixture fixture) : IClassFixture<DefaultTestFixture>
{
    [Fact]
    public async Task WhenRequestValid_ShouldAddStockpile()
    {
        HttpClient client = fixture.CreateClient();

        HttpResponseMessage result = await client.PostAsJsonAsync("api/v1/stockpile",
            new AddStockpileHandler.AddStockpileRequest("Test Stockpile", Guid.NewGuid().ToString()[..Stockpile.ShortCodeMaxLength], false));

        result.EnsureSuccessStatusCode();

        AddStockpileHandler.AddStockpileResponse? stockpileId =
            await result.Content.ReadFromJsonAsync<AddStockpileHandler.AddStockpileResponse>();

        Assert.NotNull(stockpileId);

        await using StashMavenContext context = fixture.CreateDbContext();

        Stockpile? stockpile = await context.Stockpiles
            .FirstOrDefaultAsync(x => x.StockpileId.Value == stockpileId.StockpileId);

        Assert.NotNull(stockpile);
        Assert.Equal("Test Stockpile", stockpile.Name);
    }
}
