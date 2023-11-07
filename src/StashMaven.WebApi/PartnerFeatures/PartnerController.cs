using Microsoft.AspNetCore.Mvc;

namespace StashMaven.WebApi.PartnerFeatures;

[ApiController]
[Route("api/v1/[controller]")]
public class PartnerController : ControllerBase
{
    private readonly CreatePartnerHandler _createPartnerHandler;
    private readonly GetPartnerByIdHandler _getPartnerByIdHandler;
    private readonly ListPartnersHandler _listPartnersHandler;

    public PartnerController(
        CreatePartnerHandler createPartnerHandler,
        GetPartnerByIdHandler getPartnerByIdHandler,
        ListPartnersHandler listPartnersHandler)
    {
        _createPartnerHandler = createPartnerHandler;
        _getPartnerByIdHandler = getPartnerByIdHandler;
        _listPartnersHandler = listPartnersHandler;
    }

    [HttpGet]
    [Route("{partnerId}")]
    public async Task<IActionResult> GetPartnerByIdAsync(
        string partnerId)
    {
        StashMavenResult<GetPartnerByIdHandler.GetPartnerResponse> response =
            await _getPartnerByIdHandler.GetPartnerByIdAsync(partnerId);

        if (!response.IsSuccess)
        {
            return NotFound();
        }

        return Ok(response.Data);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePartnerAsync(
        CreatePartnerHandler.CreatePartnerRequest request)
    {
        Guid partnerId = await _createPartnerHandler.CreatePartnerAsync(request);
        return Created($"/api/v1/partner/{partnerId}", partnerId.ToString());
    }

    [HttpGet]
    [Route("list")]
    public async Task<IActionResult> ListPartnersAsync(
        [FromQuery]ListPartnersHandler.ListPartnerRequest request)
    {
        ListPartnersHandler.ListPartnerResponse response =
            await _listPartnersHandler.ListPartnersAsync(request);
        return Ok(response);
    }
}
