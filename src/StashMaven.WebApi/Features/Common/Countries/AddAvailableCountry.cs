using System.Text.Json;
using StashMaven.WebApi.Data.Services;

namespace StashMaven.WebApi.Features.Common.Countries;

public partial class CountryController
{
    [HttpPost]
    [Route("available")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddAvailableCountryAsync(
        [FromServices]
        AddAvailableCountryHandler handler,
        AddAvailableCountryHandler.AddAvailableCountryRequest request)
    {
        StashMavenResult result = await handler.AddAvailableCountryAsync(request);
        return Ok(result);
    }
}

[Injectable]
public class AddAvailableCountryHandler(
    UpsertOptionService optionService,
    CountryService countryService,
    CacheReader cacheReader,
    UnitOfWork unitOfWork)
{
    public class AddAvailableCountryRequest
    {
        public required string Name { get; set; }
        public required string Code { get; set; }
    }

    public async Task<StashMavenResult> AddAvailableCountryAsync(
        AddAvailableCountryRequest request)
    {
        IReadOnlyList<Country> countries = await countryService.GetAvailableCountries();
        if (countries.Any(c => c.IsoCode == request.Code))
        {
            return StashMavenResult.Success();
        }

        List<Country> availableCountries = countries.ToList();
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
