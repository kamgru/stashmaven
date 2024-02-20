using Microsoft.AspNetCore.Authorization;

namespace StashMaven.WebApi.Features.Common.Countries;

[ApiController]
[Authorize]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public partial class CountryController : ControllerBase;
