using Microsoft.AspNetCore.Authorization;

namespace StashMaven.WebApi.Features.Inventory.Shipments;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public partial class ShipmentController : ControllerBase;
