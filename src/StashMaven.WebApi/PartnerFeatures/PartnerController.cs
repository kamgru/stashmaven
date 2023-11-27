using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StashMaven.WebApi.Data;

namespace StashMaven.WebApi.PartnerFeatures;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class PartnerController : ControllerBase
{
    private readonly CreatePartnerHandler _createPartnerHandler;
    private readonly GetPartnerByIdHandler _getPartnerByIdHandler;
    private readonly ListPartnersHandler _listPartnersHandler;
    private readonly UpdatePartnerHandler _updatePartnerHandler;

    public PartnerController(
        CreatePartnerHandler createPartnerHandler,
        GetPartnerByIdHandler getPartnerByIdHandler,
        ListPartnersHandler listPartnersHandler,
        UpdatePartnerHandler updatePartnerHandler)
    {
        _createPartnerHandler = createPartnerHandler;
        _getPartnerByIdHandler = getPartnerByIdHandler;
        _listPartnersHandler = listPartnersHandler;
        _updatePartnerHandler = updatePartnerHandler;
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
        PartnerId partnerId = await _createPartnerHandler.CreatePartnerAsync(request);
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

    [HttpPatch]
    [Route("{partnerId}")]
    public async Task<IActionResult> UpdatePartnerAsync(
        string partnerId,
        UpdatePartnerHandler.PatchPartnerRequest request)
    {
        await _updatePartnerHandler.PatchPartnerAsync(partnerId, request);
        return Ok();
    }
}
