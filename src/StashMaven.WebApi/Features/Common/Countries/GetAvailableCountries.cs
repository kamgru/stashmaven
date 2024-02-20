using StashMaven.WebApi.Data.Services;

namespace StashMaven.WebApi.Features.Common.Countries;

public partial class CountryController
{
    [HttpGet]
    [Route("available")]
    [ProducesResponseType<List<GetAvailableCountriesHandler.CountryItem>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAvailableCountriesAsync(
        [FromServices]
        GetAvailableCountriesHandler handler)
    {
        IReadOnlyList<GetAvailableCountriesHandler.CountryItem> countries = await handler.GetAvailableCountriesAsync();
        return Ok(countries);
    }
}

[Injectable]
public class GetAvailableCountriesHandler(CacheReader cacheReader)
{
    public class CountryItem
    {
        public required string Name { get; set; }
        public required string Code { get; set; }
    }

    public async Task<IReadOnlyList<CountryItem>> GetAvailableCountriesAsync()
    {
        IReadOnlyList<Country> countries = await cacheReader.GetAvailableCountriesAsync();
        return countries.Select(e => new CountryItem
        {
            Name = e.Name,
            Code = e.IsoCode
        }).ToList();
    }
}
