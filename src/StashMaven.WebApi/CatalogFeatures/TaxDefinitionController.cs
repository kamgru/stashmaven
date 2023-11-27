using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StashMaven.WebApi.Data;

namespace StashMaven.WebApi.CatalogFeatures;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class TaxDefinitionController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateTaxDefinitionAsync(
        CreateTaxDefinitionHandler.CreateTaxDefinitionRequest request,
        [FromServices]
        CreateTaxDefinitionHandler handler)
    {
        TaxDefinitionId taxDefinitionId = await handler.CreateTaxDefinitionAsync(request);

        return Created($"api/v1/catalog/TaxDefinition/{taxDefinitionId.Value}", taxDefinitionId.ToString());
    }

    [HttpGet]
    [Route("list")]
    public async Task<IActionResult> ListTaxDefinitionsAsync(
        [FromQuery]
        ListTaxDefinitionsHandler.ListTaxDefinitionsRequest request,
        [FromServices]
        ListTaxDefinitionsHandler handler)
    {
        ListTaxDefinitionsHandler.ListTaxDefinitionsResponse response =
            await handler.ListTaxDefinitionsAsync(request);
        return Ok(response);
    }

    [HttpGet]
    [Route("{taxDefinitionId}")]
    public async Task<IActionResult> GetTaxDefinitionByIdAsync(
        string taxDefinitionId,
        [FromServices]
        GetTaxDefinitionByIdHandler handler)
    {
        StashMavenResult<GetTaxDefinitionByIdHandler.GetTaxDefinitionByIdResponse> response =
            await handler.GetTaxDefinitionByIdAsync(taxDefinitionId);

        if (!response.IsSuccess)
        {
            return NotFound();
        }

        return Ok(response.Data);
    }
}
