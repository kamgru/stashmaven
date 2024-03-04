using System.Text.Json;
using StashMaven.WebApi.Data.Services;

namespace StashMaven.WebApi.Features.Common.Countries;

public partial class CountryController
{
    [HttpPut]
    [Route("available")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateAvailableCountryAsync(
        [FromServices]
        UpdateAvailableCountryHandler handler,
        UpdateAvailableCountryHandler.UpdateAvailableCountryRequest request)
    {
        StashMavenResult result = await handler.UpdateAvailableCountryAsync(request);
        return Ok(result);
    }
}

[Injectable]
public class UpdateAvailableCountryHandler(
    UpsertOptionService optionService,
    CountryService countryService,
    CacheReader cacheReader,
    UnitOfWork unitOfWork)
{
    public class UpdateAvailableCountryRequest
    {
        public required string Name { get; set; }
        public required string Code { get; set; }
    }

    public async Task<StashMavenResult> UpdateAvailableCountryAsync(
        UpdateAvailableCountryRequest request)
    {
        IReadOnlyList<Country> countries = await countryService.GetAvailableCountries();
        Country? country = countries.FirstOrDefault(c => c.IsoCode == request.Code);
        if (country is null)
        {
            return StashMavenResult.Error(ErrorCodes.CountryNotFound);
        }

        List<Country> availableCountries = countries.ToList();
        availableCountries.RemoveAll(c => c.IsoCode == request.Code);
        availableCountries.Add(new Country(request.Name, request.Code));

        string value = JsonSerializer.Serialize(availableCountries);

        await optionService.UpsertStashMavenOptionAsync(
            StashMavenOption.Keys.AvailableCountries,
            value);

        await unitOfWork.SaveChangesAsync();

        cacheReader.InvalidateKey(CacheReader.Keys.AvailableCountries);

        return StashMavenResult.Success();
    }
}
