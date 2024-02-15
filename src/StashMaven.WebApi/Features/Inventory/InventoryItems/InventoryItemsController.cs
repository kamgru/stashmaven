using Microsoft.AspNetCore.Authorization;

namespace StashMaven.WebApi.Features.Inventory.InventoryItems;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
[Produces("application/json")]
public partial class InventoryItemController : ControllerBase;
