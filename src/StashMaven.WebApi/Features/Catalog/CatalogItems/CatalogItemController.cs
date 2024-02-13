using Microsoft.AspNetCore.Authorization;

namespace StashMaven.WebApi.Features.Catalog.CatalogItems;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public partial class CatalogItemController : ControllerBase;
