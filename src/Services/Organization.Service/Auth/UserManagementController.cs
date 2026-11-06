using System.Security.Claims;
using ARGERP.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Organization.Service.Infrastructure.Data;
namespace ARGERP.Support;
[ApiController, Authorize, Route("api/arg/users")]
public sealed class UserManagementController(OrganizationDbContext db, ArgAuthDbContext auth) : ControllerBase
{
    public sealed record UpdateUser(string FullName,int? EmployeeId,int EntityId,bool Blocked,string? Password);
    [HttpGet("options")]
    public async Task<IActionResult> Options(CancellationToken ct) => Ok(new {
        employees=await db.EmployeeinfohdrLegacy.AsNoTracking().Where(x=>x.clientid==1 && !x.utblock).Select(x=>new { id=x.Id,name=(x.firstname??"")+" "+(x.lastname??""),code=x.employeecode,entityId=x.entityid }).ToListAsync(ct),
        branches=await db.OrgLegalentityLegacy.AsNoTracking().Where(x=>x.clientid==1&&!x.utblock).Select(x=>new { id=x.Id,name=x.stxt }).ToListAsync(ct)
    });
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id,UpdateUser request,CancellationToken ct)
    {
        var user=await auth.Users.SingleOrDefaultAsync(x=>x.Id==id,ct);if(user==null)return NotFound();
        if(string.IsNullOrWhiteSpace(request.FullName))return BadRequest(new {error="Full name is required."});
        if(request.Blocked && id==int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!))return BadRequest(new {error="You cannot deactivate your own account."});
        if(!await db.OrgLegalentityLegacy.AnyAsync(x=>x.Id==request.EntityId&&x.clientid==1&&!x.utblock,ct))return BadRequest(new {error="Choose an existing branch."});
        if(request.EmployeeId.HasValue&&!await db.EmployeeinfohdrLegacy.AnyAsync(x=>x.Id==request.EmployeeId&&x.clientid==1,ct))return BadRequest(new {error="Choose an existing employee."});
        if(!string.IsNullOrEmpty(request.Password)&&request.Password.Length<12)return BadRequest(new {error="Use a password of at least 12 characters."});
        if(user.Blocked!=request.Blocked||user.EntityId!=request.EntityId||user.EmployeeId!=request.EmployeeId||!string.IsNullOrEmpty(request.Password))user.SessionVersion=(user.SessionVersion??0)+1;
        user.FullName=request.FullName.Trim();user.EntityId=request.EntityId;user.EmployeeId=request.EmployeeId;user.Blocked=request.Blocked;
        if(!string.IsNullOrEmpty(request.Password))user.PasswordHash=new PasswordHasher<ArgUser>().HashPassword(user,request.Password);
        await auth.SaveChangesAsync(ct);return NoContent();
    }
}
