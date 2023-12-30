namespace StashMaven.WebApi.Features.Common.Options;

public partial class OptionController
{
    [HttpPut]
    public async Task<IActionResult> UpsertOptionAsync(
        [FromBody]
        UpsertOptionHandler.UpsertOptionRequest request,
        [FromServices]
        UpsertOptionHandler handler)
    {
        StashMavenResult result = await handler.UpsertOptionAsync(request);

        return result.IsSuccess
            ? Ok()
            : BadRequest(result.Message);
    }
}

[Injectable]
public class UpsertOptionHandler(StashMavenContext context)
{
    public enum OptionType
    {
        Company = 0,
        StashMaven = 1
    }

    public class UpsertOptionRequest
    {
        public required string Key { get; set; }
        public required string Value { get; set; }
        public required OptionType Type { get; set; }
    }

    public async Task<StashMavenResult> UpsertOptionAsync(
        UpsertOptionRequest request)
    {
        switch (request)
        {
            case { Type: OptionType.Company }:
            {
                await UpsertCompanyOption(request);
                break;
            }
            case { Type: OptionType.StashMaven }:
            {
                await UpsertStashMavenOption(request);
                break;
            }
            default:
                return StashMavenResult.Error("Invalid option type");
        }

        await context.SaveChangesAsync();

        return StashMavenResult.Success();
    }

    private async Task UpsertStashMavenOption(
        UpsertOptionRequest request)
    {
        StashMavenOption? option = await context.StashMavenOptions
            .FirstOrDefaultAsync(x => x.Key == request.Key);

        if (option == null)
        {
            option = new StashMavenOption
            {
                Key = request.Key,
                Value = request.Value
            };
            context.StashMavenOptions.Add(option);
        }
        else
        {
            option.Value = request.Value;
        }
    }

    private async Task UpsertCompanyOption(
        UpsertOptionRequest request)
    {
        CompanyOption? option = await context.CompanyOptions
            .FirstOrDefaultAsync(x => x.Key == request.Key);

        if (option == null)
        {
            option = new CompanyOption
            {
                Key = request.Key,
                Value = request.Value
            };
            context.CompanyOptions.Add(option);
        }
        else
        {
            option.Value = request.Value;
        }
    }
}
