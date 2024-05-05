using Microsoft.AspNetCore.Authorization;

namespace StashMaven.WebApi.Features.Inventory.ShipmentKinds;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
[Produces("application/json")]
public partial class ShipmentKindController : ControllerBase;
