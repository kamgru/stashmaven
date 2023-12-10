using Microsoft.AspNetCore.Authorization;

namespace StashMaven.WebApi.Features.Partners;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public partial class PartnerController : ControllerBase;
