using System.Text.Json;
using EDXS.Shared.Workflow;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Workflow.Service.Data;
using Workflow.Service.Services;

namespace Workflow.Service.Api;

[ApiController, Authorize, Route("api/Workflow/v3")]
public sealed class WorkflowController(WorkflowDbContext db, WorkflowEngine engine, WorkflowHttpClient serviceClient) : ControllerBase
{
    private Task<WorkflowIdentity> Actor(CancellationToken ct) => engine.IdentityAsync(WorkflowActor.From(HttpContext) with { EntityId = null }, ct);
    private static void Admin(WorkflowIdentity actor) { if (!actor.IsTenantAdmin && !actor.IsPlatformAdmin) throw new WorkflowException(403, "AdminRequired", "Workflow administration permission is required."); }
    private static void ConfigurationAdmin(WorkflowIdentity actor)
    {
        if (!actor.IsPlatformAdmin) throw new WorkflowException(403, "PlatformAdminRequired", "Only superadmins can access Workflow Management.");
    }
    private async Task<int> ConfigClient(WorkflowIdentity actor, string scope, int? tenantId, CancellationToken ct)
    {
        ConfigurationAdmin(actor);
        if (scope == "global") return 0;
        if (scope != "tenant") throw new WorkflowException(400, "InvalidScope", "Choose tenant or global configuration.");
        // Require an explicit target; never accidentally configure the superadmin's own tenant.
        var clientId = tenantId ?? 0;
        if (clientId <= 0) throw new WorkflowException(400, "TenantRequired", "Select a tenant to configure.");
        await serviceClient.PostAsync<WorkflowLookup>("auth", "/internal/workflow/tenant", new { clientId }, ct);
        return clientId;
    }
    private void ConfigAudit(int clientId, int userId, string kind, string reference) => db.Events.Add(WorkflowAudit.Added(new AuditEvent { ClientId = clientId, UserId = userId, Kind = kind, Comment = reference }, userId));

    [HttpGet("me")]
    public async Task<object> Me(CancellationToken ct) => await Actor(ct);
    [HttpGet("tenants")]
    public async Task<object> Tenants(CancellationToken ct)
    {
        var actor = await Actor(ct); ConfigurationAdmin(actor);
        return await serviceClient.PostAsync<List<WorkflowLookup>>("auth", "/internal/workflow/tenants", new IdentityRequest(new(actor.UserId, actor.ClientId, null)), ct);
    }
    [HttpGet("screens")]
    public async Task<object> Screens(CancellationToken ct)
    {
        var actor = await Actor(ct); ConfigurationAdmin(actor);
        return await serviceClient.PostAsync<List<WorkflowScreen>>("auth", "/internal/workflow/screens", new { }, ct);
    }
    [HttpGet("actions")]
    public async Task<object> Actions(CancellationToken ct)
    {
        await Actor(ct);
        return (await db.Actions.AsNoTracking().OrderBy(x => x.ScreenKey).ToListAsync(ct)).Select(x => JsonSerializer.Deserialize<ActionRegistration>(x.RegistrationJson, WorkflowJson.Options));
    }
    [HttpGet("directory")]
    public async Task<object> Directory([FromQuery] int? clientId, CancellationToken ct)
    {
        var actor = await Actor(ct); var client = await ConfigClient(actor, "tenant", clientId, ct);
        return await serviceClient.PostAsync<WorkflowDirectory>("auth", "/internal/workflow/directory", new { clientId = client }, ct);
    }

    private static object DefinitionView(Definition x) => new { x.Id, x.ClientId, x.Name, x.ScreenKey, x.ActionKey, x.Status, x.Version, x.PreviousVersionId,
        steps = JsonSerializer.Deserialize<List<WorkflowStep>>(x.StepsJson, WorkflowJson.Options) };
    [HttpGet("definitions")]
    public async Task<object> Definitions([FromQuery] string scope = "tenant", [FromQuery] int? clientId = null, CancellationToken ct = default)
    {
        var client = await ConfigClient(await Actor(ct), scope, clientId, ct);
        return (await db.Definitions.AsNoTracking().Where(x => x.ClientId == client || x.ClientId == 0).OrderByDescending(x => x.CreatedAtUtc).ToListAsync(ct)).Select(DefinitionView);
    }
    public sealed record SaveDefinition(string Scope, DefinitionDraft Draft, int ExpectedVersion = 0, Guid? PreviousVersionId = null, int? ClientId = null);
    [HttpPost("definitions")]
    public async Task<object> CreateDefinition(SaveDefinition request, CancellationToken ct)
    {
        var actor = await Actor(ct); var client = await ConfigClient(actor, request.Scope, request.ClientId, ct);
        await engine.RegistrationAsync(request.Draft.ScreenKey, ct);
        if (request.PreviousVersionId.HasValue && !await db.Definitions.AnyAsync(x => x.Id == request.PreviousVersionId &&
            (x.ClientId == client || x.ClientId == 0) && x.ScreenKey == request.Draft.ScreenKey && x.ActionKey == request.Draft.ActionKey, ct))
            throw new WorkflowException(400, "InvalidSource", "The source workflow version is unavailable.");
        var definition = new Definition { ClientId = client, ScreenKey = request.Draft.ScreenKey, ActionKey = request.Draft.ActionKey,
            Name = request.Draft.Name.Trim(), StepsJson = JsonSerializer.Serialize(request.Draft.Steps, WorkflowJson.Options), CreatedBy = actor.UserId, PreviousVersionId = request.PreviousVersionId };
        if (definition.Name.Length is < 1 or > 150 || definition.ActionKey is not ("Save" or "Delete")) throw new WorkflowException(400, "InvalidDefinition", "Name and Save/Delete action are required.");
        db.Definitions.Add(WorkflowAudit.Added(definition, actor.UserId)); ConfigAudit(client, actor.UserId, "DefinitionCreated", definition.Id.ToString());
        await db.SaveChangesAsync(ct); return DefinitionView(definition);
    }
    private async Task<Definition> ConfigDefinition(Guid id, WorkflowIdentity actor, CancellationToken ct)
    {
        ConfigurationAdmin(actor);
        var definition = await db.Definitions.SingleOrDefaultAsync(x => x.Id == id, ct) ?? throw new WorkflowException(404, "NotFound", "Workflow definition was not found.");
        await ConfigClient(actor, definition.ClientId == 0 ? "global" : "tenant", definition.ClientId == 0 ? null : definition.ClientId, ct);
        return definition;
    }
    [HttpPut("definitions/{id:guid}")]
    public async Task<object> UpdateDefinition(Guid id, SaveDefinition request, CancellationToken ct)
    {
        var actor = await Actor(ct); var definition = await ConfigDefinition(id, actor, ct);
        if (definition.Status != "Draft" || definition.Version != request.ExpectedVersion) throw new WorkflowException(409, "ImmutableVersion", "Edit a current draft or create a new version of a published workflow.");
        if (definition.ScreenKey != request.Draft.ScreenKey || definition.ActionKey != request.Draft.ActionKey) throw new WorkflowException(400, "IdentityChanged", "Create a new definition to change its screen or action.");
        if (string.IsNullOrWhiteSpace(request.Draft.Name) || request.Draft.Name.Length > 150 || request.Draft.Steps.Count > 30) throw new WorkflowException(400, "InvalidDefinition", "Check the workflow name and step count.");
        definition.Name = request.Draft.Name.Trim(); definition.StepsJson = JsonSerializer.Serialize(request.Draft.Steps, WorkflowJson.Options); definition.Version++;
        WorkflowAudit.Edited(definition, actor.UserId);
        ConfigAudit(definition.ClientId, actor.UserId, "DefinitionEdited", id.ToString());
        await db.SaveChangesAsync(ct); return DefinitionView(definition);
    }
    public sealed record PublishDefinition(int ExpectedVersion, int ExpectedPolicyVersion);
    [HttpPost("definitions/{id:guid}/publish")]
    public async Task<object> Publish(Guid id, PublishDefinition request, CancellationToken ct)
    {
        var actor = await Actor(ct); var definition = await ConfigDefinition(id, actor, ct);
        if (definition.Status != "Draft" || definition.Version != request.ExpectedVersion) throw new WorkflowException(409, "VersionConflict", "Refresh the current draft before publishing.");
        var draft = new DefinitionDraft(definition.ScreenKey, definition.ActionKey, definition.Name, JsonSerializer.Deserialize<List<WorkflowStep>>(definition.StepsJson, WorkflowJson.Options)!);
        await engine.RegistrationAsync(definition.ScreenKey, ct);
        var errors = DefinitionRules.Validate(draft, definition.ClientId == 0);
        if (errors.Length > 0) throw new WorkflowException(400, "InvalidDefinition", string.Join(" ", errors));
        var groupKeys = draft.Steps.SelectMany(x => x.Assignees).Where(x => x.Kind == "Group").Select(x => x.Id).Distinct().ToArray();
        if (await db.Groups.CountAsync(x => groupKeys.Contains(x.Key), ct) != groupKeys.Length) throw new WorkflowException(400, "MissingGroup", "Create all referenced logical approver groups first.");
        if (definition.ClientId > 0) await ValidateSelectors(definition.ClientId, draft.Steps.SelectMany(x => x.Assignees).Where(x => x.Kind != "Group").ToList(), ct);
        var policy = await GetPolicy(definition.ClientId, definition.ScreenKey, definition.ActionKey, request.ExpectedPolicyVersion, ct);
        policy.Mode = "Custom"; policy.DefinitionId = definition.Id; WorkflowAudit.Edited(policy, actor.UserId);
        definition.Status = "Published"; definition.Version++; WorkflowAudit.Edited(definition, actor.UserId);
        // Only one version per scope/screen/action is published (live); earlier ones are retired so
        // the list never shows a stale "Published" version that no longer gates submissions.
        var superseded = await db.Definitions.Where(x => x.Id != id && x.Status == "Published" && x.ClientId == definition.ClientId
            && x.ScreenKey == definition.ScreenKey && x.ActionKey == definition.ActionKey).ToListAsync(ct);
        foreach (var previous in superseded)
        {
            previous.Status = "Archived"; previous.Version++; WorkflowAudit.Edited(previous, actor.UserId);
            ConfigAudit(previous.ClientId, actor.UserId, "DefinitionArchived", $"{previous.Id} (superseded by {id})");
        }
        ConfigAudit(definition.ClientId, actor.UserId, "DefinitionPublished", id.ToString());
        await db.SaveChangesAsync(ct); return DefinitionView(definition);
    }
    [HttpDelete("definitions/{id:guid}")]
    public async Task<object> DeleteDefinition(Guid id, [FromQuery] int expectedVersion, CancellationToken ct)
    {
        var actor = await Actor(ct); var definition = await ConfigDefinition(id, actor, ct);
        if (definition.Version != expectedVersion || definition.Status != "Draft") throw new WorkflowException(409, "ImmutableVersion", "Only a current draft can be deleted.");
        db.Definitions.Remove(definition); ConfigAudit(definition.ClientId, actor.UserId, "DraftDeleted", id.ToString());
        await db.SaveChangesAsync(ct); return new { success = true };
    }
    [HttpPost("definitions/{id:guid}/archive")]
    public async Task<object> Archive(Guid id, PublishDefinition request, CancellationToken ct)
    {
        var actor = await Actor(ct); var definition = await ConfigDefinition(id, actor, ct);
        if (definition.Version != request.ExpectedVersion) throw new WorkflowException(409, "VersionConflict", "Refresh before archiving.");
        var policy = await db.Policies.SingleOrDefaultAsync(x => x.ClientId == definition.ClientId && x.ScreenKey == definition.ScreenKey && x.ActionKey == definition.ActionKey, ct);
        if (policy?.DefinitionId == id)
        {
            if (policy.Version != request.ExpectedPolicyVersion) throw new WorkflowException(409, "VersionConflict", "The effective workflow changed.");
            policy.Mode = "Inherit"; policy.DefinitionId = null; policy.Version++; WorkflowAudit.Edited(policy, actor.UserId);
        }
        definition.Status = "Archived"; definition.Version++; WorkflowAudit.Edited(definition, actor.UserId);
        ConfigAudit(definition.ClientId, actor.UserId, "DefinitionArchived", id.ToString());
        await db.SaveChangesAsync(ct); return new { success = true };
    }
    [HttpPost("definitions/{id:guid}/unpublish")]
    public async Task<object> Unpublish(Guid id, PublishDefinition request, CancellationToken ct)
    {
        var actor = await Actor(ct); var definition = await ConfigDefinition(id, actor, ct);
        if (definition.Status != "Published" || definition.Version != request.ExpectedVersion) throw new WorkflowException(409, "VersionConflict", "Only the current published version can be unpublished. Refresh and retry.");
        var policy = await db.Policies.SingleOrDefaultAsync(x => x.ClientId == definition.ClientId && x.ScreenKey == definition.ScreenKey && x.ActionKey == definition.ActionKey, ct);
        if (policy?.DefinitionId == id)
        {
            if (policy.Version != request.ExpectedPolicyVersion) throw new WorkflowException(409, "VersionConflict", "The effective workflow changed.");
            policy.Mode = "Inherit"; policy.DefinitionId = null; policy.Version++; WorkflowAudit.Edited(policy, actor.UserId);
        }
        // Running requests keep their own step snapshot, so returning the definition to an editable draft is safe.
        definition.Status = "Draft"; definition.Version++; WorkflowAudit.Edited(definition, actor.UserId);
        ConfigAudit(definition.ClientId, actor.UserId, "DefinitionUnpublished", id.ToString());
        await db.SaveChangesAsync(ct); return DefinitionView(definition);
    }
    public sealed record UnarchiveDefinition(int ExpectedVersion);
    [HttpPost("definitions/{id:guid}/unarchive")]
    public async Task<object> Unarchive(Guid id, UnarchiveDefinition request, CancellationToken ct)
    {
        var actor = await Actor(ct); var definition = await ConfigDefinition(id, actor, ct);
        if (definition.Status != "Archived" || definition.Version != request.ExpectedVersion) throw new WorkflowException(409, "VersionConflict", "Only an archived version can be unarchived. Refresh and retry.");
        // Restored as a draft: it must be published again (replacing the live version) before it gates submissions.
        definition.Status = "Draft"; definition.Version++; WorkflowAudit.Edited(definition, actor.UserId);
        ConfigAudit(definition.ClientId, actor.UserId, "DefinitionUnarchived", id.ToString());
        await db.SaveChangesAsync(ct); return DefinitionView(definition);
    }

    [HttpGet("policies")]
    public async Task<object> Policies([FromQuery] string scope = "tenant", [FromQuery] int? clientId = null, CancellationToken ct = default)
    {
        var client = await ConfigClient(await Actor(ct), scope, clientId, ct);
        return await db.Policies.AsNoTracking().Where(x => x.ClientId == client || x.ClientId == 0).ToListAsync(ct);
    }
    private async Task<Policy> GetPolicy(int client, string screen, string action, int expected, CancellationToken ct)
    {
        var policy = await db.Policies.SingleOrDefaultAsync(x => x.ClientId == client && x.ScreenKey == screen && x.ActionKey == action, ct);
        if ((policy?.Version ?? 0) != expected) throw new WorkflowException(409, "VersionConflict", "The effective workflow changed. Refresh and retry.");
        if (policy == null) { policy = WorkflowAudit.Added(new Policy { ClientId = client, ScreenKey = screen, ActionKey = action }); db.Policies.Add(policy); }
        else policy.Version++;
        return policy;
    }
    public sealed record SetPolicy(string Scope, string ScreenKey, string ActionKey, string Mode, int ExpectedVersion, int? ClientId = null);
    [HttpPut("policies")]
    public async Task<object> Policy(SetPolicy request, CancellationToken ct)
    {
        var actor = await Actor(ct); var client = await ConfigClient(actor, request.Scope, request.ClientId, ct);
        if (request.Mode is not ("Inherit" or "Disabled") || request.ActionKey is not ("Save" or "Delete") || client == 0 && request.Mode == "Disabled")
            throw new WorkflowException(400, "InvalidPolicy", "Choose Inherit or, for a tenant, Disabled. Publish a draft to replace the workflow.");
        await engine.RegistrationAsync(request.ScreenKey, ct);
        var policy = await GetPolicy(client, request.ScreenKey, request.ActionKey, request.ExpectedVersion, ct);
        policy.Mode = request.Mode; policy.DefinitionId = null; WorkflowAudit.Edited(policy, actor.UserId);
        ConfigAudit(client, actor.UserId, "PolicyChanged", $"{request.ScreenKey}/{request.ActionKey}: {request.Mode}");
        await db.SaveChangesAsync(ct); return policy;
    }

    [HttpGet("groups")]
    public async Task<object> Groups(CancellationToken ct) { ConfigurationAdmin(await Actor(ct)); return await db.Groups.AsNoTracking().OrderBy(x => x.Name).ToListAsync(ct); }
    [HttpPost("groups")]
    public async Task<object> AddGroup(ApproverGroup request, CancellationToken ct)
    {
        var actor = await Actor(ct);
        ConfigurationAdmin(actor);
        if (!System.Text.RegularExpressions.Regex.IsMatch(request.Key, "^[A-Za-z][A-Za-z0-9_.-]{0,99}$") || string.IsNullOrWhiteSpace(request.Name) || request.Name.Length > 150)
            throw new WorkflowException(400, "InvalidGroup", "Use a stable group key and a name of at most 150 characters.");
        if (await db.Groups.AnyAsync(x => x.Key == request.Key, ct)) throw new WorkflowException(409, "DuplicateGroup", "This group key already exists.");
        // The group arrives from the request body, so its audit block is stamped here rather than by a constructor.
        db.Groups.Add(WorkflowAudit.Added(request, actor.UserId)); ConfigAudit(0, actor.UserId, "GroupCreated", request.Key); await db.SaveChangesAsync(ct); return request;
    }
    [HttpDelete("groups/{key}")]
    public async Task<object> DeleteGroup(string key, CancellationToken ct)
    {
        var actor = await Actor(ct); ConfigurationAdmin(actor);
        var referenced = (await db.Definitions.AsNoTracking().Where(x => x.Status != "Archived").Select(x => x.StepsJson).ToListAsync(ct))
            .Any(json => JsonSerializer.Deserialize<List<WorkflowStep>>(json, WorkflowJson.Options)!.SelectMany(x => x.Assignees).Any(x => x.Kind == "Group" && x.Id == key));
        // Retained instance snapshots may still resolve a later step using this group.
        referenced |= (await db.Instances.AsNoTracking().Where(x => !WorkflowEngine.ClosedStates.Contains(x.Status)).Select(x => x.StepsJson).ToListAsync(ct))
            .Any(json => JsonSerializer.Deserialize<List<WorkflowStep>>(json, WorkflowJson.Options)!.SelectMany(x => x.Assignees).Any(x => x.Kind == "Group" && x.Id == key));
        if (referenced || await db.Bindings.AnyAsync(x => x.GroupKey == key, ct)) throw new WorkflowException(409, "GroupInUse", "This group is referenced by workflows or tenant bindings.");
        var group = await db.Groups.FindAsync([key], ct); if (group != null) db.Groups.Remove(group);
        ConfigAudit(0, actor.UserId, "GroupDeleted", key); await db.SaveChangesAsync(ct); return new { success = true };
    }
    [HttpGet("bindings")]
    public async Task<object> Bindings([FromQuery] int? clientId, CancellationToken ct)
    {
        var client = await ConfigClient(await Actor(ct), "tenant", clientId, ct);
        return (await db.Bindings.AsNoTracking().Where(x => x.ClientId == client).ToListAsync(ct)).Select(x => new { x.GroupKey, x.Version, selectors = JsonSerializer.Deserialize<List<AssigneeSelector>>(x.SelectorsJson, WorkflowJson.Options) });
    }
    public sealed record SetBinding(List<AssigneeSelector> Selectors, int ExpectedVersion, int? ClientId = null);
    private async Task ValidateSelectors(int clientId, List<AssigneeSelector> selectors, CancellationToken ct)
    {
        if (selectors.Count > 100 || selectors.Any(x => x.Kind is not ("User" or "Role") || !int.TryParse(x.Id, out var id) || id <= 0)) throw new WorkflowException(400, "InvalidSelectors", "Select tenant users or roles.");
        var directory = await serviceClient.PostAsync<WorkflowDirectory>("auth", "/internal/workflow/directory", new { clientId }, ct);
        if (selectors.Any(x => x.Kind == "User" ? !directory.Users.Any(u => u.UserId.ToString() == x.Id) : !directory.Roles.Any(r => r.Id.ToString() == x.Id)))
            throw new WorkflowException(400, "InvalidSelectors", "One or more users or roles are not active in this tenant.");
    }
    [HttpPut("bindings/{key}")]
    public async Task<object> Bind(string key, SetBinding request, CancellationToken ct)
    {
        var actor = await Actor(ct); var client = await ConfigClient(actor, "tenant", request.ClientId, ct);
        if (!await db.Groups.AnyAsync(x => x.Key == key, ct)) throw new WorkflowException(404, "NotFound", "Approver group was not found.");
        await ValidateSelectors(client, request.Selectors, ct);
        var binding = await db.Bindings.SingleOrDefaultAsync(x => x.ClientId == client && x.GroupKey == key, ct);
        if ((binding?.Version ?? 0) != request.ExpectedVersion) throw new WorkflowException(409, "VersionConflict", "Refresh these group bindings before editing.");
        if (binding == null) { binding = WorkflowAudit.Added(new GroupBinding { ClientId = client, GroupKey = key }, actor.UserId); db.Bindings.Add(binding); }
        else { binding.Version++; WorkflowAudit.Edited(binding, actor.UserId); }
        binding.SelectorsJson = JsonSerializer.Serialize(request.Selectors.Distinct().ToList(), WorkflowJson.Options);
        ConfigAudit(client, actor.UserId, "GroupBound", key); await db.SaveChangesAsync(ct);
        return new { binding.GroupKey, binding.Version, selectors = request.Selectors };
    }
    public sealed record SimulateRequest(DefinitionDraft Draft, int RequesterId, int? EntityId, int? ClientId = null);
    [HttpPost("simulate")]
    public async Task<object> Simulate(SimulateRequest request, CancellationToken ct)
    {
        var actor = await Actor(ct); var client = await ConfigClient(actor, "tenant", request.ClientId, ct);
        var errors = DefinitionRules.Validate(request.Draft, false);
        var instance = new Instance { ClientId = client, EntityId = request.EntityId, SubmitterId = request.RequesterId };
        var results = new List<object>();
        foreach (var step in request.Draft.Steps.Take(30))
        {
            var resolved = await engine.ResolveAsync(instance, step, ct);
            results.Add(new { step.Name, users = resolved.Users, blocked = resolved.Users.Count == 0, step.RelatedUser, resolved.RelatedUserMissing });
        }
        return new { errors, steps = results };
    }

    [HttpGet("instances")]
    public async Task<object> Instances([FromQuery] string view = "pending", [FromQuery] string? screenKey = null, [FromQuery] string? status = null,
        [FromQuery] string? action = null, [FromQuery] int? submitterId = null, [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 25, CancellationToken ct = default)
    {
        var actor = await Actor(ct); var query = db.Instances.AsNoTracking().Where(x => x.ClientId == actor.ClientId);
        query = view switch
        {
            "mine" => query.Where(x => x.SubmitterId == actor.UserId),
            "history" => query.Where(x => x.SubmitterId == actor.UserId || db.Tasks.Any(t => t.InstanceId == x.Id && t.UserId == actor.UserId)),
            "admin" when actor.IsTenantAdmin || actor.IsPlatformAdmin => query,
            "pending" => query.Where(x => x.Status == "Pending" && db.Tasks.Any(t => t.InstanceId == x.Id && t.UserId == actor.UserId && t.Status == "Pending")),
            _ => throw new WorkflowException(403, "InvalidView", "This workflow view is not available.")
        };
        if (!string.IsNullOrEmpty(screenKey)) query = query.Where(x => x.ScreenKey == screenKey);
        if (!string.IsNullOrEmpty(status)) query = query.Where(x => x.Status == status);
        if (action == "Save") query = query.Where(x => x.Operation != "Delete"); else if (action == "Delete") query = query.Where(x => x.Operation == "Delete");
        if (submitterId.HasValue) query = query.Where(x => x.SubmitterId == submitterId);
        if (from.HasValue) query = query.Where(x => x.CreatedAtUtc >= from); if (to.HasValue) query = query.Where(x => x.CreatedAtUtc <= to);
        var count = await query.CountAsync(ct); page = Math.Max(1, page); pageSize = Math.Clamp(pageSize, 1, 100);
        var rows = await query.OrderByDescending(x => x.CreatedAtUtc).Skip((page - 1) * pageSize).Take(pageSize)
            .Select(x => new { x.Id, x.ScreenKey, x.Operation, x.Subject, x.Status, x.SubmitterId, x.SubmitterName, x.EntityId, x.RecordKey, x.StepIndex, x.DefinitionName, x.CreatedAtUtc, x.UpdatedAtUtc, x.Version, x.FailureCode }).ToListAsync(ct);
        return new { items = rows, totalCount = count, page, pageSize };
    }
    [HttpGet("summary")]
    public async Task<object> Summary(CancellationToken ct)
    {
        var actor = await Actor(ct);
        var unread = await db.Notifications.CountAsync(x => x.ClientId == actor.ClientId && x.UserId == actor.UserId && x.ReadAtUtc == null, ct);
        var pending = await db.Tasks.CountAsync(x => x.ClientId == actor.ClientId && x.UserId == actor.UserId && x.Status == "Pending", ct);
        return new { unread, pending };
    }
    [HttpGet("notifications")]
    public async Task<object> Notifications(CancellationToken ct)
    {
        var actor = await Actor(ct);
        var rows = await (from notice in db.Notifications.AsNoTracking()
                          join instance in db.Instances.AsNoTracking() on notice.InstanceId equals instance.Id
                          where notice.ClientId == actor.ClientId && notice.UserId == actor.UserId && instance.ClientId == actor.ClientId
                          orderby notice.CreatedAtUtc descending
                          select new { notice.Id, notice.InstanceId, notice.Kind, notice.CreatedAtUtc, notice.ReadAtUtc, instance.Subject, instance.ScreenKey,
                              instance.Operation, instance.SubmitterId, instance.SubmitterName }).Take(20).ToListAsync(ct);
        // The registered actions are a handful of rows; their display names turn screen keys into readable text.
        var screens = (await db.Actions.AsNoTracking().ToListAsync(ct))
            .ToDictionary(x => x.ScreenKey, x => JsonSerializer.Deserialize<ActionRegistration>(x.RegistrationJson, WorkflowJson.Options)?.Name ?? x.ScreenKey);
        return rows.Select(x => new { x.Id, x.InstanceId, x.Kind, x.CreatedAtUtc, x.ReadAtUtc, x.Subject, x.ScreenKey, x.SubmitterName,
            message = WorkflowNotices.Message(x.Kind, screens.GetValueOrDefault(x.ScreenKey, x.ScreenKey), x.Operation, x.Subject, x.SubmitterName, x.SubmitterId == actor.UserId) });
    }
    [HttpGet("operations")]
    public async Task<object> Operations(CancellationToken ct)
    {
        var actor = await Actor(ct); Admin(actor);
        var query = db.Instances.AsNoTracking().Where(x => x.ClientId == actor.ClientId);
        var states = await query.GroupBy(x => x.Status).Select(g => new { status = g.Key, count = g.Count(), oldestCreatedAtUtc = g.Min(x => x.CreatedAtUtc) }).ToListAsync(ct);
        return new { states, deliveryBacklog = await db.Outbox.CountAsync(x => x.ClientId == actor.ClientId && x.DeliveredAtUtc == null, ct),
            executionAttention = await query.CountAsync(x => x.Status == "Executing" && x.NextAttemptAtUtc == null, ct) };
    }
    // Who may change a request's values is decided by the edit right on its screen, so that right is read with
    // the identity, but only while the values can still change; otherwise the right is not needed (CanSubmit
    // then means nothing, and WorkflowEngine.AmendBlocker refuses on the request's state first).
    private async Task<(Instance Instance, WorkflowIdentity Actor)> RecordAsync(Guid id, CancellationToken ct)
    {
        var claims = WorkflowActor.From(HttpContext) with { EntityId = null };
        var instance = await db.Instances.SingleOrDefaultAsync(x => x.Id == id && x.ClientId == claims.ClientId, ct)
            ?? throw new WorkflowException(404, "NotFound", "Workflow request was not found.");
        var screen = WorkflowEngine.Amendable(instance) && instance.SubmitterId != claims.UserId ? instance.ScreenKey : null;
        var actor = await serviceClient.PostAsync<WorkflowIdentity>("auth", "/internal/workflow/identity",
            new IdentityRequest(claims, screen, screen == null ? null : "Update", RequireRight: false), ct);
        await engine.CheckAccessAsync(instance, actor, ct);
        return (instance, actor);
    }
    [HttpGet("instances/{id:guid}")]
    public async Task<object> Detail(Guid id, CancellationToken ct)
    {
        var (instance, actor) = await RecordAsync(id, ct);
        var tasks = await db.Tasks.AsNoTracking().Where(x => x.InstanceId == id).OrderBy(x => x.StepIndex).ThenBy(x => x.Generation).ToListAsync(ct);
        var events = await db.Events.AsNoTracking().Where(x => x.InstanceId == id && x.Kind != "PayloadViewed").OrderBy(x => x.AtUtc).ToListAsync(ct);
        var users = (await serviceClient.PostAsync<List<WorkflowUser>>("auth", "/internal/workflow/users",
            new UserNamesRequest(instance.ClientId, tasks.Select(x => x.UserId).Concat(events.Select(x => x.UserId)).ToArray()), ct)).ToDictionary(x => x.UserId);
        var registration = await engine.RegistrationAsync(instance.ScreenKey, ct);
        // Assignment determines decision rights. Older unavailable details do not erase that
        // assignment; saving the approved business record still requires the original payload.
        PreparedAction? payload = null; string? payloadError = null;
        try { payload = engine.Payload(instance); }
        catch (WorkflowException e) when (e.Code is "PayloadUnavailable" or "PayloadInvalid" or "PayloadChanged") { payloadError = e.Message; }
        var steps = WorkflowEngine.Steps(instance);
        var current = tasks.Where(x => x.StepIndex == instance.StepIndex && x.Generation == instance.Generation).ToList();
        var canDecide = instance.Status == "Pending" && current.Any(x => x.UserId == actor.UserId && x.Status == "Pending");
        var amendBlocked = payloadError == null ? WorkflowEngine.AmendBlocker(instance, actor, current)?.Message : null;
        // A decision names the step it settled: the decider's task holding that decision, closest in time.
        string? StepOf(AuditEvent e) => tasks.Where(t => t.UserId == e.UserId && t.Decision == e.Kind && t.DecidedAtUtc != null)
            .OrderBy(t => Math.Abs((t.DecidedAtUtc!.Value - e.AtUtc).Ticks)).Select(t => steps.ElementAtOrDefault(t.StepIndex)?.Name).FirstOrDefault();
        var canStop = instance.Status is "Pending" or "ConfigurationBlocked" or "PendingExecution";
        var terminal = WorkflowEngine.ClosedStates.Contains(instance.Status);
        engine.Audit(instance, actor.UserId, "PayloadViewed"); await db.SaveChangesAsync(ct);
        return new { instance.Id, instance.ScreenKey, instance.Operation, instance.Subject, instance.Status, instance.RecordKey, instance.EntityId,
            instance.SubmitterId, instance.SubmitterName, instance.DefinitionName, instance.Version, instance.PreviousInstanceId, instance.CreatedAtUtc, instance.StepIndex,
            instance.Generation, instance.FailureCode, instance.Attempts, registration.Route, screenName = registration.Name, steps,
            tasks = tasks.Select(x => new { x.Id, x.UserId, x.UserName, roleName = users.GetValueOrDefault(x.UserId)?.RoleName ?? "No role", x.StepIndex, x.Generation, x.Status, x.Decision }),
            events = events.Select(x => new { x.Id, x.UserId, userName = x.UserId == 0 ? "Workflow system" : users.GetValueOrDefault(x.UserId)?.Name ?? "Unknown user",
                roleName = x.UserId == 0 ? null : users.GetValueOrDefault(x.UserId)?.RoleName, stepName = StepOf(x), x.Kind, x.Comment, x.AtUtc }),
            before = payload?.Before, after = payload?.After,
            payload = payload?.Payload, payloadError, canDecide, canAmend = payloadError == null && amendBlocked == null, amendBlocked, canComment = !terminal, canWithdraw = canStop && actor.UserId == instance.SubmitterId,
            canCancel = canStop && (actor.IsTenantAdmin || actor.IsPlatformAdmin), canResolve = (instance.Status is "Pending" or "ConfigurationBlocked") && (actor.IsTenantAdmin || actor.IsPlatformAdmin),
            canRetry = instance.Status == "Executing" && instance.NextAttemptAtUtc == null && (actor.IsTenantAdmin || actor.IsPlatformAdmin),
            canResubmit = actor.UserId == instance.SubmitterId && instance.Status is "Returned" or "Withdrawn" or "NeedsResubmission" };
    }
    [HttpPost("instances/{id:guid}/decisions")]
    public async Task<object> Decide(Guid id, DecisionRequest request, CancellationToken ct)
    {
        var actor = await Actor(ct); var instance = await engine.AccessibleAsync(id, actor, ct);
        await engine.DecideAsync(instance, actor, request, ct); return new { instance.Id, instance.Status, instance.Version };
    }
    [HttpPost("instances/{id:guid}/amend")]
    public async Task<object> Amend(Guid id, AmendRequest request, CancellationToken ct)
    {
        var (instance, actor) = await RecordAsync(id, ct);
        await engine.AmendAsync(instance, actor, request, ct); return new { instance.Id, instance.Status, instance.Version };
    }
    [HttpPost("instances/{id:guid}/commands/{command}")]
    public async Task<object> Command(Guid id, string command, InstanceCommand request, CancellationToken ct)
    {
        var actor = await Actor(ct); var instance = await engine.AccessibleAsync(id, actor, ct);
        await engine.CommandAsync(instance, actor, command, request, ct); return new { instance.Id, instance.Status, instance.Version };
    }
    [HttpPost("instances/{id:guid}/comments")]
    public async Task<object> Comment(Guid id, InstanceCommand request, CancellationToken ct)
    {
        var actor = await Actor(ct); var instance = await engine.AccessibleAsync(id, actor, ct);
        WorkflowEngine.CheckVersion(instance, request.ExpectedVersion); WorkflowEngine.ValidateComment(request.Comment, true);
        if (WorkflowEngine.ClosedStates.Contains(instance.Status)) throw new WorkflowException(409, "RequestClosed", "This workflow is closed.");
        WorkflowEngine.Touch(instance); engine.Audit(instance, actor.UserId, "Comment", request.Comment);
        await engine.NotifyAsync(instance, "Comment", ct); await db.SaveChangesAsync(ct); return new { instance.Version };
    }
    [HttpPost("instances/{id:guid}/read")]
    public async Task<object> Read(Guid id, CancellationToken ct)
    {
        var actor = await Actor(ct); await engine.AccessibleAsync(id, actor, ct);
        var rows = await db.Notifications.Where(x => x.InstanceId == id && x.ClientId == actor.ClientId && x.UserId == actor.UserId && x.ReadAtUtc == null).ToListAsync(ct);
        foreach (var row in rows) { row.ReadAtUtc = DateTime.UtcNow; WorkflowAudit.Edited(row, actor.UserId); }
        await db.SaveChangesAsync(ct); return new { success = true };
    }
}
