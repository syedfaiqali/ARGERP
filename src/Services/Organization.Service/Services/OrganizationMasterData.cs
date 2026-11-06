using System.Data;
using System.Data.Common;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Organization.Service.Infrastructure.Data;

namespace Organization.Service.Services;

internal static class OrganizationMasterData
{
    public static Task<T> SaveAsync<T>(OrganizationDbContext db, Func<Task<T>> save, CancellationToken ct) =>
        db.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
        {
            await using var transaction = db.Database.IsRelational() && db.Database.CurrentTransaction == null
                ? await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct) : null;
            try
            {
                var result = await save();
                if (transaction != null) await transaction.CommitAsync(ct);
                return result;
            }
            catch { db.ChangeTracker.Clear(); throw; }
        });

    public static string NextCode(IEnumerable<string?> codes, int maxLength)
    {
        var existing = codes.Select(x => x?.Trim() ?? "").ToHashSet(StringComparer.OrdinalIgnoreCase);
        long next = existing.Where(x => x.Length <= maxLength && x.All(char.IsAsciiDigit))
            .Select(x => long.TryParse(x, out var number) ? number : 0).DefaultIfEmpty().Max() + 1;
        var code = next.ToString(System.Globalization.CultureInfo.InvariantCulture);
        if (code.Length > maxLength) throw new InvalidOperationException("Automatic code range is exhausted.");
        return code;
    }

    // All modules share the legacy database. Discover explicit foreign keys and
    // legacy reference columns, including tables outside this service's EF model.
    public static async Task<IReadOnlyList<string>> FindUsageAsync(OrganizationDbContext db, string kind, int id, CancellationToken ct)
    {
        var screens = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);
        if (!db.Database.IsRelational())
        {
            // The same guard remains testable with mapped reference entities.
            foreach (var entity in db.Model.GetEntityTypes())
            {
                if (entity.GetTableName() == "Org" + kind) continue;
                foreach (var property in entity.GetProperties().Where(p => IsReference(kind, p.GetColumnName() ?? p.Name)))
                {
                    var method = typeof(OrganizationMasterData).GetMethod(nameof(HasMappedReference), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!;
                    var used = await (Task<bool>)method.MakeGenericMethod(entity.ClrType).Invoke(null, [db, property.Name, property.ClrType == typeof(int), id, ct])!;
                    if (used) { screens.Add(ScreenName(entity.GetTableName() ?? entity.ClrType.Name)); break; }
                }
            }
            return screens.ToArray();
        }
        var connection = db.Database.GetDbConnection();
        var opened = connection.State != ConnectionState.Open;
        if (opened) await db.Database.OpenConnectionAsync(ct);
        try
        {
            await using var command = connection.CreateCommand();
            command.Transaction = db.Database.CurrentTransaction?.GetDbTransaction();
            command.CommandText = """
                SELECT s.name,t.name,c.name
                FROM sys.tables t JOIN sys.schemas s ON s.schema_id=t.schema_id
                JOIN sys.columns c ON c.object_id=t.object_id
                WHERE t.is_ms_shipped=0 AND t.name NOT LIKE 'Backup%' AND t.name <> @master
                AND c.system_type_id IN(48,52,56,127)
                AND ((@kind='Department' AND LOWER(c.name) IN('depid','deptid','departmentid','orgdepartmentid','pdepartid'))
                  OR (@kind='Designation' AND LOWER(c.name) IN('designationid','desgid','orgdesignationid','reportingtodesignationid'))
                  OR EXISTS(SELECT 1 FROM sys.foreign_key_columns f
                    WHERE f.parent_object_id=t.object_id AND f.parent_column_id=c.column_id
                    AND f.referenced_object_id=OBJECT_ID('dbo.'+@master)))
                ORDER BY CASE WHEN t.name='EREmployeeinfohdr' THEN 0 WHEN t.name='EREmpPosition' THEN 1 ELSE 2 END,
                    (SELECT COALESCE(SUM(p.rows),0) FROM sys.partitions p WHERE p.object_id=t.object_id AND p.index_id IN(0,1)),
                    s.name,t.name,c.name;
                """;
            Add(command, "@kind", kind);
            Add(command, "@master", "Org" + kind);
            var references = new List<(string Schema, string Table, string Column)>();
            await using (var reader = await command.ExecuteReaderAsync(ct))
                while (await reader.ReadAsync(ct)) references.Add((reader.GetString(0), reader.GetString(1), reader.GetString(2)));
            // Collect every usage in a single result set/database round trip.
            // Group a table's reference columns to avoid scanning it repeatedly.
            var probes = new StringBuilder("SET NOCOUNT ON;\n");
            foreach (var table in references.GroupBy(x => (x.Schema, x.Table)))
            {
                probes.Append("SELECT N'").Append(table.Key.Table.Replace("'", "''"))
                    .Append("' AS TableName WHERE EXISTS(SELECT 1 FROM ")
                    .Append(Quote(table.Key.Schema)).Append('.').Append(Quote(table.Key.Table))
                    .Append(" WHERE ").Append(string.Join(" OR ", table.Select(x => Quote(x.Column) + "=@id")))
                    .Append(") UNION ALL\n");
            }
            probes.Append("SELECT CAST(NULL AS nvarchar(128)) WHERE 1=0;");
            command.Parameters.Clear();
            command.CommandText = probes.ToString();
            Add(command, "@id", id);
            await using var usedTables = await command.ExecuteReaderAsync(ct);
            while (await usedTables.ReadAsync(ct)) screens.Add(ScreenName(usedTables.GetString(0)));
            return screens.ToArray();
        }
        finally { if (opened) await db.Database.CloseConnectionAsync(); }
    }

    private static Task<bool> HasMappedReference<T>(OrganizationDbContext db, string property, bool required, int id, CancellationToken ct) where T : class => required
        ? db.Set<T>().IgnoreQueryFilters().AnyAsync(x => EF.Property<int>(x, property) == id, ct)
        : db.Set<T>().IgnoreQueryFilters().AnyAsync(x => EF.Property<int?>(x, property) == id, ct);

    private static bool IsReference(string kind, string column) => kind == "Department"
        ? new[] { "depid", "deptid", "departmentid", "orgdepartmentid", "pdepartid" }.Contains(column.ToLowerInvariant())
        : new[] { "designationid", "desgid", "orgdesignationid", "reportingtodesignationid" }.Contains(column.ToLowerInvariant());

    private static string Quote(string name) => "[" + name.Replace("]", "]]") + "]";
    private static void Add(DbCommand command, string name, object value)
    { var parameter = command.CreateParameter(); parameter.ParameterName = name; parameter.Value = value; command.Parameters.Add(parameter); }

    private static string ScreenName(string table) => table.ToLowerInvariant() switch
    {
        "eremployeeinfohdr" => "Employee Profile",
        "erempposition" => "Employee Position Assignment",
        "etshiftshareddtl" or "etshifthdr" or "etshiftdtl" or "etshiftsubdtl" => "Shift",
        "etpolicyshareddtl" or "etpolicyhdr" => "Attendance Policy",
        "etholidaygrpshareddtl" or "etholidaygrphdr" => "Holiday Group",
        "hrposition" => "Position Setup",
        "hrvacancy" => "Vacancy",
        "erdesgpayroll" => "Designation Payroll",
        "hrdailyattendance" => "Daily Attendance",
        "hrpayrollprocess" => "Payroll Processing",
        "fitransdtl" => "Finance Transactions",
        "fmexpensedtl" or "fmexpensehdr" => "Expense",
        "lmstrainingrecord" => "Training Records",
        "pmconsumingitem" => "Consuming Item",
        "pmpurchaseorderhdr" => "Purchase Order",
        "smexamapproval" or "smapprovallog" => "Exam Approval",
        _ => "Other related setup"
    };
}
