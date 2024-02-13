using Microsoft.AspNetCore.Authorization;

namespace StashMaven.WebApi.Features.Inventory.Stockpiles;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public partial class StockpileController : ControllerBase;
