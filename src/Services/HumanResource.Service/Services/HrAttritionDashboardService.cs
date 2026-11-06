using HumanResource.Service.Infrastructure.Data;

using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Auth.Service.Services;

public sealed class HrAttritionDashboardService(HumanResourceDbContext organization)
{
    private sealed record Source(List<HrLookup> Entities, List<HrLookup> Departments, List<HrEmployee> Employees,
        List<HrPositionHistory> History, List<HrOpenPosition> Positions, bool PositionsAvailable)
    {
        public HrAttritionMetrics Metrics => new(Employees, History, Departments, Positions, PositionsAvailable);
        public string Note => HrAttritionMetrics.DataNote +
            (PositionsAvailable ? " Open positions reflect current vacancies, independently of the selected date range." : " Vacancy tables are unavailable; open positions cannot be reported.") +
            $" {Employees.Count(e => !e.JoiningDate.HasValue)} employee records have no joining date; {Employees.Count(e => e.StatusId is 126 or 127 && !e.ExitDate.HasValue)} exit records have no exit date.";
    }
    public async Task<HrDashboardResponse> GetAsync(int userId, int clientId, int? entityId, int? departmentId,
        DateTime from, DateTime to, CancellationToken ct)
    {
        Validate(from, to, entityId, departmentId);
        var source = await LoadAsync(userId, clientId, entityId, departmentId, ct);
        return new(source.Entities, source.Departments.Where(d => d.Id > 0).ToList(), source.Metrics.Summarize(from.Date, to.Date, departmentId), source.Note);
    }
    public async Task<HrDetailResponse> DetailsAsync(int userId, int clientId, int? entityId, int? departmentId,
        DateTime from, DateTime to, string kind, int pageNumber, int pageSize, string? filters, CancellationToken ct)
    {
        Validate(from, to, entityId, departmentId);
        if (pageNumber < 1 || pageNumber > 100000 || pageSize < 1 || pageSize > 100) throw new ArgumentException("Invalid page; page size must be between 1 and 100.");
        if (kind is not ("staff" or "cohort" or "retained" or "joiners" or "exits" or "trend" or "positions" or "comparison")) throw new ArgumentException("Unknown detail kind.");
        var source = await LoadAsync(userId, clientId, entityId, departmentId, ct);
        IEnumerable<object> rows = kind switch {
            "positions" => source.Positions.Where(p => !departmentId.HasValue || p.DepartmentId == departmentId).OrderBy(p => p.PositionNumber).Cast<object>(),
            "comparison" => source.Metrics.Summarize(from.Date, to.Date, departmentId).Comparison.Cast<object>(),
            _ => source.Metrics.Details(kind, from.Date, to.Date, departmentId).OrderByDescending(e => e.Date).ThenBy(e => e.Id).Cast<object>()
        };
        if (!string.IsNullOrWhiteSpace(filters))
        {
            if (filters.Length > 4000) throw new ArgumentException("Detail filters are too long.");
            Dictionary<string, string>? values;
            try { values = JsonSerializer.Deserialize<Dictionary<string, string>>(filters); }
            catch (JsonException) { throw new ArgumentException("Invalid detail filters."); }
            if (values?.Count > 20) throw new ArgumentException("Too many detail filters.");
            // Filter rendered columns on the server before counting/paging. No filter is interpolated into SQL.
            foreach (var filter in values ?? [])
            {
                var property = (kind == "positions" ? typeof(HrOpenPosition) : kind == "comparison" ? typeof(HrComparison) : typeof(HrEmployeeDetail))
                    .GetProperties().FirstOrDefault(p => string.Equals(p.Name, filter.Key, StringComparison.OrdinalIgnoreCase));
                if (property == null || filter.Value == null || filter.Value.Length > 200) throw new ArgumentException("Invalid detail filter.");
                rows = rows.Where(row => Convert.ToString(property.GetValue(row), System.Globalization.CultureInfo.InvariantCulture)?.Contains(filter.Value, StringComparison.OrdinalIgnoreCase) == true);
            }
        }
        var result = rows.ToList();
        return new(result.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToArray(), result.Count, pageNumber, pageSize, source.Note);
    }
    public static void Validate(DateTime from, DateTime to, int? entityId, int? departmentId)
    {
        if (from.Date > to.Date || from.Year < 1903 || to.Year > 9995 || (to.Date - from.Date).Days > 1096)
            throw new ArgumentException("Select a valid date range of no more than three years.");
        if (entityId is <= 0 || departmentId is <= 0) throw new ArgumentException("Campus and department IDs must be positive when provided.");
    }
    private async Task<Source> LoadAsync(int userId, int clientId, int? entityId, int? departmentId, CancellationToken ct)
    {
        var permitted = await organization.OrgLegalentities.Where(x => x.ClientId == clientId && !x.UtBlock).Select(x => x.Id).ToListAsync(ct);
        var ids = string.Join(",", permitted);
        var entities = await Query<HrLookupRow>($"SELECT Id, COALESCE(stxt,shortname,entityno,'Campus') Name FROM dbo.OrgLegalentity WHERE clientid=@client AND ISNULL(utblock,0)=0 AND Id IN ({ids}) ORDER BY stxt");
        if (entities.Count == 0 || entityId.HasValue && entities.All(e => e.Id != entityId)) throw new UnauthorizedAccessException();
        var scopeIds = string.Join(",", entities.Where(e => !entityId.HasValue || e.Id == entityId).Select(e => e.Id));
        var departmentRows = await Query<HrLookupRow>("SELECT Id, COALESCE(depstxt,depcode,'Department') Name FROM dbo.OrgDepartment WHERE clientid=@client AND ISNULL(utblock,0)=0 ORDER BY depstxt");
        var employees = await Query<HrEmployee>($"""
            SELECT e.Id, COALESCE(e.employeecode,'') EmployeeCode, LTRIM(RTRIM(COALESCE(e.firstname,'')+' '+COALESCE(e.lastname,''))) Employee,
              e.entityid EntityId, COALESCE(c.stxt,c.entityno,'Campus') CampusName, ISNULL(e.depid,0) DepartmentId,
              CASE e.lvemployeecateg WHEN 269 THEN 'Teachers' WHEN 267 THEN 'Admin Staff' WHEN 268 THEN 'Admin Staff' WHEN 571 THEN 'Support Staff' ELSE COALESCE(l.stxt,'Other') END StaffType,
              ISNULL(e.empstatusid,0) StatusId, e.joiningdate JoiningDate,
              CASE WHEN e.empstatusid IN (126,127) THEN e.resigndate END ExitDate
            FROM dbo.EREmployeeinfohdr e JOIN dbo.OrgLegalentity c ON c.Id=e.entityid AND c.clientid=@client
            LEFT JOIN dbo.lov l ON l.Id=e.lvemployeecateg
            WHERE e.clientid=@client AND e.entityid IN ({scopeIds}) AND ISNULL(e.utblock,0)=0
            """);
        var history = await Query<HrPositionHistory>($"""
            SELECT p.id Id, p.employeeid EmployeeId, ISNULL(p.deptid,0) DepartmentId, p.fromdate FromDate, p.tilldate TillDate
            FROM dbo.EREmpPosition p JOIN dbo.EREmployeeinfohdr e ON e.Id=p.employeeid
            WHERE e.clientid=@client AND e.entityid IN ({scopeIds}) AND ISNULL(e.utblock,0)=0 AND ISNULL(p.utblock,0)=0
            """);
        var available = (await Query<HrAvailability>("SELECT CAST(CASE WHEN OBJECT_ID('dbo.HRVacancy','U') IS NOT NULL AND OBJECT_ID('dbo.HRVacancyPosition','U') IS NOT NULL THEN 1 ELSE 0 END AS bit) Available"))[0].Available;
        var positions = available ? await Query<HrOpenPosition>($"""
            SELECT p.Id, p.PositionNumber, v.JobTitle Position, v.EntityId, COALESCE(c.stxt,c.entityno,'Campus') CampusName,
              v.DepartmentId, COALESCE(d.depstxt,'Unassigned') DepartmentName, p.Status
            FROM dbo.HRVacancyPosition p JOIN dbo.HRVacancy v ON v.Id=p.VacancyId
            JOIN dbo.OrgLegalentity c ON c.Id=v.EntityId AND c.clientid=@client
            LEFT JOIN dbo.OrgDepartment d ON d.Id=v.DepartmentId AND d.clientid=@client
            WHERE v.ClientId=@client AND v.EntityId IN ({scopeIds}) AND v.IsDeleted=0 AND v.UtBlock=0 AND p.UtBlock=0 AND p.Status='Vacant'
            """) : [];
        var lookups = departmentRows.Select(d => new HrLookup(d.Id, d.Name)).ToList();
        // Preserve historical/blocked or unassigned departments in grouped totals.
        foreach (var id in employees.Select(e => e.DepartmentId).Concat(history.Select(p => p.DepartmentId)).Distinct().Where(id => lookups.All(d => d.Id != id)))
            lookups.Add(new(id, id == 0 ? "Unassigned" : $"Department {id} (historical)"));
        if (departmentId.HasValue && lookups.All(d => d.Id != departmentId)) throw new ArgumentException("Department does not belong to the current client.");
        return new(entities.Select(e => new HrLookup(e.Id, e.Name)).ToList(), lookups, employees, history, positions, available);

        async Task<List<T>> Query<T>(string sql) where T : class => await organization.Database.SqlQueryRaw<T>(sql, new SqlParameter("@client", clientId)).ToListAsync(ct);
    }
    private sealed class HrLookupRow { public int Id { get; set; } public string Name { get; set; } = ""; }
    private sealed class HrAvailability { public bool Available { get; set; } }
}
