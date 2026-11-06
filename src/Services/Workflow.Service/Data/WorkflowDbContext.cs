using Microsoft.EntityFrameworkCore;

namespace Workflow.Service.Data;

/// <summary>
/// The standard EDXS <c>ut*</c> audit block, carried by every WF_ table so the workflow
/// tables match the convention of the business tables they live beside in OrganizationDB.
///
/// <c>utsysgen</c>, <c>utblock</c>, <c>utispartial</c> and <c>utrecordversion</c> are NOT NULL
/// with no database defaults, so every insert path must set them; <see cref="WorkflowAudit"/>
/// centralises that. The <c>*id</c>/<c>*sessionid</c>/<c>*ip</c> members stay null here: a
/// background execution worker has no HTTP session, client address or interactive user.
/// </summary>
public abstract class WorkflowAuditable
{
    public int? utdoctype { get; set; }
    public int? utsubdoctype { get; set; }
    public bool utsysgen { get; set; }
    public bool utblock { get; set; }
    public bool utispartial { get; set; }
    public DateTime? utaddtime { get; set; }
    public string? utadduser { get; set; }
    public string? utaddsystem { get; set; }
    public string? utaddip { get; set; }
    public string? utaddappversion { get; set; }
    public string? utadddbversion { get; set; }
    public long? utaddid { get; set; }
    public int? utaddsessionid { get; set; }
    public DateTime? utedittime { get; set; }
    public string? utedituser { get; set; }
    public string? uteditsystem { get; set; }
    public string? uteditip { get; set; }
    public string? uteditappversion { get; set; }
    public string? uteditdbversion { get; set; }
    public long? uteditid { get; set; }
    public int? uteditsessionid { get; set; }
    public int utrecordversion { get; set; } = 1;
}

/// <summary>Stamps the <c>ut*</c> block on insert and on update, from the workflow service's own identity.</summary>
public static class WorkflowAudit
{
    public const string System = "workflow";

    /// <summary>Called for every new row. Sets the four NOT NULL columns and the add-side trail.</summary>
    public static T Added<T>(T row, int? userId = null) where T : WorkflowAuditable
    {
        row.utsysgen = userId is null or 0; row.utblock = false; row.utispartial = false; row.utrecordversion = 1;
        row.utaddtime = DateTime.UtcNow; row.utadduser = User(userId); row.utaddsystem = System;
        return row;
    }

    /// <summary>Called whenever an existing row changes; bumps the record version as the SM tables do.</summary>
    public static void Edited<T>(T row, int? userId = null) where T : WorkflowAuditable
    {
        row.utrecordversion++; row.utedittime = DateTime.UtcNow; row.utedituser = User(userId); row.uteditsystem = System;
    }

    private static string User(int? userId) => userId is null or 0 ? System : userId.Value.ToString();
}

/// <summary>Maps the <c>ut*</c> block onto its lowercase columns, with the lengths used across the SM tables.</summary>
public static class WorkflowAuditMapping
{
    public static void AuditBlock<T>(this Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<T> entity) where T : WorkflowAuditable
    {
        entity.Property(x => x.utdoctype).HasColumnName("utdoctype");
        entity.Property(x => x.utsubdoctype).HasColumnName("utsubdoctype");
        entity.Property(x => x.utsysgen).HasColumnName("utsysgen");
        entity.Property(x => x.utblock).HasColumnName("utblock");
        entity.Property(x => x.utispartial).HasColumnName("utispartial");
        entity.Property(x => x.utaddtime).HasColumnName("utaddtime");
        entity.Property(x => x.utadduser).HasColumnName("utadduser").HasMaxLength(25);
        entity.Property(x => x.utaddsystem).HasColumnName("utaddsystem").HasMaxLength(25);
        entity.Property(x => x.utaddip).HasColumnName("utaddip").HasMaxLength(25);
        entity.Property(x => x.utaddappversion).HasColumnName("utaddappversion").HasMaxLength(10);
        entity.Property(x => x.utadddbversion).HasColumnName("utadddbversion").HasMaxLength(10);
        entity.Property(x => x.utaddid).HasColumnName("utaddid");
        entity.Property(x => x.utaddsessionid).HasColumnName("utaddsessionid");
        entity.Property(x => x.utedittime).HasColumnName("utedittime");
        entity.Property(x => x.utedituser).HasColumnName("utedituser").HasMaxLength(25);
        entity.Property(x => x.uteditsystem).HasColumnName("uteditsystem").HasMaxLength(25);
        entity.Property(x => x.uteditip).HasColumnName("uteditip").HasMaxLength(25);
        entity.Property(x => x.uteditappversion).HasColumnName("uteditappversion").HasMaxLength(10);
        entity.Property(x => x.uteditdbversion).HasColumnName("uteditdbversion").HasMaxLength(10);
        entity.Property(x => x.uteditid).HasColumnName("uteditid");
        entity.Property(x => x.uteditsessionid).HasColumnName("uteditsessionid");
        entity.Property(x => x.utrecordversion).HasColumnName("utrecordversion");
    }
}

public sealed class WorkflowDbContext(DbContextOptions<WorkflowDbContext> options) : DbContext(options)
{
    public DbSet<ActionCatalog> Actions => Set<ActionCatalog>();
    public DbSet<Definition> Definitions => Set<Definition>();
    public DbSet<Policy> Policies => Set<Policy>();
    public DbSet<ApproverGroup> Groups => Set<ApproverGroup>();
    public DbSet<GroupBinding> Bindings => Set<GroupBinding>();
    public DbSet<Instance> Instances => Set<Instance>();
    public DbSet<UserTask> Tasks => Set<UserTask>();
    public DbSet<AuditEvent> Events => Set<AuditEvent>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<OutboxMessage> Outbox => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder model)
    {
        model.Entity<ActionCatalog>(e => { e.ToTable("WF_Actions"); e.HasKey(x => x.ScreenKey); e.Property(x => x.ScreenKey).HasMaxLength(100); e.AuditBlock(); });
        model.Entity<Definition>(e => {
            e.ToTable("WF_Definitions"); e.HasKey(x => x.Id); e.Property(x => x.Version).IsConcurrencyToken();
            e.Property(x => x.ScreenKey).HasMaxLength(100); e.Property(x => x.ActionKey).HasMaxLength(10);
            e.HasIndex(x => new { x.ClientId, x.ScreenKey, x.ActionKey });
            e.AuditBlock();
        });
        model.Entity<Policy>(e => {
            e.ToTable("WF_Policies"); e.HasKey(x => new { x.ClientId, x.ScreenKey, x.ActionKey });
            e.Property(x => x.ScreenKey).HasMaxLength(100); e.Property(x => x.ActionKey).HasMaxLength(10);
            e.Property(x => x.Version).IsConcurrencyToken();
            e.AuditBlock();
        });
        model.Entity<ApproverGroup>(e => { e.ToTable("WF_Groups"); e.HasKey(x => x.Key); e.Property(x => x.Key).HasMaxLength(100); e.AuditBlock(); });
        model.Entity<GroupBinding>(e => {
            e.ToTable("WF_GroupBindings"); e.HasKey(x => new { x.ClientId, x.GroupKey }); e.Property(x => x.GroupKey).HasMaxLength(100);
            e.Property(x => x.Version).IsConcurrencyToken();
            e.AuditBlock();
        });
        model.Entity<Instance>(e => {
            e.ToTable("WF_Instances"); e.HasKey(x => x.Id); e.Property(x => x.Version).IsConcurrencyToken();
            e.Property(x => x.OpenKey).HasMaxLength(64); e.Property(x => x.ScreenKey).HasMaxLength(100); e.Property(x => x.Status).HasMaxLength(40);
            e.HasIndex(x => x.OpenKey).IsUnique().HasFilter("[OpenKey] IS NOT NULL");
            e.HasIndex(x => new { x.ClientId, x.SubmitterId, x.IdempotencyKey }).IsUnique();
            e.HasIndex(x => new { x.ClientId, x.Status, x.CreatedAtUtc });
            e.HasIndex(x => new { x.Status, x.NextAttemptAtUtc });
            e.AuditBlock();
        });
        model.Entity<UserTask>(e => {
            e.ToTable("WF_Tasks"); e.HasKey(x => x.Id); e.HasIndex(x => new { x.InstanceId, x.StepIndex, x.Generation, x.UserId }).IsUnique();
            e.HasIndex(x => new { x.ClientId, x.UserId, x.Status });
            e.HasOne<Instance>().WithMany().HasForeignKey(x => x.InstanceId).OnDelete(DeleteBehavior.Restrict);
            e.AuditBlock();
        });
        model.Entity<AuditEvent>(e => {
            e.ToTable("WF_Events"); e.HasKey(x => x.Id); e.HasIndex(x => new { x.ClientId, x.InstanceId, x.AtUtc });
            e.HasIndex(x => new { x.InstanceId, x.UserId, x.IdempotencyKey }).IsUnique().HasFilter("[IdempotencyKey] IS NOT NULL");
            e.AuditBlock();
        });
        model.Entity<Notification>(e => {
            e.ToTable("WF_Notifications"); e.HasKey(x => x.Id); e.HasIndex(x => new { x.ClientId, x.UserId, x.ReadAtUtc });
            e.AuditBlock();
        });
        model.Entity<OutboxMessage>(e => { e.ToTable("WF_Outbox"); e.HasKey(x => x.Id); e.HasIndex(x => x.DeliveredAtUtc); e.AuditBlock(); });
        // SQL datetime2 does not preserve DateTime.Kind. Restore the UTC contract on
        // materialization so JSON includes Z and browsers convert to the correct local time.
        var utc = new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime, DateTime>(
            value => value, value => DateTime.SpecifyKind(value, DateTimeKind.Utc));
        foreach (var entity in model.Model.GetEntityTypes())
        foreach (var property in entity.GetProperties())
            if (property.Name.EndsWith("Utc", StringComparison.Ordinal) &&
                (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?)))
                property.SetValueConverter(utc);
    }
}

public sealed class ActionCatalog : WorkflowAuditable
{
    public string ScreenKey { get; set; } = "";
    public string Service { get; set; } = "";
    public string RegistrationJson { get; set; } = "";
    public DateTime RegisteredAtUtc { get; set; }
}
public sealed class Definition : WorkflowAuditable
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int ClientId { get; set; } // 0 is platform-global, never a data-record tenant.
    public string ScreenKey { get; set; } = "";
    public string ActionKey { get; set; } = "";
    public string Name { get; set; } = "";
    public string StepsJson { get; set; } = "[]";
    public string Status { get; set; } = "Draft";
    public Guid? PreviousVersionId { get; set; }
    public int Version { get; set; } = 1;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public int CreatedBy { get; set; }
}
public sealed class Policy : WorkflowAuditable
{
    public int ClientId { get; set; }
    public string ScreenKey { get; set; } = "";
    public string ActionKey { get; set; } = "";
    public string Mode { get; set; } = "Inherit";
    public Guid? DefinitionId { get; set; }
    public int Version { get; set; } = 1;
}
public sealed class ApproverGroup : WorkflowAuditable { public string Key { get; set; } = ""; public string Name { get; set; } = ""; }
public sealed class GroupBinding : WorkflowAuditable
{
    public int ClientId { get; set; }
    public string GroupKey { get; set; } = "";
    public string SelectorsJson { get; set; } = "[]";
    public int Version { get; set; } = 1;
}
public sealed class Instance : WorkflowAuditable
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int ClientId { get; set; }
    public int? EntityId { get; set; }
    public int SubmitterId { get; set; }
    public string SubmitterName { get; set; } = "";
    public string ScreenKey { get; set; } = "";
    public string Operation { get; set; } = "";
    public string Subject { get; set; } = "";
    public string? RecordKey { get; set; }
    public string? OpenKey { get; set; }
    public Guid IdempotencyKey { get; set; }
    public string RequestHash { get; set; } = "";
    public Guid DefinitionId { get; set; }
    public string DefinitionName { get; set; } = "";
    public string StepsJson { get; set; } = "[]";
    public string PayloadEnvelope { get; set; } = "";
    public string PayloadHash { get; set; } = "";
    public string Status { get; set; } = "Pending";
    public int StepIndex { get; set; }
    public int Generation { get; set; }
    public int Version { get; set; } = 1;
    public Guid? PreviousInstanceId { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? NextAttemptAtUtc { get; set; }
    public DateTime? LeaseUntilUtc { get; set; }
    public Guid? ExecutionToken { get; set; }
    public int Attempts { get; set; }
    public string? FailureCode { get; set; }
}
public sealed class UserTask : WorkflowAuditable
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid InstanceId { get; set; }
    public int ClientId { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = "";
    public int StepIndex { get; set; }
    public int Generation { get; set; }
    public string Status { get; set; } = "Pending";
    public string? Decision { get; set; }
    public DateTime? DecidedAtUtc { get; set; }
}
public sealed class AuditEvent : WorkflowAuditable
{
    public long Id { get; set; }
    public Guid? InstanceId { get; set; }
    public int ClientId { get; set; }
    public int UserId { get; set; }
    public string Kind { get; set; } = "";
    public string Comment { get; set; } = "";
    public Guid? IdempotencyKey { get; set; }
    public string? RequestHash { get; set; }
    public DateTime AtUtc { get; set; } = DateTime.UtcNow;
}
public sealed class Notification : WorkflowAuditable
{
    public long Id { get; set; }
    public Guid InstanceId { get; set; }
    public int ClientId { get; set; }
    public int UserId { get; set; }
    public string Kind { get; set; } = "";
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? ReadAtUtc { get; set; }
}
public sealed class OutboxMessage : WorkflowAuditable
{
    public long Id { get; set; }
    public Guid InstanceId { get; set; }
    public int ClientId { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? DeliveredAtUtc { get; set; }
}
