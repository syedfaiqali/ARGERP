using HumanResource.Service.DTOs;
using HumanResource.Service.Entities;
using HumanResource.Service.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.Service.Services;

// One calculation for fetched, saved and monthly attendance, including legacy rows
// whose holiday flags or late counts were never populated.
public sealed class AttendanceShiftRules
{
    private List<EmployeeRosterLegacy> _rosters = new();
    private List<ShiftLegacy> _shifts = new();
    private List<ETShiftdtlLegacy> _details = new();
    private List<SmShiftLegacy> _legacyShifts = new();
    private List<ETPolicyhdrLegacy> _policies = new();

    public static async Task<AttendanceShiftRules> LoadAsync(HumanResourceDbContext db, int clientId,
        IEnumerable<int> employeeIds, DateTime from, DateTime till, CancellationToken ct)
    {
        var ids = employeeIds.Distinct().ToList();
        var rules = new AttendanceShiftRules();
        if (ids.Count == 0) return rules;
        rules._rosters = await db.EmployeeRosters.AsNoTracking().Where(x =>
            x.EmployeeId.HasValue && ids.Contains(x.EmployeeId.Value) && x.UtBlock != true &&
            x.StartDate.HasValue && x.StartDate.Value.Date <= till.Date &&
            (!x.EndDate.HasValue || x.EndDate.Value.Date >= from.Date)).ToListAsync(ct);
        rules._shifts = await db.Shifts.AsNoTracking()
            .Where(x => x.ClientId == clientId && x.UtBlock != true).ToListAsync(ct);
        var shiftIds = rules._shifts.Select(x => x.Id).ToList();
        rules._details = await db.ShiftDetails.AsNoTracking()
            .Where(x => x.ShiftId.HasValue && shiftIds.Contains(x.ShiftId.Value) && x.UtBlock != true).ToListAsync(ct);
        rules._legacyShifts = await db.SmShifts.AsNoTracking()
            .Where(x => x.ClientId == clientId && !x.UtBlock).ToListAsync(ct);
        var policyIds = rules._shifts.Where(x => x.LateArrivPolicyId.HasValue)
            .Select(x => x.LateArrivPolicyId!.Value).Distinct().ToList();
        rules._policies = await db.AttendancePolicies.AsNoTracking()
            .Include(x => x.PolicyDetails).Include(x => x.AbsentDetails)
            .Where(x => policyIds.Contains(x.Id) && x.ClientId == clientId && !x.Utblock).ToListAsync(ct);
        return rules;
    }

    private EmployeeRosterLegacy? Roster(int employeeId, DateTime date) => _rosters
        .Where(x => x.EmployeeId == employeeId && x.StartDate!.Value.Date <= date.Date &&
            (!x.EndDate.HasValue || x.EndDate.Value.Date >= date.Date))
        .OrderByDescending(x => x.StartDate).ThenByDescending(x => x.Id).FirstOrDefault();

    public int? LatePolicyId(int employeeId, DateTime date, int? shiftId) =>
        _shifts.FirstOrDefault(x => x.Id == (Roster(employeeId, date)?.ShiftId ?? shiftId))?.LateArrivPolicyId;

    public double LateAbsences(int? policyId, double count) => _policies.FirstOrDefault(x => x.Id == policyId)?
        .AbsentDetails.Where(x => !x.Utblock && x.Policycount > 0 && count >= x.Policycount)
        .OrderByDescending(x => x.Policycount).Select(x => x.Absentcount ?? 0).FirstOrDefault() ?? 0;

    public EmployeeDailyAttendanceV3Dto Apply(EmployeeDailyAttendanceV3Dto row)
    {
        if (!row.EmployeeId.HasValue || !row.AtnDate.HasValue) return row;
        var date = row.AtnDate.Value.Date;
        var roster = Roster(row.EmployeeId.Value, date);
        var shiftId = roster?.ShiftId ?? row.ShiftId;
        var shift = _shifts.FirstOrDefault(x => x.Id == shiftId);
        var day = date.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)date.DayOfWeek;
        var detail = _details.Where(x => x.ShiftId == shiftId && (x.DayId == day || x.DayId == day + 99) &&
                (!x.StartDate.HasValue || x.StartDate.Value.Date <= date) &&
                (!x.EndDate.HasValue || x.EndDate.Value.Date >= date))
            .OrderByDescending(x => x.StartDate).ThenByDescending(x => x.Id).FirstOrDefault();
        var legacy = _legacyShifts.Where(x => x.Id == shiftId ||
                (shift != null && x.EntityId == shift.EntityId &&
                 ((!string.IsNullOrWhiteSpace(shift.Code) && x.Code == shift.Code) ||
                  (!string.IsNullOrWhiteSpace(shift.Stxt) && x.Stxt == shift.Stxt))))
            .OrderByDescending(x => x.Id == shiftId).FirstOrDefault();
        var offDay = date.DayOfWeek switch
        {
            DayOfWeek.Monday => shift?.WoMonday ?? legacy?.WoMonday,
            DayOfWeek.Tuesday => shift?.WoTuesday ?? legacy?.WoTuesday,
            DayOfWeek.Wednesday => shift?.WoWednesday ?? legacy?.WoWednesday,
            DayOfWeek.Thursday => shift?.WoThuresday ?? legacy?.WoThuresday,
            DayOfWeek.Friday => shift?.WoFriday ?? legacy?.WoFriday,
            DayOfWeek.Saturday => shift?.WoSaturday ?? legacy?.WoSaturday,
            _ => shift?.WoSunday ?? legacy?.WoSunday
        };
        var holiday = row.IsHoliday || offDay == true;
        var scheduledIn = detail?.InTime ?? legacy?.StartTime ?? row.SchTimeIn;
        var scheduledOut = detail?.OutTime ?? legacy?.EndTime ?? row.SchTimeOut;
        var minutes = scheduledIn.HasValue && row.ActShiftIn.HasValue
            ? Math.Max(0, (int)(row.ActShiftIn.Value.TimeOfDay - scheduledIn.Value.TimeOfDay).TotalMinutes) : 0;
        var policy = _policies.FirstOrDefault(x => x.Id == shift?.LateArrivPolicyId);
        // The shift's assigned policy governs late attendance. Older Shift screens
        // saved LatePolicyApplicable=false for every detail, even with a policy selected.
        var applies = roster?.LateAllowed != true &&
            (!holiday || policy?.Applyoffdays == true) && minutes > (shift?.LateArrivPolicyMnt ?? detail?.GraceTime ?? legacy?.GraceTime ?? 0);
        int Penalty(int type) => !applies ? 0 : Convert.ToInt32(policy?.PolicyDetails
            .Where(x => !x.Utblock && x.Valtypeid == type && minutes >= (x.Starttime ?? 0) &&
                (!x.Endtime.HasValue || minutes <= x.Endtime.Value))
            // Keep the missing rule nullable: an applicable late day still counts
            // once when its minutes fall outside the policy's Count ranges.
            .OrderByDescending(x => x.Starttime).Select(x => x.Value).FirstOrDefault() ??
            (type == 249 ? 1 : 0));
        return row with
        {
            ShiftId = shiftId, ShiftDtlId = detail?.Id ?? row.ShiftDtlId,
            DayId = detail?.DayId ?? day, SchTimeIn = scheduledIn, SchTimeOut = scheduledOut,
            IsHoliday = holiday, IsAbsent = !holiday && row.ActShiftIn == null && row.ActShiftOut == null,
            LateMin = minutes, LateCount = Penalty(249), LateHalfDays = Penalty(250),
            LateAbsent = Penalty(251), LateExcuse = Penalty(248)
        };
    }

    public void Apply(HRDailyAttendanceLegacy entity)
    {
        var row = Apply(new EmployeeDailyAttendanceV3Dto(entity.Id, entity.EmployeeId, "", "", entity.AtnDate, "",
            entity.SchTimeIn, entity.SchTimeOut, entity.ActShiftIn, entity.ActShitOut,
            entity.LateMin, entity.LateCount, entity.LateHalfDays, entity.LateAbsent, entity.LateExcuse,
            entity.EdMin, entity.EdCount, entity.EdHalfDays, entity.EdAbsent, entity.EdExcuse,
            entity.OtMin, entity.OtCount, entity.OtHalfDays, entity.Remarks ?? "", entity.IsAbsent, entity.IsHoliday,
            entity.ShiftId, entity.ShiftDtlId, entity.DayId, entity.DepId, entity.DesignationId));
        entity.ShiftId = row.ShiftId; entity.ShiftDtlId = row.ShiftDtlId; entity.DayId = row.DayId;
        entity.SchTimeIn = row.SchTimeIn; entity.SchTimeOut = row.SchTimeOut;
        entity.IsHoliday = row.IsHoliday; entity.IsAbsent = row.IsAbsent;
        entity.LateMin = row.LateMin; entity.LateCount = row.LateCount; entity.LateHalfDays = row.LateHalfDays;
        entity.LateAbsent = row.LateAbsent; entity.LateExcuse = row.LateExcuse;
    }
}
