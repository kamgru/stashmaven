using Npgsql;
using StashMaven.WebApi.Data.Services;

namespace StashMaven.WebApi.Features.Partnership.BusinessIdentifiers;

public partial class BusinessIdentifierController
{
    [HttpPost]
    [ProducesResponseType<AddBusinessIdentifierHandler.AddBusinessIdentifierResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddBusinessIdentifierAsync(
        AddBusinessIdentifierHandler.AddBusinessIdentifierRequest request,
        [FromServices]
        AddBusinessIdentifierHandler handler)
    {
        StashMavenResult<AddBusinessIdentifierHandler.AddBusinessIdentifierResponse> result =
            await handler.AddBusinessIdentifierAsync(request);
        
        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorCode);
        }
        
        return Created($"/api/v1/businessidentifier/{result.Data?.BusinessIdentifierId}", result.Data);
    }
}

[Injectable]
public class AddBusinessIdentifierHandler(StashMavenContext context, CacheReader cacheReader)
{
    public class AddBusinessIdentifierRequest
    {
        [MinLength(5)]
        [MaxLength(256)]
        public required string Name { get; set; }
        
        [MinLength(2)]
        [MaxLength(10)]
        public required string ShortCode { get; set; }
    }
    
    public record AddBusinessIdentifierResponse(string BusinessIdentifierId);
    
    public async Task<StashMavenResult<AddBusinessIdentifierResponse>> AddBusinessIdentifierAsync(
        AddBusinessIdentifierRequest request)
    {
        BusinessIdentifier businessIdentifier = new()
        {
            BusinessIdentifierId = new BusinessIdentifierId(Guid.NewGuid().ToString()),
            Name = request.Name,
            ShortCode = request.ShortCode,
        };
        
        context.BusinessIdentifiers.Add(businessIdentifier);
        
        try
        {
            await context.SaveChangesAsync();
            cacheReader.InvalidateKey(CacheReader.Keys.BusinessIdentifiers);
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
            {
                return StashMavenResult<AddBusinessIdentifierResponse>.Error(ErrorCodes
                    .BusinessIdentifierShortCodeNotUnique);
            }
            
            throw;
        }
        
        return StashMavenResult<AddBusinessIdentifierResponse>.Success(
            new AddBusinessIdentifierResponse(businessIdentifier.BusinessIdentifierId.Value));
    }
}
