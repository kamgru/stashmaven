using Microsoft.AspNetCore.Authorization;

namespace StashMaven.WebApi.Features.Partners;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public partial class PartnerController : ControllerBase;
