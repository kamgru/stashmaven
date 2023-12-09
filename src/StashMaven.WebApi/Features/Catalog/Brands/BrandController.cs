using Microsoft.AspNetCore.Authorization;

namespace StashMaven.WebApi.Features.Catalog.Brands;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public partial class BrandController : ControllerBase;
