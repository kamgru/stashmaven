using Microsoft.AspNetCore.Authorization;

namespace StashMaven.WebApi.Features.Common.Options;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public partial class OptionController : ControllerBase;
