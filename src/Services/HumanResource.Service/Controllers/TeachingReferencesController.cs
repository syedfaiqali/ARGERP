using HumanResource.Service.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace HumanResource.Service.Controllers;
// Local teaching-reference lookups used by employee specialization, independent of Academics services.
[ApiController, Authorize, Route("api/TeachingReferences")]
public sealed class TeachingReferencesController(HumanResourceDbContext db) : ControllerBase
{
    public sealed record Option(int Id,string? Name,string? CourseName);
    [HttpGet("Courses/v3")]
    public async Task<IActionResult> Classes([FromQuery]int entityId=1,CancellationToken ct=default)
    {
        var items=await db.Database.SqlQuery<Option>($"SELECT Id,stxt Name,stxt CourseName FROM SMCourse WHERE clientid=1 AND utblock=0 AND (entityid={entityId} OR entityid IS NULL) ORDER BY stxt").ToListAsync(ct);
        return Ok(new { items=items.Select(x=>new { x.Id,courseName=x.Name }),totalCount=items.Count });
    }
    [HttpGet("Subjects/v3")]
    public async Task<IActionResult> Subjects([FromQuery]int entityId=1,CancellationToken ct=default)
    {
        var items=await db.Database.SqlQuery<Option>($"SELECT Id,stxt Name,CAST(NULL AS nvarchar(150)) CourseName FROM SMSubject WHERE clientid=1 AND utblock=0 AND (entityid={entityId} OR entityid IS NULL) ORDER BY stxt").ToListAsync(ct);
        return Ok(new { items,totalCount=items.Count });
    }
}
