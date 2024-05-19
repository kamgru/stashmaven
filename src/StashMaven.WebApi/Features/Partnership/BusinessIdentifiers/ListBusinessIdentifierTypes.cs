namespace StashMaven.WebApi.Features.Partnership.BusinessIdentifiers;

public partial class BusinessIdentifierController 
{
    [HttpGet]
    [Route("types")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult ListBusinessIdentifierTypes()
    {
        return Ok(BusinessIdType.All());
    }
}
