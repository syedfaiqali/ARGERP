using System.Security.Cryptography;
using System.Text;
using HumanResource.Service.Entities;
using HumanResource.Service.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace HumanResource.Service.Services
{
    public interface IZkAttendanceSyncService
    {
        Task<ZkAttendanceSyncResult> SyncAttendanceAsync(CancellationToken stoppingToken);
    }

    public sealed record ZkAttendanceSyncResult(int SyncedCount, int SkippedCount);

    public sealed class ZkAttendanceSyncOptions
    {
        public bool Enabled { get; set; } = true;
        public string ConnectionString { get; set; } = string.Empty;
        public int IntervalMinutes { get; set; } = 5;
    }

    public sealed class ZkAttendanceSyncService : IZkAttendanceSyncService
    {
        private static readonly SemaphoreSlim SyncLock = new(1, 1);
        private readonly ILogger<ZkAttendanceSyncService> _logger;
        private readonly HumanResourceDbContext _dbContext;
        private readonly ZkAttendanceSyncOptions _options;

        public ZkAttendanceSyncService(
            ILogger<ZkAttendanceSyncService> logger,
            HumanResourceDbContext dbContext,
            IOptions<ZkAttendanceSyncOptions> options)
        {
            _logger = logger;
            _dbContext = dbContext;
            _options = options.Value;
        }

        public async Task<ZkAttendanceSyncResult> SyncAttendanceAsync(CancellationToken stoppingToken)
        {
            await SyncLock.WaitAsync(stoppingToken);
            try
            {
                return await SyncAttendanceCoreAsync(stoppingToken);
            }
            finally
            {
                SyncLock.Release();
            }
        }

        private async Task<ZkAttendanceSyncResult> SyncAttendanceCoreAsync(CancellationToken stoppingToken)
        {
            var syncedCount = 0;
            var skippedCount = 0;

            if (!_options.Enabled || string.IsNullOrWhiteSpace(_options.ConnectionString))
            {
                _logger.LogWarning("ZK attendance sync skipped because ZkAttendance:ConnectionString is not configured.");
                return new ZkAttendanceSyncResult(0, 1);
            }

            try
            {
                using var conn = new SqlConnection(new SqlConnectionStringBuilder(_options.ConnectionString) { ApplicationIntent = ApplicationIntent.ReadOnly, ConnectTimeout = 10 }.ConnectionString);
                await conn.OpenAsync(stoppingToken);
                // Namespace receipts by source database; never include credentials in persisted data.
                var source = new SqlConnectionStringBuilder(_options.ConnectionString);
                var sourceKey = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(
                    source.DataSource.ToLowerInvariant() + "|" + source.InitialCatalog.ToLowerInvariant())));

                var terminals = new List<(string sn, string ip)>();
                using (var cmd = new SqlCommand("SELECT sn, ip_address FROM iclock_terminal", conn))
                using (var reader = await cmd.ExecuteReaderAsync(stoppingToken))
                {
                    while (await reader.ReadAsync(stoppingToken))
                    {
                        terminals.Add((reader["sn"].ToString() ?? "", reader["ip_address"].ToString() ?? ""));
                    }
                }

                foreach (var terminal in terminals)
                {
                    var registeredMachines = await _dbContext.MachineRegistrations
                        .AsNoTracking()
                        .Where(m => m.ltxt == terminal.sn && !m.inactive && !m.utblock).ToListAsync(stoppingToken);

                    if (registeredMachines.Count == 0)
                    {
                        skippedCount++;
                        _logger.LogWarning("Machine not registered for Serial Number: {SN}", terminal.sn);
                        continue;
                    }

                    // Scan all history in bounded pages, including punches already consumed by EDXS.
                    // Missing employee mappings are retried on the next run.
                    var afterId = 0;
                    while (!stoppingToken.IsCancellationRequested)
                    {
                    var transactions = new List<ZkTransaction>();
                    using (var cmd = new SqlCommand("SELECT TOP (1000) Id, emp_code, punch_time, punch_state FROM iclock_transaction WHERE terminal_sn = @sn AND Id > @afterId ORDER BY Id", conn))
                    {
                        cmd.Parameters.AddWithValue("@sn", terminal.sn);
                        cmd.Parameters.AddWithValue("@afterId", afterId);
                        using (var reader = await cmd.ExecuteReaderAsync(stoppingToken))
                        {
                            while (await reader.ReadAsync(stoppingToken))
                            {
                                transactions.Add(new ZkTransaction
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    EmpCode = reader["emp_code"].ToString() ?? "",
                                    PunchTime = Convert.ToDateTime(reader["punch_time"]),
                                    PunchState = Convert.ToInt32(reader["punch_state"])
                                });
                            }
                        }
                    }

                    if (transactions.Count == 0) break;
                    afterId = transactions[^1].Id;
                    var ids = transactions.Select(x => x.Id).ToArray();
                    var imported = (await _dbContext.ZkImportReceipts.AsNoTracking()
                        .Where(x => x.SourceKey == sourceKey && ids.Contains(x.TransactionId))
                        .Select(x => x.TransactionId).ToListAsync(stoppingToken)).ToHashSet();
                    foreach (var txn in transactions.Where(x => !imported.Contains(x.Id)))
                    {
                        try
                        {
                            var candidates = new List<(SMMachineRegistrationLegacy Machine, EmployeeProfileLegacy Employee)>();
                            foreach (var device in registeredMachines)
                            {
                                var legacyDevice = await _dbContext.ZkDeviceLinks.AsNoTracking()
                                    .SingleOrDefaultAsync(x => x.MachineId == device.Id, stoppingToken);
                                var employees = _dbContext.Employees.AsNoTracking();
                                if (legacyDevice != null)
                                    employees = from employeeRow in employees
                                                join link in _dbContext.ZkEmployeeLinks on employeeRow.Id equals link.EmployeeId
                                                where link.SourceKey == legacyDevice.SourceKey
                                                    && link.SourceClientId == legacyDevice.SourceClientId
                                                    && link.MachineCode == txn.EmpCode
                                                select employeeRow;
                                else
                                    employees = employees.Where(e => e.MachineCode == txn.EmpCode && e.ClientId == device.clientid);
                                var matches = await employees.ToListAsync(stoppingToken);
                                // Prefer the device's branch when codes have been reused across campuses.
                                if (matches.Count > 1 && matches.Any(e => e.EntityId == device.entityid))
                                    matches = matches.Where(e => e.EntityId == device.entityid).ToList();
                                foreach (var match in matches) candidates.Add((device, match));
                            }
                            // Duplicate registrations of the same physical serial may resolve to the same employee.
                            candidates = candidates.GroupBy(x => x.Employee.Id).Select(g => g.OrderBy(x => x.Machine.Id).First()).ToList();
                            if (candidates.Count > 1)
                                candidates = candidates.Where(x => (!x.Employee.JoiningDate.HasValue || x.Employee.JoiningDate.Value.Date <= txn.PunchTime.Date)
                                    && (!x.Employee.ResignDate.HasValue || x.Employee.ResignDate.Value.Date >= txn.PunchTime.Date)).ToList();
                            if (candidates.Count != 1)
                            {
                                skippedCount++;
                                _logger.LogWarning("ZK transaction {Id} deferred: employee mapping is missing or ambiguous.", txn.Id);
                                continue;
                            }
                            var (machine, employee) = candidates[0];

                            int dayOfWeek = ToLegacyDayId(txn.PunchTime.DayOfWeek);
                            var currentShift = await GetCurrentShiftDetail(
                                employee.Id,
                                machine.entityid,
                                machine.clientid,
                                txn.PunchTime.Date,
                                employee.ShiftId,
                                dayOfWeek,
                                stoppingToken);

                            int activityId = txn.PunchTime.TimeOfDay >= new TimeSpan(12, 0, 0) ? 2 : 1;

                            await using var importTransaction = await _dbContext.Database.BeginTransactionAsync(stoppingToken);
                            var attendance = new HRAtnintoutLegacy
                            {
                                Id = await GetNextIdAsync(stoppingToken),
                                EmpId = employee.Id,
                                EntityId = employee.EntityId ?? machine.entityid,
                                ClientId = employee.ClientId ?? machine.clientid,
                                MachineId = machine.Id,
                                OrgDttime = txn.PunchTime,
                                Dttime = txn.PunchTime,
                                ShiftId = currentShift?.ShiftId ?? employee.ShiftId,
                                ShiftDtlId = currentShift?.ShiftDtlId,
                                DayId = dayOfWeek,
                                AtnActivityId = activityId,
                                AtnTypeId = 1,
                                ShiftIn = currentShift?.InTime,
                                ShiftOut = currentShift?.OutTime,
                                SchTimeIn = currentShift?.InTime,
                                SchTimeOut = currentShift?.OutTime,
                                UtBlock = false,
                                UtSysGen = true
                            };

                            _dbContext.EmployeeAttendances.Add(attendance);
                            _dbContext.ZkImportReceipts.Add(new ZkImportReceipt
                            {
                                SourceKey = sourceKey, TransactionId = txn.Id,
                                AttendanceId = attendance.Id, ImportedAtUtc = DateTime.UtcNow
                            });
                            // Attendance and receipt commit together. ZKBioTime is strictly read-only.
                            await _dbContext.SaveChangesAsync(stoppingToken);
                            await importTransaction.CommitAsync(stoppingToken);
                            _dbContext.ChangeTracker.Clear();

                            syncedCount++;
                            _logger.LogInformation("Synced Attendance for Emp: {Code}, Time: {Time}", txn.EmpCode, txn.PunchTime);
                        }
                        catch (Exception innerEx) when (!stoppingToken.IsCancellationRequested)
                        {
                            _dbContext.ChangeTracker.Clear();
                            skippedCount++;
                            _logger.LogError(innerEx, "Error processing ZK transaction ID: {Id}", txn.Id);
                        }
                    }
                    }
                }
            }
            catch (Exception connEx) when (!stoppingToken.IsCancellationRequested)
            {
                skippedCount++;
                _logger.LogError(connEx, "Failed to connect to ZK database.");
            }

            return new ZkAttendanceSyncResult(syncedCount, skippedCount);
        }

        private async Task<int> GetNextIdAsync(CancellationToken ct)
        {
            var maxId = await _dbContext.EmployeeAttendances.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id, ct) ?? 0;
            return maxId + 1;
        }

        private async Task<CurrentShiftDetail?> GetCurrentShiftDetail(
            int employeeId,
            int? entityId,
            int? clientId,
            DateTime attendanceDate,
            int? requestShiftId,
            int dayId,
            CancellationToken cancellationToken)
        {
            var roster = await _dbContext.EmployeeRosters.AsNoTracking()
                .Where(x =>
                    x.EmployeeId == employeeId &&
                    (x.UtBlock == null || x.UtBlock == false) &&
                    x.StartDate.HasValue &&
                    x.StartDate.Value.Date <= attendanceDate.Date &&
                    (!x.EndDate.HasValue || x.EndDate.Value.Date >= attendanceDate.Date))
                .OrderByDescending(x => x.StartDate)
                .Select(x => new { x.ShiftId })
                .FirstOrDefaultAsync(cancellationToken);

            var shiftId = roster?.ShiftId ?? requestShiftId;
            if (!shiftId.HasValue)
            {
                return null;
            }

            var dayIds = GetLegacyDayIds(attendanceDate.DayOfWeek);

            var query =
                from detail in _dbContext.ShiftDetails.AsNoTracking()
                join header in _dbContext.Shifts.AsNoTracking() on detail.ShiftId equals header.Id
                where detail.Id > 0
                      && detail.ShiftId == shiftId.Value
                      && detail.DayId.HasValue
                      && dayIds.Contains(detail.DayId.Value)
                      && (!detail.StartDate.HasValue || detail.StartDate.Value.Date <= attendanceDate.Date)
                      && (!detail.EndDate.HasValue || detail.EndDate.Value.Date >= attendanceDate.Date)
                      && (detail.UtBlock == null || detail.UtBlock == false)
                      && (header.UtBlock == null || header.UtBlock == false)
                      && (!clientId.HasValue || header.ClientId == clientId.Value)
                orderby header.EntityId == entityId descending, detail.StartDate descending
                select new CurrentShiftDetail
                {
                    ShiftId = shiftId.Value,
                    ShiftDtlId = detail.Id,
                    InTime = detail.InTime,
                    OutTime = detail.OutTime
                };

            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        private static int ToLegacyDayId(DayOfWeek dayOfWeek)
        {
            return dayOfWeek == DayOfWeek.Sunday ? 7 : (int)dayOfWeek;
        }

        private static List<int> GetLegacyDayIds(DayOfWeek dayOfWeek)
        {
            var standardDayId = ToLegacyDayId(dayOfWeek);
            var etDayId = dayOfWeek == DayOfWeek.Sunday ? 106 : (int)dayOfWeek + 99;
            return standardDayId == etDayId
                ? new List<int> { standardDayId }
                : new List<int> { standardDayId, etDayId };
        }
        private class ZkTransaction
        {
            public int Id { get; set; }
            public string EmpCode { get; set; } = string.Empty;
            public DateTime PunchTime { get; set; }
            public int PunchState { get; set; }
        }

        private sealed class CurrentShiftDetail
        {
            public int ShiftId { get; set; }
            public int ShiftDtlId { get; set; }
            public DateTime? InTime { get; set; }
            public DateTime? OutTime { get; set; }
        }
    }
}

