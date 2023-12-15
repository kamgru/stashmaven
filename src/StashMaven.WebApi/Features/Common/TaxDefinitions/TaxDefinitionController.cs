using Microsoft.AspNetCore.Authorization;

namespace StashMaven.WebApi.Features.Common.TaxDefinitions;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public partial class TaxDefinitionController : ControllerBase;
