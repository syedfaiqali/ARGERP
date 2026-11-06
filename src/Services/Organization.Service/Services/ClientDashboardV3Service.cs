using Microsoft.EntityFrameworkCore;
using Organization.Service.DTOs;
using Organization.Service.Infrastructure.Data;
using System.Globalization;

namespace Organization.Service.Services
{
    public sealed class ClientDashboardV3Service : IClientDashboardV3Service
    {
        private const int StudentStatusContinue = 357;
        private const int StudentStatusPassOut = 1120;
        private const int GenderBoy = 337;
        private const int GenderGirl = 338;
        private const int EmployeeStatusActive = 125;
        private const int OrganizationStatusOperational = 561;
        private const int OrganizationStatusUnderEstablish = 562;
        private const int OrganizationStatusSuspend = 563;

        private readonly OrganizationDbContext _db;
        private readonly IDbContextFactory<OrganizationDbContext>? _dbContextFactory;
        private readonly ILogger<ClientDashboardV3Service> _logger;

        public ClientDashboardV3Service(OrganizationDbContext db, ILogger<ClientDashboardV3Service> logger)
        {
            _db = db;
            _logger = logger;
        }

        public ClientDashboardV3Service(
            OrganizationDbContext db,
            IDbContextFactory<OrganizationDbContext> dbContextFactory,
            ILogger<ClientDashboardV3Service> logger) : this(db, logger)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<CampusDashboardDto> GetDashboardAsync(int clientId, int? selectedEntityId, IReadOnlyCollection<int> allowedEntityIds, int duration, CancellationToken cancellationToken = default)
        {
            duration = duration <= 0 ? 3 : Math.Min(duration, 24);
            var allEntities = await GetEntitiesAsync(clientId, cancellationToken);
            var entities = allEntities.Where(x => allowedEntityIds.Contains(x.Id)).ToList();
            if (selectedEntityId is > 0 && entities.All(x => x.Id != selectedEntityId.Value))
            {
                throw new UnauthorizedAccessException("The requested campus is outside the permitted scope.");
            }

            var scopedEntityIds = selectedEntityId is > 0
                ? new List<int> { selectedEntityId.Value }
                : entities.Select(x => x.Id).ToList();

            // Each section is read-only and independent. A DbContext cannot run concurrent
            // operations, so each task receives its own factory-created context.
            var summaryTask = BuildSectionAsync(x => x.BuildSummaryAsync(clientId, scopedEntityIds, cancellationToken), new CampusSummaryDto(), "summary", cancellationToken);
            var enrollmentTask = BuildSectionAsync(x => x.BuildEnrollmentAsync(clientId, scopedEntityIds, duration, cancellationToken), new List<CampusEnrollmentPointDto>(), "enrollment", cancellationToken);
            var admissionTask = BuildSectionAsync(x => x.BuildAdmissionAsync(clientId, scopedEntityIds, duration, cancellationToken), new List<CampusAdmissionPointDto>(), "admission", cancellationToken);
            var campusTypesTask = BuildSectionAsync(x => x.BuildCampusTypesAsync(clientId, scopedEntityIds, cancellationToken), new List<CampusTypeStatusDto>(), "campus types", cancellationToken);
            var courseWiseTask = BuildSectionAsync(x => x.BuildCourseWiseAsync(clientId, scopedEntityIds, cancellationToken), new List<CampusRegionCourseDto>(), "course-wise data", cancellationToken);
            var campusBreakdownTask = BuildSectionAsync(x => x.BuildCampusBreakdownAsync(clientId, entities, scopedEntityIds, duration, cancellationToken), new List<CampusDashboardBreakdownDto>(), "campus breakdown", cancellationToken);
            var capacityTask = BuildSectionAsync(x => x.BuildCapacityAsync(clientId, scopedEntityIds, cancellationToken), new CampusCapacityDto(), "capacity", cancellationToken);
            var attendanceTask = BuildSectionAsync(x => x.BuildAttendanceAsync(clientId, scopedEntityIds, cancellationToken), new ClientDashboardAttendanceDto(), "attendance", cancellationToken);
            var financeTask = BuildSectionAsync(x => x.BuildFinanceAsync(clientId, scopedEntityIds, duration, cancellationToken), new ClientDashboardFinanceDto(), "finance", cancellationToken);

            await Task.WhenAll(summaryTask, enrollmentTask, admissionTask, campusTypesTask, courseWiseTask, campusBreakdownTask, capacityTask, attendanceTask, financeTask);

            return new CampusDashboardDto
            {
                SelectedEntityId = selectedEntityId,
                Duration = duration,
                Durations = new List<int> { 3, 6, 12 },
                Entities = entities,
                Summary = await summaryTask,
                Enrollment = await enrollmentTask,
                Admission = await admissionTask,
                CampusTypes = await campusTypesTask,
                CourseWise = await courseWiseTask,
                CampusBreakdown = await campusBreakdownTask,
                Capacity = await capacityTask,
                Attendance = await attendanceTask,
                Finance = await financeTask
            };
        }

        private async Task<T> BuildSectionAsync<T>(Func<ClientDashboardV3Service, Task<T>> build, T fallback, string section, CancellationToken cancellationToken)
        {
            if (_dbContextFactory is null)
            {
                return await BuildSafelyAsync(() => build(this), fallback, section);
            }

            return await BuildSafelyAsync(async () =>
            {
                await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
                return await build(new ClientDashboardV3Service(context, _logger));
            }, fallback, section);
        }

        private async Task<T> BuildSafelyAsync<T>(Func<Task<T>> build, T fallback, string section)
        {
            try
            {
                return await build();
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Client dashboard {Section} section failed", section);
                return fallback;
            }
        }

        private async Task<ClientDashboardAttendanceDto> BuildAttendanceAsync(int clientId, IReadOnlyCollection<int> entityIds, CancellationToken cancellationToken)
        {
            var today = DateTime.Today;
            var activeStudents = await _db.CampusStudents.AsNoTracking().CountAsync(x => x.ClientId == clientId && x.StatusId == StudentStatusContinue && entityIds.Contains(x.EntityId ?? 0), cancellationToken);
            var marked = await _db.ClientStudentAttendance.AsNoTracking().Where(x => x.ClientId == clientId && x.AttendanceDate == today && !x.UtBlock && entityIds.Contains(x.EntityId ?? 0)).Select(x => x.StudentId).Distinct().CountAsync(cancellationToken);
            var staff = await _db.ClientEmployeeAttendance.AsNoTracking()
                .Where(x => x.AttendanceDate == today && !x.UtBlock && entityIds.Contains(x.EntityId ?? 0))
                .GroupBy(_ => 1)
                .Select(g => new { Present = g.Count(x => !x.IsAbsent), Absent = g.Count(x => x.IsAbsent) })
                .FirstOrDefaultAsync(cancellationToken);
            return new ClientDashboardAttendanceDto { StudentsMarked = marked, StudentPresent = marked, StudentNotMarked = Math.Max(0, activeStudents - marked), StaffPresent = staff?.Present ?? 0, StaffAbsent = staff?.Absent ?? 0 };
        }

        private async Task<ClientDashboardFinanceDto> BuildFinanceAsync(int clientId, IReadOnlyCollection<int> entityIds, int duration, CancellationToken cancellationToken)
        {
            var today = DateTime.Today;
            var startDate = new DateTime(today.AddMonths(-(duration - 1)).Year, today.AddMonths(-(duration - 1)).Month, 1);
            // These are the legacy aggregate queries. Use the legacy table names and
            // columns directly, because this dashboard reads both legacy fee and v3
            // organization tables from the same database.
            var scopedEntityIds = string.Join(",", entityIds.Where(id => id > 0).Distinct());
            if (string.IsNullOrWhiteSpace(scopedEntityIds)) return new ClientDashboardFinanceDto();
            // Aggregate total and today collection together.  The prior query scanned the
            // large collection-header table separately for each value, which dominated the
            // endpoint time on tenants with a long payment history.
            var feeTotalsSql = $@"
                SELECT
                    CAST(COALESCE(collectionTotals.Collection, 0) AS float) AS Collection,
                    CAST(COALESCE(collectionTotals.TodayCollection, 0) AS float) AS TodayCollection,
                    CAST(COALESCE(outstandingTotals.Outstanding, 0) AS float) AS Outstanding,
                    CAST(COALESCE(fineTotals.Fine, 0) AS float) AS Fine
                FROM (
                    SELECT
                        SUM(h.paidamount) AS Collection,
                        SUM(CASE WHEN h.transdate >= {{1}} AND h.transdate < {{2}} THEN h.paidamount ELSE 0 END) AS TodayCollection
                    FROM dbo.SMFeecollectionhdr h
                    WHERE h.clientid = {{0}} AND ISNULL(h.revcoll, 0) = 0 AND h.entityid IN ({scopedEntityIds})
                ) collectionTotals
                CROSS JOIN (
                    SELECT SUM(h.balanceamount) AS Outstanding
                    FROM dbo.SMFeechallanhdr h
                    WHERE h.clientid = {{0}} AND ISNULL(h.revchln, 0) = 0 AND h.entityid IN ({scopedEntityIds})
                ) outstandingTotals
                CROSS JOIN (
                    SELECT SUM(d.fineamount) AS Fine
                    FROM dbo.SMFeecollectiondtl d
                    INNER JOIN dbo.SMFeecollectionhdr h ON h.Id = d.transid
                    WHERE h.clientid = {{0}} AND ISNULL(h.revcoll, 0) = 0 AND h.entityid IN ({scopedEntityIds})
                ) fineTotals";
            var feeTotals = await _db.Database.SqlQueryRaw<LegacyFeeTotalsRow>(feeTotalsSql, clientId, today, today.AddDays(1))
                .SingleAsync(cancellationToken);

            double income = 0;
            double expense = 0;
            try
            {
                var ledger = await (from detail in _db.ClientFinancialTransactionDetails.AsNoTracking()
                                    join master in _db.ClientFinancialTransactions.AsNoTracking() on detail.TransactionId equals master.Id
                                    join account in _db.FicoaLegacy.AsNoTracking() on detail.AccountId equals account.Id
                                    where master.ClientId == clientId && master.UtBlock == false && detail.UtBlock == false && master.TransactionDate >= startDate && master.TransactionDate < today.AddDays(1) && entityIds.Contains(master.EntityId ?? 0)
                                    select new { account.AccountNatureId, Debit = detail.Debit ?? 0, Credit = detail.Credit ?? 0 }).ToListAsync(cancellationToken);
                income = ledger.Where(x => x.AccountNatureId == 1108).Sum(x => x.Credit - x.Debit);
                expense = ledger.Where(x => x.AccountNatureId == 1109).Sum(x => x.Debit - x.Credit);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Client dashboard Profit & Loss data failed for client {ClientId}", clientId);
            }

            return new ClientDashboardFinanceDto
            {
                Collection = feeTotals.Collection,
                TodayCollection = feeTotals.TodayCollection,
                Outstanding = feeTotals.Outstanding,
                Fine = feeTotals.Fine,
                Income = income,
                Expense = expense
            };
        }

        private sealed class LegacyFeeTotalsRow
        {
            public double Collection { get; set; }
            public double TodayCollection { get; set; }
            public double Outstanding { get; set; }
            public double Fine { get; set; }
        }

        private async Task<CampusCapacityDto> BuildCapacityAsync(int clientId, IReadOnlyCollection<int> entityIds, CancellationToken cancellationToken)
        {
            var rooms = await _db.OrgRoomLegacy.AsNoTracking()
                .Where(x => x.clientid == clientId && !x.utblock && entityIds.Contains(x.entityid ?? 0))
                .GroupBy(_ => 1)
                .Select(g => new { Classrooms = g.Count(), Seats = g.Sum(x => x.strength ?? 0) })
                .FirstOrDefaultAsync(cancellationToken);
            var learners = await _db.CampusStudents.AsNoTracking()
                .CountAsync(x => x.ClientId == clientId && x.StatusId == StudentStatusContinue && entityIds.Contains(x.EntityId ?? 0), cancellationToken);
            return new CampusCapacityDto { TotalClassrooms = rooms?.Classrooms ?? 0, SeatingCapacity = rooms?.Seats ?? 0, ActiveLearners = learners };
        }

        private async Task<List<CampusDashboardBreakdownDto>> BuildCampusBreakdownAsync(int clientId, IReadOnlyCollection<CampusLookupDto> entities, IReadOnlyCollection<int> entityIds, int duration, CancellationToken cancellationToken)
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            var fromDate = new DateTime(today.AddMonths(-(duration - 1)).Year, today.AddMonths(-(duration - 1)).Month, 1);

            var enquiries = await (
                from detail in _db.CampusAdmissionEnquiryDetails.AsNoTracking()
                join enquiry in _db.CampusAdmissionEnquiries.AsNoTracking() on detail.EnquiryId equals enquiry.Id
                join application in _db.CampusAdmissionApplications.AsNoTracking() on detail.Id equals application.EnquiryDetailId
                join student in _db.CampusStudents.AsNoTracking() on application.Id equals student.AppId into students
                from student in students.DefaultIfEmpty()
                where enquiry.ClientId == clientId && entityIds.Contains(enquiry.EntityId ?? 0)
                    && application.AppDate >= fromDate && application.AppDate < tomorrow
                select new { EntityId = enquiry.EntityId ?? 0, Registered = student != null && student.AppId.HasValue })
                .GroupBy(x => x.EntityId)
                .Select(g => new { EntityId = g.Key, Enquiries = g.Count(), Registrations = g.Count(x => x.Registered) })
                .ToListAsync(cancellationToken);

            var enrolled = await _db.CampusStudents.AsNoTracking()
                .Where(x => x.ClientId == clientId && entityIds.Contains(x.EntityId ?? 0) && x.RegDate.HasValue && x.RegDate.Value >= fromDate && x.RegDate.Value < tomorrow && x.StatusId == StudentStatusContinue)
                .GroupBy(x => x.EntityId ?? 0)
                .Select(g => new { EntityId = g.Key, Count = g.Count() })
                .ToListAsync(cancellationToken);

            var activeLearners = await _db.CampusStudents.AsNoTracking()
                .Where(x => x.ClientId == clientId && x.StatusId == StudentStatusContinue && entityIds.Contains(x.EntityId ?? 0))
                .GroupBy(x => x.EntityId ?? 0)
                .Select(g => new { EntityId = g.Key, Count = g.Count() })
                .ToListAsync(cancellationToken);

            var roomCounts = await _db.OrgRoomLegacy.AsNoTracking()
                .Where(x => x.clientid == clientId && !x.utblock && entityIds.Contains(x.entityid ?? 0))
                .GroupBy(x => x.entityid ?? 0)
                .Select(g => new { EntityId = g.Key, Classrooms = g.Count(), Seats = g.Sum(x => x.strength ?? 0) })
                .ToListAsync(cancellationToken);

            return entities.Where(entity => entityIds.Contains(entity.Id)).Select(entity => new CampusDashboardBreakdownDto
            {
                EntityId = entity.Id,
                Name = entity.Name,
                Enquiries = enquiries.Where(x => x.EntityId == entity.Id).Sum(x => x.Enquiries),
                Registrations = enquiries.Where(x => x.EntityId == entity.Id).Sum(x => x.Registrations),
                Enrolled = enrolled.Where(x => x.EntityId == entity.Id).Sum(x => x.Count),
                ActiveLearners = activeLearners.Where(x => x.EntityId == entity.Id).Sum(x => x.Count),
                Classrooms = roomCounts.Where(x => x.EntityId == entity.Id).Sum(x => x.Classrooms),
                SeatingCapacity = roomCounts.Where(x => x.EntityId == entity.Id).Sum(x => x.Seats)
            }).ToList();
        }
        private async Task<List<CampusLookupDto>> GetEntitiesAsync(int clientId, CancellationToken cancellationToken)
        {
            return await _db.OrgLegalentityLegacy
                .AsNoTracking()
                .Where(x => x.clientid == clientId && !x.utblock)
                .OrderBy(x => x.stxt)
                .Select(x => new CampusLookupDto
                {
                    Id = x.Id,
                    Name = x.stxt ?? x.shortname ?? x.entityno ?? $"Campus {x.Id}"
                })
                .ToListAsync(cancellationToken);
        }

        private async Task<CampusSummaryDto> BuildSummaryAsync(int clientId, IReadOnlyCollection<int> entityIds, CancellationToken cancellationToken)
        {
            var studentsQuery = _db.CampusStudents.AsNoTracking()
                .Where(x => x.ClientId == clientId && x.StatusId == StudentStatusContinue);
            studentsQuery = studentsQuery.Where(x => entityIds.Contains(x.EntityId ?? 0));

            var employeeQuery = _db.EmployeeinfohdrLegacy.AsNoTracking()
                .Where(x => x.clientid == clientId && x.empstatusid == EmployeeStatusActive && !x.utblock);
            employeeQuery = employeeQuery.Where(x => entityIds.Contains(x.entityid ?? 0));

            var entityQuery = _db.OrgLegalentityLegacy.AsNoTracking()
                .Where(x => x.clientid == clientId && !x.utblock);
            entityQuery = entityQuery.Where(x => entityIds.Contains(x.Id));

            var parentQuery =
                from student in _db.CampusStudents.AsNoTracking()
                join link in _db.CampusStudentGuardians.AsNoTracking() on student.Id equals link.StudentId
                join guardian in _db.CampusGuardians.AsNoTracking() on link.GuardianId equals guardian.Id
                where student.ClientId == clientId
                    && student.StatusId == StudentStatusContinue
                    && entityIds.Contains(student.EntityId ?? 0)
                    && guardian.ClientId == clientId
                select guardian.Id;

            var studentCounts = await studentsQuery
                .GroupBy(_ => 1)
                .Select(g => new
                {
                    Total = g.Count(),
                    Boys = g.Count(x => x.GenderId == GenderBoy),
                    Girls = g.Count(x => x.GenderId == GenderGirl)
                })
                .FirstOrDefaultAsync(cancellationToken);

            var entityRows = await entityQuery.Select(x => new { x.cityid, x.orgstatusid }).ToListAsync(cancellationToken);

            return new CampusSummaryDto
            {
                TotalStudents = studentCounts?.Total ?? 0,
                TotalStudentBoys = studentCounts?.Boys ?? 0,
                TotalStudentGirls = studentCounts?.Girls ?? 0,
                TotalTeacher = await employeeQuery.CountAsync(cancellationToken),
                TotalParent = await parentQuery.Distinct().CountAsync(cancellationToken),
                TotalCampus = entityRows.Count,
                TotalCity = entityRows.Where(x => x.cityid.HasValue).Select(x => x.cityid).Distinct().Count(),
                Operational = entityRows.Count(x => x.orgstatusid == OrganizationStatusOperational),
                Establish = entityRows.Count(x => x.orgstatusid == OrganizationStatusUnderEstablish),
                Suspended = entityRows.Count(x => x.orgstatusid == OrganizationStatusSuspend)
            };
        }

        private async Task<List<CampusEnrollmentPointDto>> BuildEnrollmentAsync(int clientId, IReadOnlyCollection<int> entityIds, int duration, CancellationToken cancellationToken)
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            var fromDate = new DateTime(today.AddMonths(-(duration - 1)).Year, today.AddMonths(-(duration - 1)).Month, 1);

            var studentQuery = _db.CampusStudents.AsNoTracking()
                .Where(x => x.ClientId == clientId && x.RegDate.HasValue && x.RegDate.Value >= fromDate && x.RegDate.Value < tomorrow && x.StatusId == StudentStatusContinue);
            studentQuery = studentQuery.Where(x => entityIds.Contains(x.EntityId ?? 0));

            var withdrawalQuery =
                from withdrawal in _db.CampusStudentWithdrawals.AsNoTracking()
                join student in _db.CampusStudents.AsNoTracking() on withdrawal.StudentId equals student.Id
                where student.ClientId == clientId && withdrawal.LeavingDate >= fromDate && withdrawal.LeavingDate < tomorrow
                select withdrawal;
            withdrawalQuery = withdrawalQuery.Where(x => entityIds.Contains(x.EntityId ?? 0));

            var enrolled = await studentQuery
                .GroupBy(x => new { x.RegDate!.Value.Year, x.RegDate!.Value.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
                .ToListAsync(cancellationToken);

            var withdrawals = await withdrawalQuery
                .GroupBy(x => new { x.LeavingDate!.Value.Year, x.LeavingDate!.Value.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    Withdrawl = g.Count(),
                    Passout = g.Count(x => x.StatusId == StudentStatusPassOut)
                })
                .ToListAsync(cancellationToken);

            return BuildMonthRange(fromDate, duration)
                .Select(month => new CampusEnrollmentPointDto
                {
                    Name = month.ToString("MMM-yyyy", CultureInfo.InvariantCulture),
                    Enrolled = enrolled.Where(x => x.Year == month.Year && x.Month == month.Month).Sum(x => x.Count),
                    Withdrawl = withdrawals.Where(x => x.Year == month.Year && x.Month == month.Month).Sum(x => x.Withdrawl),
                    Passout = withdrawals.Where(x => x.Year == month.Year && x.Month == month.Month).Sum(x => x.Passout)
                })
                .ToList();
        }

        private async Task<List<CampusAdmissionPointDto>> BuildAdmissionAsync(int clientId, IReadOnlyCollection<int> entityIds, int duration, CancellationToken cancellationToken)
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            var fromDate = new DateTime(today.AddMonths(-(duration - 1)).Year, today.AddMonths(-(duration - 1)).Month, 1);

            var rows = await (
                from detail in _db.CampusAdmissionEnquiryDetails.AsNoTracking()
                join enquiry in _db.CampusAdmissionEnquiries.AsNoTracking() on detail.EnquiryId equals enquiry.Id
                join application in _db.CampusAdmissionApplications.AsNoTracking() on detail.Id equals application.EnquiryDetailId
                join student in _db.CampusStudents.AsNoTracking() on application.Id equals student.AppId into students
                from student in students.DefaultIfEmpty()
                where enquiry.ClientId == clientId && entityIds.Contains(enquiry.EntityId ?? 0) && application.AppDate >= fromDate && application.AppDate < tomorrow
                select new
                {
                    application.AppDate!.Value.Year,
                    application.AppDate!.Value.Month,
                    Converted = student != null && student.AppId.HasValue
                })
                .GroupBy(x => new { x.Year, x.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Enquiry = g.Count(), Admission = g.Count(x => x.Converted) })
                .ToListAsync(cancellationToken);

            return BuildMonthRange(fromDate, duration)
                .Select(month => new CampusAdmissionPointDto
                {
                    Name = month.ToString("MMM-yyyy", CultureInfo.InvariantCulture),
                    Enquiry = rows.Where(x => x.Year == month.Year && x.Month == month.Month).Sum(x => x.Enquiry),
                    Admission = rows.Where(x => x.Year == month.Year && x.Month == month.Month).Sum(x => x.Admission)
                })
                .ToList();
        }

        private async Task<List<CampusTypeStatusDto>> BuildCampusTypesAsync(int clientId, IReadOnlyCollection<int> entityIds, CancellationToken cancellationToken)
        {
            var rows = await (
                from campus in _db.OrgLegalentityLegacy.AsNoTracking()
                join type in _db.Lovs.AsNoTracking() on campus.lovcampustypeid equals type.Id into types
                from type in types.DefaultIfEmpty()
                where campus.clientid == clientId
                    && !campus.utblock
                    && entityIds.Contains(campus.Id)
                select new
                {
                    campus.lovcampustypeid,
                    Type = type != null ? type.stxt : "Unspecified",
                    campus.orgstatusid
                }).ToListAsync(cancellationToken);

            return rows
                .GroupBy(x => new { x.lovcampustypeid, x.Type })
                .Select(g => new CampusTypeStatusDto
                {
                    Type = g.Key.Type ?? "Unspecified",
                    Operational = g.Count(x => x.orgstatusid == OrganizationStatusOperational),
                    UnderEstablish = g.Count(x => x.orgstatusid == OrganizationStatusUnderEstablish),
                    Suspend = g.Count(x => x.orgstatusid == OrganizationStatusSuspend)
                })
                .OrderBy(x => x.Type)
                .ToList();
        }

        private async Task<List<CampusRegionCourseDto>> BuildCourseWiseAsync(int clientId, IReadOnlyCollection<int> entityIds, CancellationToken cancellationToken)
        {
            var regionCourses = await (
                from region in _db.OrgRegion.AsNoTracking()
                join entity in _db.OrgLegalentityLegacy.AsNoTracking() on region.Id equals entity.regionid
                join courseExtend in _db.CampusCourseExtends.AsNoTracking() on entity.Id equals courseExtend.EntityId
                join course in _db.CampusCourses.AsNoTracking() on courseExtend.CourseId equals course.Id
                where entity.clientid == clientId
                    && !entity.utblock
                    && entityIds.Contains(entity.Id)
                select new
                {
                    RegionId = (int?)region.Id,
                    Region = region.stxt,
                    CourseId = courseExtend.CourseId,
                    Course = course.Name,
                    CourseIndex = course.Index ?? course.Id
                }).Distinct().ToListAsync(cancellationToken);

            var studentCounts = await (
                from student in _db.CampusStudents.AsNoTracking()
                join entity in _db.OrgLegalentityLegacy.AsNoTracking() on student.EntityId equals entity.Id
                where student.ClientId == clientId
                    && entityIds.Contains(student.EntityId ?? 0)
                group student by new { RegionId = entity.regionid, CourseId = student.CurrentCourseId } into grouped
                select new
                {
                    grouped.Key.RegionId,
                    grouped.Key.CourseId,
                    Enrolled = grouped.Count(x => x.StatusId == StudentStatusContinue),
                    Withdrawl = grouped.Count(x => x.StatusId != StudentStatusContinue)
                }).ToListAsync(cancellationToken);

            var combined = regionCourses
                .Select(row =>
                {
                    var matching = studentCounts.FirstOrDefault(x => x.RegionId == row.RegionId && x.CourseId == row.CourseId);
                    return new
                    {
                        row.RegionId,
                        row.Region,
                        row.CourseId,
                        row.Course,
                        row.CourseIndex,
                        Enrolled = matching?.Enrolled ?? 0,
                        Withdrawl = matching?.Withdrawl ?? 0
                    };
                })
                .OrderBy(x => x.Region)
                .ThenBy(x => x.CourseIndex)
                .ToList();

            return combined
                .GroupBy(x => new { x.RegionId, x.Region })
                .Select(g => new CampusRegionCourseDto
                {
                    RegionId = g.Key.RegionId,
                    Region = g.Key.Region ?? "Region",
                    TotalEnrolled = g.Sum(x => x.Enrolled),
                    TotalWithdrawl = g.Sum(x => x.Withdrawl),
                    Courses = g.Select(x => new CampusCourseSummaryDto
                    {
                        CourseId = x.CourseId,
                        Course = x.Course ?? string.Empty,
                        Enrolled = x.Enrolled,
                        Withdrawl = x.Withdrawl
                    }).ToList()
                })
                .ToList();
        }

        private static List<int> ParseEntityIds(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return new List<int>();
            return value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(item => int.TryParse(item, out var id) ? id : 0)
                .Where(id => id > 0)
                .Distinct()
                .ToList();
        }
        private static IEnumerable<DateTime> BuildMonthRange(DateTime from, int duration)
        {
            return Enumerable.Range(0, duration).Select(offset => from.AddMonths(offset));
        }

    }
}
