namespace StashMaven.WebApi.Features.Common.Options;

public partial class OptionController
{
    [HttpGet]
    [Route("list")]
    [ProducesResponseType<ListOptionsHandler.ListOptionsResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListOptionsAsync(
        [FromServices]
        ListOptionsHandler handler)
    {
        ListOptionsHandler.ListOptionsResponse result = await handler.ListOptionsAsync();

        return Ok(result);
    }
}

[Injectable]
public class ListOptionsHandler(StashMavenContext context)
{
    public class ListOptionsResponse
    {
        public Dictionary<string, string> Company { get; set; } = new();
        public Dictionary<string, string> StashMaven { get; set; } = new();
    }

    public async Task<ListOptionsResponse> ListOptionsAsync()
    {
        List<CompanyOption> companyOptions = await context.CompanyOptions.ToListAsync();
        List<StashMavenOption> stashMavenOptions = await context.StashMavenOptions.ToListAsync();

        return new ListOptionsResponse
        {
            Company = companyOptions.ToDictionary(x => x.Key, x => x.Value),
            StashMaven = stashMavenOptions.ToDictionary(x => x.Key, x => x.Value)
        };
    }
}



