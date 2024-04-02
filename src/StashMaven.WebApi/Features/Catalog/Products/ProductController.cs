using Microsoft.AspNetCore.Authorization;

namespace StashMaven.WebApi.Features.Catalog.Products;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public partial class ProductController : ControllerBase;
