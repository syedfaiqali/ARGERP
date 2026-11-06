using ARGERP.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Organization.Service.Infrastructure.Data;
namespace ARGERP.Support;
[ApiController, Authorize]
public sealed class StandaloneSupportController(OrganizationDbContext db, ArgAuthDbContext auth) : ControllerBase
{
    [HttpGet("api/auth/Legalentity/v3/Legalentity")]
    public async Task<IActionResult> Entities(CancellationToken ct) => Ok(await db.OrgLegalentityLegacy.AsNoTracking().Where(x => x.clientid == 1 && !x.utblock).Select(x => new { id = x.Id, entityId = x.Id, name = x.stxt, stxt = x.stxt, instituteName = x.stxt, code = x.entityno }).ToListAsync(ct));
    [HttpGet("api/Administration/SystemPolicyGeneral/v3/grid-paging")]
    public IActionResult Paging() => Ok(new { gridPaging = 20, pageSize = 20, rowsPerPage = 20, pageSizeOptions = new[] { 10, 20, 50, 100 } });
    [HttpGet("api/arg/users")]
    public async Task<IActionResult> Users(CancellationToken ct) => Ok(await auth.Users.AsNoTracking().Select(x => new { x.Id, x.Username, x.FullName, x.EmployeeId, x.EntityId, x.Blocked }).ToListAsync(ct));
    public sealed record SaveUser(string Username, string FullName, string Password, int? EmployeeId, int EntityId = 1);
    [HttpPost("api/arg/users")]
    public async Task<IActionResult> AddUser(SaveUser request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.FullName) || request.Password.Length < 12) return BadRequest(new { error = "Username, name and a password of at least 12 characters are required." });
        if (await auth.Users.AnyAsync(x => x.Username == request.Username.Trim(), ct)) return Conflict(new { error = "Username already exists." });
        if (!await db.OrgLegalentityLegacy.AnyAsync(x => x.Id == request.EntityId && x.clientid == 1 && !x.utblock, ct)) return BadRequest(new { error = "Choose an existing branch." });
        if (request.EmployeeId.HasValue && !await db.EmployeeinfohdrLegacy.AnyAsync(x => x.Id == request.EmployeeId, ct)) return BadRequest(new { error = "Choose an existing employee." });
        var user = new ArgUser { Username = request.Username.Trim(), FullName = request.FullName.Trim(), ClientId = 1, EntityId = request.EntityId, EmployeeId = request.EmployeeId };
        user.PasswordHash = new PasswordHasher<ArgUser>().HashPassword(user, request.Password);
        auth.Users.Add(user); await auth.SaveChangesAsync(ct); return Ok(new { user.Id, user.Username });
    }
}
