using HumanResource.Service.DTOs;
using HumanResource.Service.Entities;
using HumanResource.Service.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.Service.Controllers;

[ApiController]
[Authorize]
[Route("api/HumanResource/Grade/v3")]
[Route("api/Grade/v3")]
public sealed class EmployeeGradeV3Controller : ControllerBase
{
    private readonly HumanResourceDbContext _context;

    public EmployeeGradeV3Controller(HumanResourceDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmployeeGradeV3Dto>>> Get()
    {
        var clientId = GetClientId();
        return Ok(await Grades(clientId)
            .OrderBy(x => x.Code)
            .Select(x => new EmployeeGradeV3Dto(x.Id, x.Code, x.MinimumSalary, x.MaximumSalary))
            .ToListAsync());
    }

    [HttpPost]
    public async Task<ActionResult<EmployeeGradeV3Dto>> Create([FromBody] UpsertEmployeeGradeV3Request request)
    {
        var clientId = GetClientId();
        var entityId = GetEntityId();
        if (!entityId.HasValue) return BadRequest("Entity context is required.");
        var error = Validate(request);
        if (error != null) return BadRequest(error);
        var code = request.Code!.Trim();
        if (await Grades(clientId).AnyAsync(x => x.Code == code))
            return BadRequest("Grade Code already exists for the current client.");

        var entity = new EmployeeGradeLegacy
        {
            Id = (await _context.EmployeeGrades.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id) ?? 0) + 1,
            ClientId = clientId,
            EntityId = entityId.Value, // Retained for legacy storage; grade availability is client-wide.
            Code = code,
            MinimumSalary = request.MinimumSalary!.Value,
            MaximumSalary = request.MaximumSalary!.Value,
            UtBlock = false,
            UtAddTime = DateTime.UtcNow,
            UtAddUser = GetActor(),
            UtRecordVersion = 1
        };
        _context.EmployeeGrades.Add(entity);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new EmployeeGradeV3Dto(entity.Id, entity.Code, entity.MinimumSalary, entity.MaximumSalary));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpsertEmployeeGradeV3Request request)
    {
        var clientId = GetClientId();
        var error = Validate(request);
        if (error != null) return BadRequest(error);
        var grade = await Grades(clientId).FirstOrDefaultAsync(x => x.Id == id);
        if (grade == null) return NotFound();
        var code = request.Code!.Trim();
        if (await Grades(clientId).AnyAsync(x => x.Id != id && x.Code == code))
            return BadRequest("Grade Code already exists for the current client.");

        grade.Code = code;
        grade.MinimumSalary = request.MinimumSalary!.Value;
        grade.MaximumSalary = request.MaximumSalary!.Value;
        grade.UtEditTime = DateTime.UtcNow;
        grade.UtEditUser = GetActor();
        grade.UtRecordVersion++;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var clientId = GetClientId();
        var grade = await Grades(clientId).FirstOrDefaultAsync(x => x.Id == id);
        if (grade == null) return NotFound();
        if (await _context.EmployeeProfiles.AnyAsync(x => x.ClientId == clientId && !(x.UtBlock ?? false)
            && _context.EmployeePositions.Any(p => p.EmployeeId == x.Id && p.GradeId == id && !(p.UtBlock ?? false))))
            return BadRequest("This grade is assigned to an employee and cannot be deleted.");
        grade.UtBlock = true;
        grade.UtEditTime = DateTime.UtcNow;
        grade.UtEditUser = GetActor();
        grade.UtRecordVersion++;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private IQueryable<EmployeeGradeLegacy> Grades(int clientId) =>
        _context.EmployeeGrades.Where(x => x.ClientId == clientId && !x.UtBlock);

    private static string? Validate(UpsertEmployeeGradeV3Request request)
    {
        if (string.IsNullOrWhiteSpace(request.Code)) return "Grade Code is required.";
        if (!request.MinimumSalary.HasValue) return "Minimum Salary is required.";
        if (!request.MaximumSalary.HasValue) return "Maximum Salary is required.";
        if (request.MinimumSalary > request.MaximumSalary) return "Minimum Salary cannot exceed Maximum Salary.";
        return null;
    }

    private int GetClientId() => int.TryParse(User.Claims.FirstOrDefault(c => c.Type is "ClientId" or "clientId" or "client_id")?.Value, out var id) && id > 0
        ? id
        : Request.Headers.TryGetValue("X-Client-Id", out var header) && int.TryParse(header, out id) && id > 0
            ? id : throw new InvalidOperationException("ClientId not found in token or headers");

    private int? GetEntityId() => int.TryParse(User.Claims.FirstOrDefault(c => c.Type is "EntityId" or "entityId" or "entity_id")?.Value, out var id) && id > 0
        ? id
        : Request.Headers.TryGetValue("X-Entity-Id", out var header) && int.TryParse(header, out id) && id > 0 ? id : null;

    private string GetActor() => User.Identity?.Name ?? User.Claims.FirstOrDefault(c => c.Type is "username" or "sub")?.Value ?? "system";
}
