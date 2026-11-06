namespace Auth.Service.Services;

public sealed record HrLookup(int Id, string Name);
public sealed class HrEmployee
{
    public int Id { get; set; }
    public string EmployeeCode { get; set; } = "";
    public string Employee { get; set; } = "";
    public int EntityId { get; set; }
    public string CampusName { get; set; } = "";
    public int DepartmentId { get; set; }
    public string StaffType { get; set; } = "";
    public int StatusId { get; set; }
    public DateTime? JoiningDate { get; set; }
    public DateTime? ExitDate { get; set; }
}
public sealed class HrPositionHistory
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public int DepartmentId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? TillDate { get; set; }
}
public sealed class HrOpenPosition
{
    public int Id { get; set; }
    public string PositionNumber { get; set; } = "";
    public string Position { get; set; } = "";
    public int EntityId { get; set; }
    public string CampusName { get; set; } = "";
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; } = "";
    public string Status { get; set; } = "";
}
public sealed record HrMetricGroup(string Id, string Name, int Total, double AverageStaff, int Exits, double? Rate);
public sealed record HrMonth(string Name, int Joiners, int Exits, double? Rate);
public sealed record HrDistribution(string Name, int Value);
public sealed record HrComparison(string Name, string From, string To, int Joiners, int Exits, double? Rate);
public sealed record HrSummary(int Total, int OpeningStaff, double AverageStaff, int Joiners, int Exits, int Retained,
    int CohortExits, double? Rate, double? RetentionRate, double? PreviousRate, double? ChangePp, int? OpenPositions,
    List<HrMetricGroup> DepartmentRows, List<HrMetricGroup> CampusRows, List<HrMetricGroup> StaffTypeRows,
    List<HrMonth> Months, List<HrDistribution> Tenures, List<HrDistribution> Reasons, List<HrComparison> Comparison);
public sealed record HrDashboardResponse(List<HrLookup> Entities, List<HrLookup> Departments, HrSummary Summary, string DataNote);
public sealed record HrEmployeeDetail(string Id, string EmployeeCode, string Employee, string CampusName,
    string DepartmentName, string StaffType, string Status, string? JoiningDate, string? ExitDate, string? Date,
    string TenureName, string ReasonName);
public sealed record HrDetailResponse(object[] Rows, int TotalCount, int PageNumber, int PageSize, string DataNote);

/// <summary>One set of predicates supplies both card totals and drilldown membership.</summary>
public sealed class HrAttritionMetrics(IReadOnlyList<HrEmployee> employees, IReadOnlyList<HrPositionHistory> history,
    IReadOnlyList<HrLookup> departments, IReadOnlyList<HrOpenPosition> positions, bool positionsAvailable)
{
    private readonly ILookup<int, HrPositionHistory> positionsByEmployee = history.ToLookup(p => p.EmployeeId);
    private readonly Dictionary<int, string> departmentNames = departments.ToDictionary(d => d.Id, d => d.Name);
    public const string DataNote = "Attrition = exits / average opening and closing headcount. Retention tracks the starting cohort. " +
        "Dates are inclusive; an exit is effective on its recorded exit date. Only active (125) and dated exit (126/127) records with valid joining dates are used. " +
        "Exit reasons are not recorded. Campus and staff type reflect current employee records; department uses effective contract history when available. Rehire history is not reconstructed from the employee header.";
    private bool Usable(HrEmployee e) => e.JoiningDate.HasValue && (e.StatusId == 125 || e.ExitDate.HasValue)
        && (!e.ExitDate.HasValue || e.ExitDate.Value.Date >= e.JoiningDate.Value.Date);
    private bool Active(HrEmployee e, DateTime date) => Usable(e) && e.JoiningDate!.Value.Date <= date.Date
        && (!e.ExitDate.HasValue || e.ExitDate.Value.Date > date.Date);
    private bool InRange(DateTime? date, DateTime from, DateTime to) => date.HasValue && date.Value.Date >= from.Date && date.Value.Date <= to.Date;
    public int DepartmentAt(HrEmployee e, DateTime date) => positionsByEmployee[e.Id]
        .Where(p => p.FromDate.HasValue && p.FromDate.Value.Date <= date.Date && (!p.TillDate.HasValue || p.TillDate.Value.Date >= date.Date))
        .OrderByDescending(p => p.FromDate).ThenByDescending(p => p.Id).Select(p => (int?)p.DepartmentId).FirstOrDefault() ?? e.DepartmentId;
    private bool DepartmentMatches(HrEmployee e, DateTime date, int? departmentId) => !departmentId.HasValue || DepartmentAt(e, date) == departmentId;
    private List<HrEmployee> At(DateTime date, int? departmentId) => employees.Where(e => Active(e, date) && DepartmentMatches(e, date, departmentId)).ToList();
    private List<HrEmployee> Joined(DateTime from, DateTime to, int? departmentId) => employees.Where(e => Usable(e) && InRange(e.JoiningDate, from, to) && DepartmentMatches(e, e.JoiningDate!.Value, departmentId)).ToList();
    private List<HrEmployee> Left(DateTime from, DateTime to, int? departmentId) => employees.Where(e => Usable(e) && InRange(e.ExitDate, from, to) && DepartmentMatches(e, e.ExitDate!.Value, departmentId)).ToList();
    public static double? Percentage(int count, double denominator) => denominator > 0 ? Math.Round(count / denominator * 100, 1) : null;
    private (int Opening, int Closing, double Average, int Exits, int Joiners, double? Rate) Counts(DateTime from, DateTime to, int? departmentId)
    {
        var opening = At(from.AddDays(-1), departmentId).Count;
        var closing = At(to, departmentId).Count;
        var average = (opening + closing) / 2d;
        var exits = Left(from, to, departmentId).Count;
        return (opening, closing, average, exits, Joined(from, to, departmentId).Count, Percentage(exits, average));
    }
    public HrSummary Summarize(DateTime from, DateTime to, int? departmentId)
    {
        var counts = Counts(from, to, departmentId);
        var starting = At(from.AddDays(-1), departmentId);
        var retained = starting.Count(e => Active(e, to));
        var previous = Counts(from.AddYears(-1), to.AddYears(-1), departmentId);
        var exits = Left(from, to, departmentId);
        var months = new List<HrMonth>();
        for (var month = new DateTime(from.Year, from.Month, 1); month <= to; month = month.AddMonths(1))
        {
            var start = month < from ? from : month;
            var end = month.AddMonths(1).AddDays(-1) > to ? to : month.AddMonths(1).AddDays(-1);
            var m = Counts(start, end, departmentId);
            months.Add(new(month.ToString("MMM yyyy", System.Globalization.CultureInfo.InvariantCulture), m.Joiners, m.Exits, m.Rate));
        }
        var comparisons = Enumerable.Range(-2, 3).Select(offset => {
            var start = from.AddYears(offset); var end = to.AddYears(offset); var c = Counts(start, end, departmentId);
            return new HrComparison(start.ToString("dd MMM yyyy") + " - " + end.ToString("dd MMM yyyy"), start.ToString("yyyy-MM-dd"), end.ToString("yyyy-MM-dd"), c.Joiners, c.Exits, c.Rate);
        }).ToList();
        var groups = new List<HrMetricGroup>();
        foreach (var d in departments)
        {
            if (departmentId.HasValue && departmentId != d.Id) continue;
            var c = Counts(from, to, d.Id);
            if (c.Opening + c.Closing + c.Exits + c.Joiners > 0) groups.Add(new(d.Id.ToString(), d.Name, c.Closing, c.Average, c.Exits, c.Rate));
        }
        var campusRows = employees.GroupBy(e => new { e.EntityId, e.CampusName }).Select(g => Group(g.Key.EntityId.ToString(), g.Key.CampusName, g.ToList())).Where(g => g.Total + g.Exits + g.AverageStaff > 0).ToList();
        var staffRows = employees.GroupBy(e => e.StaffType).Select(g => Group(g.Key, g.Key, g.ToList())).Where(g => g.Total + g.Exits + g.AverageStaff > 0).ToList();
        var tenureNames = new[] { "< 1 Year", "1 - 3 Years", "3 - 5 Years", "> 5 Years", "Unknown" };
        return new(counts.Closing, counts.Opening, counts.Average, counts.Joiners, counts.Exits, retained, starting.Count - retained,
            counts.Rate, Percentage(retained, starting.Count), previous.Rate,
            counts.Rate.HasValue && previous.Rate.HasValue ? Math.Round(counts.Rate.Value - previous.Rate.Value, 1) : null,
            positionsAvailable ? positions.Count(p => !departmentId.HasValue || p.DepartmentId == departmentId) : null,
            groups, campusRows, staffRows, months, tenureNames.Select((name, i) => new HrDistribution(name, exits.Count(e => TenureIndex(e) == i))).ToList(),
            exits.Count == 0 ? [] : [new("Not recorded", exits.Count)], comparisons);

        HrMetricGroup Group(string id, string name, List<HrEmployee> members)
        {
            var c = new HrAttritionMetrics(members, history, departments, [], true).Counts(from, to, departmentId);
            return new(id, name, c.Closing, c.Average, c.Exits, c.Rate);
        }
    }
    private static int TenureIndex(HrEmployee e)
    {
        if (!e.JoiningDate.HasValue || !e.ExitDate.HasValue || e.ExitDate < e.JoiningDate) return 4;
        var joined = e.JoiningDate.Value.Date; var left = e.ExitDate.Value.Date;
        return left < joined.AddYears(1) ? 0 : left < joined.AddYears(3) ? 1 : left <= joined.AddYears(5) ? 2 : 3;
    }
    public IEnumerable<HrEmployeeDetail> Details(string kind, DateTime from, DateTime to, int? departmentId)
    {
        var opening = At(from.AddDays(-1), departmentId);
        return kind switch {
            "staff" => At(to, departmentId).Select(e => Detail(e, "Active", to)),
            "cohort" => opening.Select(e => Detail(e, Active(e, to) ? "Retained" : "Exited", Active(e, to) ? to : e.ExitDate!.Value)),
            "retained" => opening.Where(e => Active(e, to)).Select(e => Detail(e, "Retained", to)),
            "joiners" => Joined(from, to, departmentId).Select(e => Detail(e, "New Joiner", e.JoiningDate!.Value)),
            "exits" => Left(from, to, departmentId).Select(e => Detail(e, "Exited", e.ExitDate!.Value)),
            "trend" => Joined(from, to, departmentId).Select(e => Detail(e, "New Joiner", e.JoiningDate!.Value))
                .Concat(Left(from, to, departmentId).Select(e => Detail(e, "Exited", e.ExitDate!.Value))),
            _ => throw new ArgumentException("Unknown detail kind.")
        };
    }
    private HrEmployeeDetail Detail(HrEmployee e, string status, DateTime date) => new(
        $"{e.Id}-{status}", e.EmployeeCode, e.Employee, e.CampusName, departmentNames.GetValueOrDefault(DepartmentAt(e, date), "Unassigned"), e.StaffType, status,
        e.JoiningDate?.ToString("yyyy-MM-dd"), e.ExitDate?.ToString("yyyy-MM-dd"), date.ToString("yyyy-MM-dd"),
        new[] { "< 1 Year", "1 - 3 Years", "3 - 5 Years", "> 5 Years", "Unknown" }[TenureIndex(e)], "Not recorded");
}
