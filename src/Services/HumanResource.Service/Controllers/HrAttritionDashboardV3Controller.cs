using Auth.Service.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Auth.Service.API.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/Dashboard/v3/attrition")]
public sealed class HrAttritionDashboardV3Controller(HrAttritionDashboardService dashboard, ILogger<HrAttritionDashboardV3Controller> logger) : ControllerBase
{
    [HttpGet]
    public Task<IActionResult> Summary([FromQuery] DateTime from, [FromQuery] DateTime to,
        [FromQuery] int? entityId = null, [FromQuery] int? departmentId = null, CancellationToken cancellationToken = default) =>
        Execute((user, client) => dashboard.GetAsync(user, client, entityId, departmentId, from, to, cancellationToken), cancellationToken);

    [HttpGet("details")]
    public Task<IActionResult> Details([FromQuery] DateTime from, [FromQuery] DateTime to, [FromQuery] string kind,
        [FromQuery] int? entityId = null, [FromQuery] int? departmentId = null, [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10, [FromQuery] string? filters = null, CancellationToken cancellationToken = default) =>
        Execute((user, client) => dashboard.DetailsAsync(user, client, entityId, departmentId, from, to, kind, pageNumber, pageSize, filters, cancellationToken), cancellationToken);

    private async Task<IActionResult> Execute<T>(Func<int, int, Task<T>> action, CancellationToken ct)
    {
        var userClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("userId") ?? User.FindFirstValue("sub");
        var clientClaim = User.FindFirstValue("clientId") ?? User.FindFirstValue("clientid");
        if (!int.TryParse(userClaim, out var userId) || userId <= 0 || !int.TryParse(clientClaim, out var clientId) || clientId <= 0) return Unauthorized();
        try { return Ok(await action(userId, clientId)); }
        catch (UnauthorizedAccessException) { return Forbid(); }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
        catch (OperationCanceledException) when (ct.IsCancellationRequested) { throw; }
        catch (Exception ex)
        {
            logger.LogError(ex, "HR attrition dashboard failed for client {ClientId}", clientId);
            return StatusCode(500, new { error = "HR dashboard could not be loaded. Please retry." });
        }
    }
}
