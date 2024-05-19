using Microsoft.AspNetCore.Authorization;

namespace StashMaven.WebApi.Features.Partnership.BusinessIdentifiers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public partial class BusinessIdentifierController : ControllerBase;
