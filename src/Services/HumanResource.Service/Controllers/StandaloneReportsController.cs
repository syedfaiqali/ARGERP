using HumanResource.Service.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace HumanResource.Service.Controllers;
[ApiController, Authorize, Route("api/Reports/v3")]
public sealed class StandaloneReportsController(HumanResourceDbContext db) : ControllerBase
{
    private int EntityId => int.TryParse(Request.Headers["X-Entity-Id"], out var id) ? id : 1;
    [HttpGet("employees")]
    public async Task<IActionResult> Employees(CancellationToken ct) => Ok(await db.EmployeeProfiles.AsNoTracking().Where(x => x.ClientId == 1 && x.EntityId == EntityId && !(x.UtBlock ?? false)).OrderBy(x => x.EmployeeCode)
        .Select(x => new { x.EmployeeCode, employee = (x.FirstName ?? "") + " " + (x.LastName ?? ""), x.JoiningDate, x.EmailId, x.CellNo, x.DepartmentId, x.DesignationId }).ToListAsync(ct));
    [HttpGet("attendance")]
    public async Task<IActionResult> Attendance([FromQuery] DateTime from, [FromQuery] DateTime to, CancellationToken ct)
    {
        if (from > to) return BadRequest(new { error = "Choose a valid date range." });
        return Ok(await db.MonthlyAttendances.AsNoTracking().Where(x => x.EntityId == EntityId && !x.UtBlock && x.StartDate <= to && x.EndDate >= from)
            .Select(x => new { employee = x.Employee == null ? "" : (x.Employee.FirstName ?? "") + " " + (x.Employee.LastName ?? ""), x.StartDate, x.EndDate, x.Days, present = x.PresentsCount, absent = x.AbsentsCount, x.Leaves, x.Holidays, late = x.LateCount }).ToListAsync(ct));
    }
    [HttpGet("payslips")]
    public async Task<IActionResult> Payslips([FromQuery] DateTime from, [FromQuery] DateTime to, CancellationToken ct)
    {
        if (from > to) return BadRequest(new { error = "Choose a valid date range." });
        return Ok(await db.PayrollProcesses.AsNoTracking().Where(x => x.clientid == 1 && x.entityid == EntityId && !x.utblock && x.fromdate <= to && x.tilldate >= from)
            .Select(x => new { employee = x.Employee == null ? "" : (x.Employee.FirstName ?? "") + " " + (x.Employee.LastName ?? ""), from = x.fromdate, to = x.tilldate, basicSalary = x.salary, allowances = x.allowance, deductions = x.deduction, tax = x.tax, grossSalary = x.grosssalary }).ToListAsync(ct));
    }
}
