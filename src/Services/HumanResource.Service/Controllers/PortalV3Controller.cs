using System.Security.Claims;
using HumanResource.Service.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.Service.Controllers;

[ApiController, Authorize, Route("api/Portal/v3")]
public sealed class PortalV3Controller(HumanResourceDbContext db) : ControllerBase
{
    [HttpGet("employees/search")]
    public async Task<IActionResult> Search(string? searchTerm, int pageNumber = 1, CancellationToken ct = default)
    {
        if (!int.TryParse(User.FindFirstValue("clientId"), out var client)) return Forbid();
        if (!int.TryParse(Request.Headers["X-Entity-Id"], out var campus) ||
            !await db.OrgLegalentities.AnyAsync(x => x.Id == campus && x.ClientId == client && !x.UtBlock, ct))
            return BadRequest(new { error = "Select an active campus/branch first." });
        var term = searchTerm?.Trim() ?? "";
        if (term.Length > 100) return BadRequest(new { error = "Search cannot exceed 100 characters." });
        if (term.Length == 0) return Ok(new { items = Array.Empty<object>(), totalCount = 0 });
        var query = db.EmployeeProfiles.AsNoTracking().Where(x => x.ClientId == client && x.EntityId == campus && !(x.UtBlock ?? false)
            && ((x.EmployeeCode ?? "").Contains(term) || ((x.FirstName ?? "") + " " + (x.LastName ?? "")).Contains(term)));
        var total = await query.CountAsync(ct);
        var page = Math.Clamp(pageNumber, 1, 10000);
        var items = await query.OrderBy(x => x.FirstName).ThenBy(x => x.Id).Skip((page - 1) * 25).Take(25)
            .Select(x => new { x.Id, name = ((x.FirstName ?? "") + " " + (x.LastName ?? "")).Trim(), code = x.EmployeeCode }).ToListAsync(ct);
        return Ok(new { items, totalCount = total });
    }
}
