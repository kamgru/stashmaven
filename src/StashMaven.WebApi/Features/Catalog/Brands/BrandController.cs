using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StashMaven.WebApi.Features.Catalog.Brands;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public partial class BrandController : ControllerBase;
