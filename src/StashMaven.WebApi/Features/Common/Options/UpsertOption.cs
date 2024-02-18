using StashMaven.WebApi.Data.Services;

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
public class UpsertOptionHandler(StashMavenContext context,
    UpsertOptionService optionService,
    UnitOfWork unitOfWork)
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
                await optionService.UpsertCompanyOptionAsync(request.Key, request.Value);
                break;
            }
            case { Type: OptionType.StashMaven }:
            {
                await optionService.UpsertStashMavenOptionAsync(request.Key, request.Value);
                break;
            }
            default:
                return StashMavenResult.Error("Invalid option type");
        }

        await unitOfWork.SaveChangesAsync();

        return StashMavenResult.Success();
    }
}
