using Microsoft.EntityFrameworkCore.Metadata;
using Npgsql;
using StashMaven.WebApi.Data.Services;

namespace StashMaven.WebApi.Features.Partnership.Partners;

public partial class PartnerController
{
    [HttpPost]
    [ProducesResponseType<AddPartnerHandler.AddPartnerResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddPartnerAsync(
        AddPartnerHandler.AddPartnerRequest request,
        [FromServices]
        AddPartnerHandler handler)
    {
        StashMavenResult<AddPartnerHandler.AddPartnerResponse> result = await handler.AddPartnerAsync(request);
        
        if (!result.IsSuccess || result.Data is null)
        {
            return BadRequest(result.ErrorCode);
        }
        
        return Created($"/api/v1/partner/{result.Data.PartnerId}", result.Data);
    }
}

[Injectable]
public class AddPartnerHandler(StashMavenContext context, CountryService countryService)
{
    public class AddPartnerRequest
    {
        public class AddPartnerAddress
        {
            public required string Street { get; set; }
            public string? StreetAdditional { get; set; }
            public required string City { get; set; }
            public string? State { get; set; }
            public required string PostalCode { get; set; }
            
            [MinLength(2)]
            [MaxLength(3)]
            public required string CountryCode { get; set; }
        }
        
        public class AddPartnerBusinessIdentifier
        {
            public required string Type { get; set; }
            public required string Value { get; set; }
        }
        
        [MinLength(Partner.CustomIdentifierMinLength)]
        [MaxLength(Partner.CustomIdentifierMaxLength)]
        public required string CustomIdentifier { get; set; }
        
        [MinLength(Partner.LegalNameMinLength)]
        [MaxLength(Partner.LegalNameMaxLength)]
        public required string LegalName { get; set; }
        
        public required List<AddPartnerBusinessIdentifier> BusinessIdentifiers { get; set; } = [];
        public required AddPartnerAddress Address { get; set; }
    }
    
    public record AddPartnerResponse(string PartnerId);
    
    public async Task<StashMavenResult<AddPartnerResponse>> AddPartnerAsync(
        AddPartnerRequest request)
    {
        PartnerId partnerId = new(Guid.NewGuid().ToString());
        
        StashMavenResult countryAvailableResult =
            await countryService.IsCountryAvailableAsync(request.Address.CountryCode);
        
        if (!countryAvailableResult.IsSuccess)
        {
            return StashMavenResult<AddPartnerResponse>.Error(ErrorCodes.CountryCodeNotSupported);
        }
        
        List<BusinessIdentifier> businessIdentifiers = [];
        foreach (AddPartnerRequest.AddPartnerBusinessIdentifier requestBusinessId in request.BusinessIdentifiers
                     .Where(x => !string.IsNullOrWhiteSpace(x.Value)))
        {
            BusinessIdType businessIdType = BusinessIdType.FromId(requestBusinessId.Type);
            if (businessIdType.Equals(BusinessIdType.Unknown))
            {
                return StashMavenResult<AddPartnerResponse>.Error(ErrorCodes.UnknownBusinessIdentifierType);
            }
            
            BusinessIdentifier businessId = new()
            {
                Type = businessIdType.Id,
                IsPrimary = businessIdType.IsPrimary,
                Value = requestBusinessId.Value,
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow
            };
            businessIdentifiers.Add(businessId);
        }
        
        if (!businessIdentifiers.Any(b => b.IsPrimary))
        {
            return StashMavenResult<AddPartnerResponse>.Error(ErrorCodes.NoPrimaryBusinessIdentifier);
        }
        
        Partner partner = new()
        {
            LegalName = request.LegalName,
            CustomIdentifier = request.CustomIdentifier,
            PartnerId = partnerId,
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow,
            Address = new Address
            {
                Street = request.Address.Street,
                StreetAdditional = request.Address.StreetAdditional,
                City = request.Address.City,
                State = request.Address.State,
                PostalCode = request.Address.PostalCode,
                CountryCode = request.Address.CountryCode,
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow
            },
            BusinessIdentifiers = businessIdentifiers
        };
        
        context.Partners.Add(partner);
        
        try
        {
            await context.SaveChangesAsync();
            return StashMavenResult<AddPartnerResponse>.Success(new AddPartnerResponse(partnerId.Value));
        }
        catch (DbUpdateException exc)
        {
            if (exc.InnerException is
                PostgresException { SqlState: StashMavenContext.PostgresUniqueViolation } postgresException)
            {
                List<IIndex> uniqueIndexes = context.Model.GetEntityTypes()
                    .SelectMany(x => x.GetDeclaredIndexes())
                    .Where(x => x.IsUnique)
                    .ToList();
                
                var mappedIndexes = uniqueIndexes.SelectMany(
                        x => x.GetMappedTableIndexes(),
                        (
                            index,
                            mappedIndex) => new { mappedIndex, index })
                    .ToList();
                
                IIndex? violatedIndex = mappedIndexes.FirstOrDefault(
                        x => x.mappedIndex.Name == postgresException.ConstraintName)
                    ?.index;
                
                if (violatedIndex is null)
                {
                    return StashMavenResult<AddPartnerResponse>.Error(ErrorCodes.FatalError);
                }
                
                switch (violatedIndex)
                {
                    case
                    {
                        DeclaringEntityType.ClrType.Name: nameof(BusinessIdentifier),
                        Properties:
                        [
                            { Name: nameof(BusinessIdentifier.Type) },
                            { Name: nameof(BusinessIdentifier.Value) }
                        ]
                    }:
                        return StashMavenResult<AddPartnerResponse>.Error(ErrorCodes.BusinessIdentifierNotUnique);
                    case
                    {
                        DeclaringEntityType.ClrType.Name: nameof(Partner),
                        Properties: [{ Name: nameof(Partner.CustomIdentifier) }]
                    }:
                        return StashMavenResult<AddPartnerResponse>.Error(ErrorCodes.CustomIdentifierNotUnique);
                }
            }
            
            throw;
        }
    }
}
