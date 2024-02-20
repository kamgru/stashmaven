using System.Text.Json;

namespace StashMaven.WebApi.Data.Services;

public record Country(string Name, string IsoCode);

[Injectable]
public class CountryService(StashMavenRepository repository)
{
    public async Task<IReadOnlyList<Country>> GetAvailableCountries()
    {
        StashMavenOption? option = await repository.GetStashMavenOptionAsync(
            StashMavenOption.Keys.AvailableCountries);

        if (option is null || string.IsNullOrWhiteSpace(option.Value))
        {
            return [];
        }

        List<Country> countries = JsonSerializer.Deserialize<List<Country>>(option.Value)
                                  ?? throw new StashMavenException("Failed to deserialize available countries.");

        return countries;
    }
}
