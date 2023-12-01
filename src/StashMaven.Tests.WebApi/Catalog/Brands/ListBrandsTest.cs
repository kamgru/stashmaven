using System.Text.Json;
using FluentAssertions;
using StashMaven.WebApi.Data;
using StashMaven.WebApi.Features.Catalog.Brands;

namespace StashMaven.Tests.WebApi.Catalog.Brands;

public class ListBrandsTest(
    ListBrandsTestFixture fixture) : IClassFixture<ListBrandsTestFixture>
{
    [Fact]
    public async Task TestPagination()
    {
        HttpClient client = fixture.CreateClient();

        const int page = 3;
        const int expectedCount = 8;
        HttpResponseMessage response =
            await client.GetAsync($"/api/v1/brand/list?page={page}&pageSize={expectedCount}");

        response.EnsureSuccessStatusCode();

        ListBrandsHandler.ListBrandsResponse? result =
            await response.Content.ReadFromJsonAsync<ListBrandsHandler.ListBrandsResponse>(
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

        Assert.NotNull(result);
        Assert.Equal(expectedCount, result.Brands.Count);
        Assert.Equal(ListBrandsTestFixture.BrandSeedCount, result.TotalCount);
    }

    [Fact]
    public async Task TestSorting()
    {
        HttpClient client = fixture.CreateClient();

        const int page = 1;
        const int expectedCount = 5;
        const bool isAscending = false;

        HttpResponseMessage response = await client.GetAsync(
            $"/api/v1/brand/list?page={page}&pageSize={expectedCount}&isAscending={isAscending}");

        response.EnsureSuccessStatusCode();

        ListBrandsHandler.ListBrandsResponse? result =
            await response.Content.ReadFromJsonAsync<ListBrandsHandler.ListBrandsResponse>(
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

        Assert.NotNull(result);

        List<ListBrandsTestFixture.FakeBrand> expectedBrands = fixture.FakeBrands
            .OrderByDescending(x => x.Name)
            .Take(expectedCount)
            .ToList();

        result.Brands.Should().BeEquivalentTo(expectedBrands);
    }

    [Fact]
    public async Task TestSearching()
    {
        HttpClient client = fixture.CreateClient();

        const int page = 1;
        const int expectedCount = 5;
        const string search = "qua";

        HttpResponseMessage response = await client.GetAsync(
            $"/api/v1/brand/list?page={page}&pageSize={expectedCount}&search={search}");

        response.EnsureSuccessStatusCode();

        ListBrandsHandler.ListBrandsResponse? result =
            await response.Content.ReadFromJsonAsync<ListBrandsHandler.ListBrandsResponse>(
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

        Assert.NotNull(result);

        List<ListBrandsTestFixture.FakeBrand> expectedBrands = fixture.FakeBrands
            .OrderBy(x => x.Name)
            .Where(x => x.Name.Contains(search))
            .Take(expectedCount)
            .ToList();

        result.Brands.Should().BeEquivalentTo(expectedBrands);
    }
}

public class ListBrandsTestFixture : TestFixture
{
    public class FakeBrand
    {
        public required string BrandId { get; set; }
        public required string Name { get; set; }
        public required string ShortCode { get; set; }
    }

    public IReadOnlyList<FakeBrand> FakeBrands { get; }

    public const int BrandSeedCount = 25;

    public ListBrandsTestFixture()
        : base("listbrands")
    {
        using StashMavenContext context = CreateDbContext();
        context.Brands.RemoveRange(context.Brands);

        string json = File.ReadAllText("Catalog/Brands/listBrandsData.json");
        FakeBrands = JsonSerializer.Deserialize<List<FakeBrand>>(json, new JsonSerializerOptions
                                 {
                                     PropertyNameCaseInsensitive = true
                                 })
                                 ?? throw new InvalidOperationException("Unable to deserialize brands.json");

        List<Brand> brands = FakeBrands.Select(x => new Brand
        {
            BrandId = new BrandId(x.BrandId),
            Name = x.Name,
            ShortCode = x.ShortCode
        }).ToList();
        context.Brands.AddRange(brands);
        context.SaveChanges();
    }
}
