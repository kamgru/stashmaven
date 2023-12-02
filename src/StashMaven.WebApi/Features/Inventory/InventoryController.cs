using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StashMaven.WebApi.Features.Inventory;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public partial class InventoryController : ControllerBase;
