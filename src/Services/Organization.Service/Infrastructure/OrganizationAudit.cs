using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Organization.Service.Infrastructure.Data;

using Organization.Service.Entities;

public partial class OrganizationDbContext : DbContext
{
    private readonly IHttpContextAccessor? _auditHttp;
    private readonly TimeProvider _auditClock;
    private readonly IConfiguration? _auditConfiguration;
    private static readonly Dictionary<Type, OrganizationAuditTable> AuditTables = OrganizationAuditSchema.Tables.ToDictionary(t => t.Type);

    public OrganizationDbContext(DbContextOptions<OrganizationDbContext> options, IHttpContextAccessor? httpContextAccessor = null,
        TimeProvider? timeProvider = null, IConfiguration? configuration = null) : base(options)
    {
        _auditHttp = httpContextAccessor;
        _auditClock = timeProvider ?? TimeProvider.System;
        _auditConfiguration = configuration;
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        StampAuditFields();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        StampAuditFields();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void StampAuditFields()
    {
        ChangeTracker.DetectChanges();
        foreach (var entry in ChangeTracker.Entries().Where(e => AuditTables.ContainsKey(e.Metadata.ClrType)))
        {
            var table = AuditTables[entry.Metadata.ClrType];
            if (entry.State == EntityState.Deleted && Has(table, "utblock"))
            {
                // A key-only delete must not overwrite the retained row's business fields.
                entry.State = EntityState.Unchanged;
                Set(entry, "utblock", true);
            }
            if (entry.State is not (EntityState.Added or EntityState.Modified)) continue;
            var added = entry.State == EntityState.Added;
            foreach (var value in AuditValues(table, added)) Set(entry, value.Key, value.Value);
            if (added)
            {
                foreach (var flag in new[] { "utblock", "utsysgen", "utispartial" })
                    if (Has(table, flag) && Get(entry, flag).CurrentValue == null) Set(entry, flag, false);
                if (Has(table, "utrecordversion") && Convert.ToInt32(Get(entry, "utrecordversion").CurrentValue ?? 0) < 1)
                    Set(entry, "utrecordversion", 1);
            }
            else
            {
                foreach (var property in entry.Properties.Where(p => (p.Metadata.GetColumnName() ?? "").StartsWith("utadd", StringComparison.OrdinalIgnoreCase)))
                    property.IsModified = false;
                if (Has(table, "uteditid")) Get(entry, "uteditid").IsModified = false;
                if (Has(table, "utrecordversion"))
                {
                    var version = Get(entry, "utrecordversion");
                    var original = Convert.ToInt32(version.OriginalValue ?? 0);
                    if (Convert.ToInt32(version.CurrentValue ?? 0) <= original) version.CurrentValue = checked(original + 1);
                }
            }
            if (table.DocType.HasValue && Has(table, "utdoctype") && Has(table, "utsubdoctype"))
            {
                var type = Convert.ToInt32(Get(entry, "utdoctype").CurrentValue ?? 0);
                var subtype = Convert.ToInt32(Get(entry, "utsubdoctype").CurrentValue ?? 0);
                if (added)
                {
                    Set(entry, "utdoctype", table.DocType);
                    Set(entry, "utsubdoctype", table.SubType);
                }
                else
                {
                    if (type <= 0 && (subtype <= 0 || subtype == table.SubType)) Set(entry, "utdoctype", table.DocType);
                    if (subtype <= 0 && (type <= 0 || type == table.DocType)) Set(entry, "utsubdoctype", table.SubType);
                }
            }
        }
    }

    internal Dictionary<string, object?> AuditValues(OrganizationAuditTable table, bool added) => AuditValues(table.Columns, added);

    internal Dictionary<string, object?> AuditValues(OrganizationAuditColumn[] columns, bool added)
    {
        var http = _auditHttp?.HttpContext;
        var identity = http?.User;
        var signed = identity?.Identity?.IsAuthenticated == true;
        var actor = signed ? identity!.FindFirst("userName")?.Value ?? identity.FindFirst(ClaimTypes.Name)?.Value
            ?? identity.FindFirst("unique_name")?.Value ?? identity.FindFirst("userId")?.Value
            ?? identity.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? identity.FindFirst("sub")?.Value : null;
        actor = string.IsNullOrWhiteSpace(actor) ? "system" : actor.Trim();
        var address = http?.Connection.RemoteIpAddress;
        if (address?.IsIPv4MappedToIPv6 == true) address = address.MapToIPv4();
        int? session = signed && int.TryParse(identity!.FindFirst("sessionId")?.Value, out var id) && id > 0 ? id : null;
        var prefix = added ? "utadd" : "utedit";
        var values = new Dictionary<string, object?>
        {
            [prefix + "time"] = _auditClock.GetUtcNow().UtcDateTime,
            [prefix + "user"] = actor,
            [prefix + "system"] = _auditConfiguration?["OrganizationAudit:SystemName"] ?? Environment.MachineName,
            [prefix + "ip"] = address?.ToString(),
            [prefix + "appversion"] = _auditConfiguration?["OrganizationAudit:ApplicationVersion"] ?? typeof(OrganizationDbContext).Assembly.GetName().Version?.ToString(),
            [prefix + "dbversion"] = _auditConfiguration?["OrganizationAudit:DatabaseVersion"],
            [prefix + "sessionid"] = session
        };
        return values.Where(v => columns.Any(c => c.Name == v.Key)
                && (v.Value != null || columns.Single(c => c.Name == v.Key).Nullable))
            .ToDictionary(v => v.Key, v => Fit(columns.Single(c => c.Name == v.Key), v.Value));
    }

    private static object? Fit(OrganizationAuditColumn column, object? value)
    {
        if (value is not string text) return value;
        if (string.IsNullOrWhiteSpace(text)) return null;
        if (column.Length < 0) return text;
        var length = column.SqlType.StartsWith("n", StringComparison.Ordinal) ? column.Length / 2 : column.Length;
        if (length < 0 || text.Length <= length) return text;
        return column.Name.EndsWith("ip", StringComparison.Ordinal) ? null : text[..length];
    }

    internal static bool Has(OrganizationAuditTable table, string column) => table.Columns.Any(c => c.Name == column);
    private static PropertyEntry Get(EntityEntry entry, string column) => entry.Property(entry.Metadata.GetProperties()
        .Single(p => string.Equals(p.GetColumnName(), column, StringComparison.OrdinalIgnoreCase)).Name);
    private static void Set(EntityEntry entry, string column, object? value) => Get(entry, column).CurrentValue = value;

    private static void ConfigureAuditModel(ModelBuilder builder)
    {
        builder.Entity<Lov>().HasQueryFilter(x =>
            (x.vtype != "CALLT" && x.vtype != "CALLM" && x.vtype != "COURI") || !x.utblock);
        foreach (var table in OrganizationAuditSchema.Tables)
        {
            var entity = builder.Entity(table.Type);
            foreach (var column in table.Columns)
            {
                var existing = entity.Metadata.GetProperties().SingleOrDefault(p => string.Equals(p.GetColumnName(), column.Name, StringComparison.OrdinalIgnoreCase));
                var type = column.SqlType switch
                {
                    "bit" => column.Nullable ? typeof(bool?) : typeof(bool),
                    "int" => column.Nullable ? typeof(int?) : typeof(int),
                    "bigint" => column.Nullable ? typeof(long?) : typeof(long),
                    "datetime" or "datetime2" => column.Nullable ? typeof(DateTime?) : typeof(DateTime),
                    _ => typeof(string)
                };
                var property = existing == null ? entity.Property(type, column.Name) : entity.Property(existing.Name);
                property.HasColumnName(column.Name);
                if (type == typeof(string))
                {
                    if (column.Length > 0) property.HasMaxLength(column.SqlType.StartsWith("n", StringComparison.Ordinal) ? column.Length / 2 : column.Length);
                    property.IsUnicode(column.SqlType.StartsWith("n", StringComparison.Ordinal)).IsRequired(!column.Nullable);
                }
            }
            if (table.Filter && Has(table, "utblock"))
            {
                var block = entity.Metadata.GetProperties().Single(p => string.Equals(p.GetColumnName(), "utblock", StringComparison.OrdinalIgnoreCase));
                var row = System.Linq.Expressions.Expression.Parameter(table.Type, "row");
                entity.HasQueryFilter(System.Linq.Expressions.Expression.Lambda(System.Linq.Expressions.Expression.NotEqual(
                    System.Linq.Expressions.Expression.Call(typeof(EF), nameof(EF.Property), [block.ClrType], row, System.Linq.Expressions.Expression.Constant(block.Name)),
                    System.Linq.Expressions.Expression.Convert(System.Linq.Expressions.Expression.Constant(true), block.ClrType)), row));
            }
        }
    }
}
