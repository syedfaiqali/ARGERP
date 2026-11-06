using System.Text.Json;
using EDXS.Shared.Workflow;
using Microsoft.EntityFrameworkCore;
using Workflow.Service.Data;

namespace Workflow.Service.Services;

public sealed class WorkflowEngine(WorkflowDbContext db, WorkflowHttpClient http, WorkflowPayloadStore payloads)
{
    public static readonly string[] ClosedStates = ["Completed", "Rejected", "Canceled", "Withdrawn", "Returned", "NeedsResubmission"];
    public static List<WorkflowStep> Steps(Instance instance) => JsonSerializer.Deserialize<List<WorkflowStep>>(instance.StepsJson, WorkflowJson.Options)!;
    public PreparedAction Payload(Instance instance)
    {
        var action = payloads.Read<PreparedAction>(instance.PayloadEnvelope, $"instance:{instance.ClientId}:{instance.Id}");
        if (WorkflowJson.Hash(action) != instance.PayloadHash || action.ClientId != instance.ClientId ||
            action.UserId != instance.SubmitterId || action.EntityId != instance.EntityId ||
            action.ScreenKey != instance.ScreenKey || action.Operation != instance.Operation)
            throw new WorkflowException(409, "PayloadChanged", "The submitted request details failed their integrity check.");
        return action;
    }

    public Task<WorkflowIdentity> IdentityAsync(WorkflowActor actor, CancellationToken ct) => http.PostAsync<WorkflowIdentity>("auth", "/internal/workflow/identity", new IdentityRequest(actor), ct);

    public async Task<SubmissionResult> LookupAsync(int clientId, int userId, Guid key, string hash, CancellationToken ct)
    {
        var instance = await db.Instances.AsNoTracking().SingleOrDefaultAsync(x => x.ClientId == clientId && x.SubmitterId == userId && x.IdempotencyKey == key, ct);
        if (instance == null) return new(false);
        if (instance.RequestHash != hash) throw new WorkflowException(409, "IdempotencyConflict", "This request identifier was used for different data.", instance.Id);
        return new(true, instance.Id, instance.Status, instance.Version);
    }

    public async Task<SubmissionResult> SubmitAsync(SubmitAction request, string service, CancellationToken ct)
    {
        var action = request.Action;
        var registration = await RegistrationAsync(action.ScreenKey, ct);
        if (registration.Service != service || request.IdempotencyKey == Guid.Empty || action.ClientId <= 0 || action.UserId <= 0 ||
            action.Operation is not ("Create" or "Update" or "Delete") || action.SchemaVersion != registration.SchemaVersion ||
            (registration.EntityScoped ? action.EntityId is null or <= 0 : action.EntityId != null) ||
            (action.Operation != "Create" && (string.IsNullOrEmpty(action.RecordKey) || string.IsNullOrEmpty(action.ExpectedHash))))
            throw new WorkflowException(400, "InvalidSubmission", "The action or record scope is invalid.");
        var prior = await LookupAsync(action.ClientId, action.UserId, request.IdempotencyKey, request.RequestHash, ct);
        if (prior.Pending) return prior;
        var identity = await http.PostAsync<WorkflowIdentity>("auth", "/internal/workflow/identity",
            new IdentityRequest(new(action.UserId, action.ClientId, action.EntityId), action.ScreenKey, action.Operation), ct);
        var openKey = action.RecordKey == null ? null : WorkflowJson.Hash(new { action.ClientId, action.EntityId, registration.ResourceType, action.RecordKey });
        if (openKey != null)
        {
            var other = await db.Instances.AsNoTracking().SingleOrDefaultAsync(x => x.OpenKey == openKey, ct);
            if (other != null) throw new WorkflowException(409, "PendingMutation", "This record already has a pending request.", other.Id);
        }
        if (request.PreviousInstanceId.HasValue)
        {
            var previous = await db.Instances.SingleOrDefaultAsync(x => x.Id == request.PreviousInstanceId && x.ClientId == action.ClientId && x.SubmitterId == action.UserId, ct);
            if (previous == null || previous.ScreenKey != action.ScreenKey || previous.RecordKey != action.RecordKey || previous.EntityId != action.EntityId ||
                previous.Operation != action.Operation || previous.Status is not ("Returned" or "Withdrawn" or "NeedsResubmission"))
                throw new WorkflowException(409, "InvalidRevision", "This request cannot be resubmitted as a revision of that workflow.");
        }
        var definition = await ResolveDefinitionAsync(action.ClientId, action.ScreenKey, action.Operation == "Delete" ? "Delete" : "Save", ct);
        if (definition == null) return new(false);
        var instance = new Instance
        {
            ClientId = action.ClientId, EntityId = action.EntityId, SubmitterId = action.UserId, SubmitterName = identity.Name,
            ScreenKey = action.ScreenKey, Operation = action.Operation, Subject = action.Subject, RecordKey = action.RecordKey,
            IdempotencyKey = request.IdempotencyKey, RequestHash = request.RequestHash, OpenKey = openKey,
            DefinitionId = definition.Id, DefinitionName = definition.Name, StepsJson = definition.StepsJson, PreviousInstanceId = request.PreviousInstanceId
        };
        instance.PayloadHash = WorkflowJson.Hash(action);
        instance.PayloadEnvelope = payloads.Write(action);
        db.Instances.Add(WorkflowAudit.Added(instance, identity.UserId));
        Audit(instance, identity.UserId, "Submitted");
        await ActivateAsync(instance, ct);
        await NotifyAsync(instance, "Submitted", ct);
        try { await db.SaveChangesAsync(ct); }
        catch (DbUpdateException)
        {
            db.ChangeTracker.Clear();
            var duplicate = await LookupAsync(action.ClientId, action.UserId, request.IdempotencyKey, request.RequestHash, ct);
            if (duplicate.Pending) return duplicate;
            var other = openKey == null ? null : await db.Instances.AsNoTracking().SingleOrDefaultAsync(x => x.OpenKey == openKey, ct);
            if (other != null) throw new WorkflowException(409, "PendingMutation", "This record already has a pending request.", other.Id);
            throw;
        }
        WorkflowTelemetry.Submissions.Add(1, new KeyValuePair<string, object?>("screen", instance.ScreenKey));
        return new(true, instance.Id, instance.Status, instance.Version);
    }

    public async Task<ActionRegistration> RegistrationAsync(string key, CancellationToken ct)
    {
        var row = await db.Actions.AsNoTracking().SingleOrDefaultAsync(x => x.ScreenKey == key, ct)
            ?? throw new WorkflowException(409, "ActionUnavailable", "This screen has not registered its workflow adapter.");
        return JsonSerializer.Deserialize<ActionRegistration>(row.RegistrationJson, WorkflowJson.Options)!;
    }

    public async Task<Definition?> ResolveDefinitionAsync(int clientId, string screen, string action, CancellationToken ct)
    {
        var policy = await db.Policies.AsNoTracking().SingleOrDefaultAsync(x => x.ClientId == clientId && x.ScreenKey == screen && x.ActionKey == action, ct);
        if (policy?.Mode == "Disabled") return null;
        if (policy?.Mode != "Custom") policy = await db.Policies.AsNoTracking().SingleOrDefaultAsync(x => x.ClientId == 0 && x.ScreenKey == screen && x.ActionKey == action, ct);
        if (policy?.Mode != "Custom") return null;
        return await db.Definitions.AsNoTracking().SingleOrDefaultAsync(x => x.Id == policy.DefinitionId && x.Status == "Published", ct)
            ?? throw new WorkflowException(503, "DefinitionUnavailable", "The effective workflow definition is unavailable.");
    }

    /// <summary>
    /// A configured related user takes precedence: when the requester's relationship resolves to eligible
    /// users, they alone are assigned. Only when it is missing do the step's group/role/user selectors
    /// apply, as a fallback; with no fallback configured the step blocks rather than skipping approval.
    /// </summary>
    public async Task<ResolvedAssignees> ResolveAsync(Instance instance, WorkflowStep step, CancellationToken ct)
    {
        if (step.RelatedUser == null) return new(await SelectorsAsync(instance, step, ct));
        var related = await http.PostAsync<List<ApproverUser>>("auth", "/internal/workflow/resolve",
            new ResolveUsersRequest(instance.ClientId, instance.EntityId, step.AssigneeScope, instance.SubmitterId, step.AllowSelfAction, [], step.RelatedUser), ct);
        if (related.Count > 0) return new(related);
        return new(step.Assignees.Count == 0 ? [] : await SelectorsAsync(instance, step, ct), RelatedUserMissing: true);
    }

    private async Task<List<ApproverUser>> SelectorsAsync(Instance instance, WorkflowStep step, CancellationToken ct)
    {
        var selectors = new List<AssigneeSelector>();
        foreach (var selector in step.Assignees)
        {
            if (selector.Kind != "Group") { selectors.Add(selector); continue; }
            var binding = await db.Bindings.AsNoTracking().SingleOrDefaultAsync(x => x.ClientId == instance.ClientId && x.GroupKey == selector.Id, ct);
            // A missing selector must block the whole step, including mixed direct/group selectors.
            if (binding == null) return [];
            var members = JsonSerializer.Deserialize<List<AssigneeSelector>>(binding.SelectorsJson, WorkflowJson.Options)!;
            if (members.Count == 0) return [];
            selectors.AddRange(members);
        }
        return await http.PostAsync<List<ApproverUser>>("auth", "/internal/workflow/resolve",
            new ResolveUsersRequest(instance.ClientId, instance.EntityId, step.AssigneeScope, instance.SubmitterId, step.AllowSelfAction, selectors), ct);
    }

    public async Task ActivateAsync(Instance instance, CancellationToken ct)
    {
        var steps = Steps(instance);
        if (instance.StepIndex == steps.Count)
        {
            instance.Status = "PendingExecution"; instance.NextAttemptAtUtc = DateTime.UtcNow; return;
        }
        var step = steps[instance.StepIndex];
        ResolvedAssignees resolved;
        try { resolved = await ResolveAsync(instance, step, ct); }
        catch (WorkflowException ex) when (ex.Status >= 500) { instance.Status = "ConfigurationBlocked"; instance.FailureCode = "DirectoryUnavailable"; return; }
        var users = resolved.Users;
        instance.Status = users.Count == 0 ? "ConfigurationBlocked" : "Pending";
        instance.FailureCode = users.Count > 0 ? null : resolved.RelatedUserMissing && step.Assignees.Count == 0 ? "RelatedUserNotFound" : "NoEligibleAssignees";
        if (step.RelatedUser != null)
        {
            // The history records whom the relationship reached, or that it fell back, so a blocked or
            // unexpected assignment can be traced to the requester's records rather than the definition.
            var relation = RelatedUsers.Label(step.RelatedUser);
            Audit(instance, 0, resolved.RelatedUserMissing ? "RelatedUserNotFound" : "RelatedUserAssigned", resolved.RelatedUserMissing
                ? $"{relation} could not be resolved for the requester. {(step.Assignees.Count == 0 ? "No fallback assignees are configured." : "The step's fallback assignees were used.")}"
                : $"{relation}: {string.Join(", ", users.DistinctBy(x => x.UserId).Select(x => x.Name))}");
        }
        foreach (var user in users.DistinctBy(x => x.UserId))
            db.Tasks.Add(WorkflowAudit.Added(new UserTask { InstanceId = instance.Id, ClientId = instance.ClientId, UserId = user.UserId, UserName = user.Name, StepIndex = instance.StepIndex, Generation = instance.Generation }, user.UserId));
    }

    public async Task<Instance> AccessibleAsync(Guid id, WorkflowIdentity identity, CancellationToken ct)
    {
        var instance = await db.Instances.SingleOrDefaultAsync(x => x.Id == id && x.ClientId == identity.ClientId, ct)
            ?? throw new WorkflowException(404, "NotFound", "Workflow request was not found.");
        await CheckAccessAsync(instance, identity, ct);
        return instance;
    }

    public async Task CheckAccessAsync(Instance instance, WorkflowIdentity identity, CancellationToken ct)
    {
        if (instance.ClientId != identity.ClientId) throw new WorkflowException(404, "NotFound", "Workflow request was not found.");
        if (instance.SubmitterId == identity.UserId || identity.IsTenantAdmin || identity.IsPlatformAdmin) return;
        var ownSteps = await db.Tasks.Where(x => x.InstanceId == instance.Id && x.UserId == identity.UserId).Select(x => x.StepIndex).Distinct().ToListAsync(ct);
        if (ownSteps.Count == 0)
            throw new WorkflowException(404, "NotFound", "Workflow request was not found.");
        if (instance.EntityId.HasValue && !identity.EntityIds.Contains(instance.EntityId.Value) &&
            !ownSteps.Any(index => Steps(instance)[index].AssigneeScope == "Tenant"))
            throw new WorkflowException(403, "CampusAccessRevoked", "Your record-campus access has been revoked.");
    }

    public async Task DecideAsync(Instance instance, WorkflowIdentity actor, DecisionRequest request, CancellationToken ct)
    {
        if (request.IdempotencyKey == Guid.Empty) throw new WorkflowException(400, "IdempotencyKeyRequired", "A decision identifier is required.");
        var hash = WorkflowJson.Hash(new { request.Decision, request.Comment, request.ExpectedVersion });
        var prior = await db.Events.AsNoTracking().SingleOrDefaultAsync(x => x.InstanceId == instance.Id && x.UserId == actor.UserId && x.IdempotencyKey == request.IdempotencyKey, ct);
        if (prior != null)
        {
            if (prior.RequestHash != hash) throw new WorkflowException(409, "IdempotencyConflict", "The decision identifier was already used for another decision.");
            return;
        }
        CheckVersion(instance, request.ExpectedVersion);
        if (instance.Status != "Pending") throw new WorkflowException(409, "TaskClosed", "This workflow is not awaiting a decision.");
        var tasks = await db.Tasks.Where(x => x.InstanceId == instance.Id && x.StepIndex == instance.StepIndex && x.Generation == instance.Generation).ToListAsync(ct);
        var own = tasks.SingleOrDefault(x => x.UserId == actor.UserId && x.Status == "Pending")
            ?? throw new WorkflowException(403, "NotAssigned", "Only an active assignee may decide this step.");
        var step = Steps(instance)[instance.StepIndex];
        // Snapshot membership is stable, but account/tenant access was freshly checked by IdentityAsync.
        if (actor.UserId == instance.SubmitterId && !step.AllowSelfAction) throw new WorkflowException(403, "SelfActionDenied", "You cannot decide your own request.");
        if (instance.EntityId.HasValue && step.AssigneeScope == "Campus" && !actor.IsTenantAdmin && !actor.EntityIds.Contains(instance.EntityId.Value))
            throw new WorkflowException(403, "CampusAccessRevoked", "Your access to this campus has been revoked.");
        var positive = step.DecisionKind == "Approval" ? "Approve" : "Recommend";
        var negative = step.DecisionKind == "Approval" ? "Reject" : "Decline";
        if (request.Decision != positive && request.Decision != negative && request.Decision != "Return")
            throw new WorkflowException(400, "InvalidDecision", "That decision is not offered by this step.");
        var required = request.Decision == "Return" || (request.Decision == positive ? step.RequirePositiveComment : step.RequireNegativeComment);
        ValidateComment(request.Comment, required);
        own.Decision = request.Decision; own.Status = "Decided"; own.DecidedAtUtc = DateTime.UtcNow;
        WorkflowAudit.Edited(own, actor.UserId);
        // Recorded before the transition so the history lists the decision ahead of what it caused.
        db.Events.Add(WorkflowAudit.Added(new AuditEvent { InstanceId = instance.Id, ClientId = instance.ClientId, UserId = actor.UserId, Kind = request.Decision,
            Comment = request.Comment?.Trim() ?? "", IdempotencyKey = request.IdempotencyKey, RequestHash = hash }, actor.UserId));
        // On an "end workflow on positive decision" step that is not the last one, approval completes the
        // workflow and skips the later steps. A rejection still follows NegativeBehavior: hard rejects the
        // request, soft passes it on to the next step. On the final step there is nothing to skip.
        var steps = Steps(instance);
        var remaining = steps.Count - instance.StepIndex - 1;
        var endingStep = step.EndOnPositiveDecision && step.DecisionKind == "Approval" && remaining > 0;
        var escalated = endingStep && request.Decision == negative && step.NegativeBehavior == "Continue";
        var terminal = request.Decision == "Return" ? "Returned" : request.Decision == negative && step.NegativeBehavior == "Terminate" ? "Rejected" : null;
        if (terminal != null) Close(instance, terminal, tasks);
        // A soft rejection on an ending step settles it at once, even under All: the step can no longer approve.
        else if (step.Quorum == "Any" || escalated || tasks.All(x => x.Status != "Pending"))
        {
            foreach (var task in tasks.Where(x => x.Status == "Pending")) { task.Status = "Canceled"; WorkflowAudit.Edited(task); }
            // Positive means every counted decision approved: the deciding one under Any, all of them under All.
            if (endingStep && tasks.Where(x => x.Status == "Decided").All(x => x.Decision == positive))
            {
                Audit(instance, 0, "StepsSkipped", $"Approval at step {instance.StepIndex + 1} ({step.Name}) ended the workflow; " +
                    (remaining == 1 ? $"step {steps.Count} was skipped." : $"steps {instance.StepIndex + 2}–{steps.Count} were skipped."));
                instance.StepIndex = steps.Count;
            }
            else
            {
                if (escalated) Audit(instance, 0, "Escalated", $"Rejection at step {instance.StepIndex + 1} ({step.Name}) passed the request to step {instance.StepIndex + 2} ({steps[instance.StepIndex + 1].Name}).");
                instance.StepIndex++;
            }
            instance.Generation = 0;
            await ActivateAsync(instance, ct);
        }
        Touch(instance);
        // Participants hear that the request moved on, not that it was rejected.
        await NotifyAsync(instance, escalated ? "Escalated" : request.Decision, ct);
        await db.SaveChangesAsync(ct);
        WorkflowTelemetry.Decisions.Add(1, new KeyValuePair<string, object?>("decision", request.Decision));
    }

    // A request's values can change while it still awaits approval; a deletion proposes none.
    public static bool Amendable(Instance instance) => instance.Operation != "Delete" && instance.Status is "Pending" or "ConfigurationBlocked";

    /// <summary>
    /// Why this user may not change the request's values, or null when they may. Permission decides, not
    /// assignment: the user's role must grant the edit right on the request's screen, which the caller reads
    /// into actor.CanSubmit (operation Update). A requester changes their own request by withdrawing it and
    /// submitting a revision, which restarts its approvals.
    /// </summary>
    public static WorkflowException? AmendBlocker(Instance instance, WorkflowIdentity actor, IEnumerable<UserTask> currentTasks)
    {
        if (instance.Operation == "Delete") return new(409, "NotEditable", "A deletion request has no values to change.");
        if (!Amendable(instance)) return new(409, "RequestClosed", "Only a request still awaiting approval can be changed.");
        if (actor.UserId == instance.SubmitterId) return new(403, "OwnRequest", "To change your own request, withdraw it and submit a revision.");
        if (!actor.CanSubmit) return new(403, "ScreenActionDenied", "Your role does not allow editing this screen.");
        if (instance.EntityId.HasValue && !actor.IsTenantAdmin && !actor.IsPlatformAdmin && !actor.EntityIds.Contains(instance.EntityId.Value))
            return new(403, "CampusAccessDenied", "You do not have access to this record's campus.");
        // A decision already taken in this step was taken on the current values.
        if (currentTasks.Any(x => x.Status == "Decided"))
            return new(409, "AlreadyDecided", "An approver of the current step has already decided on these values. Return the request for changes instead.");
        return null;
    }

    /// <summary>
    /// A user whose role may edit the screen changes the values a request awaiting approval proposes. The owning
    /// service re-validates them as the requester's submission; the request stays at its step and the history
    /// records who changed which values. The record itself still changes only after the final approval.
    /// </summary>
    public async Task AmendAsync(Instance instance, WorkflowIdentity actor, AmendRequest request, CancellationToken ct)
    {
        if (request.IdempotencyKey == Guid.Empty) throw new WorkflowException(400, "IdempotencyKeyRequired", "A change identifier is required.");
        var hash = WorkflowJson.Hash(new { request.Payload, request.ExpectedVersion });
        var prior = await db.Events.AsNoTracking().SingleOrDefaultAsync(x => x.InstanceId == instance.Id && x.UserId == actor.UserId && x.IdempotencyKey == request.IdempotencyKey, ct);
        if (prior != null)
        {
            if (prior.RequestHash != hash) throw new WorkflowException(409, "IdempotencyConflict", "The change identifier was already used for other values.");
            return;
        }
        CheckVersion(instance, request.ExpectedVersion);
        var current = await db.Tasks.AsNoTracking().Where(x => x.InstanceId == instance.Id && x.StepIndex == instance.StepIndex && x.Generation == instance.Generation).ToListAsync(ct);
        if (AmendBlocker(instance, actor, current) is { } blocked) throw blocked;
        var original = Payload(instance);
        var registration = await RegistrationAsync(instance.ScreenKey, ct);
        var amended = await http.PostAsync<PreparedAction>(registration.Service, "/internal/workflow/amend", new AmendAction(original, request.Payload), ct);
        // Only the proposed values are taken from the owning service; who, where and what the request changes stay as submitted.
        var action = original with { Subject = amended.Subject, Payload = amended.Payload, After = amended.After };
        var changes = WorkflowChanges.Describe(original.After, action.After);
        if (changes.Count == 0) throw new WorkflowException(400, "NoChanges", "No values were changed.");
        instance.Subject = action.Subject;
        instance.PayloadHash = WorkflowJson.Hash(action);
        instance.PayloadEnvelope = payloads.Write(action);
        Touch(instance);
        db.Events.Add(WorkflowAudit.Added(new AuditEvent { InstanceId = instance.Id, ClientId = instance.ClientId, UserId = actor.UserId, Kind = "Edited",
            Comment = $"{actor.Name} changed {instance.SubmitterName}'s {registration.Name}:\n{string.Join("\n", changes)}", IdempotencyKey = request.IdempotencyKey, RequestHash = hash }, actor.UserId));
        await NotifyAsync(instance, "Edited", ct);
        await db.SaveChangesAsync(ct);
    }

    public async Task CommandAsync(Instance instance, WorkflowIdentity actor, string command, InstanceCommand request, CancellationToken ct)
    {
        CheckVersion(instance, request.ExpectedVersion);
        if (command == "withdraw")
        {
            if (actor.UserId != instance.SubmitterId) throw new WorkflowException(403, "NotRequester", "Only the requester can withdraw this submission.");
        }
        else if (!actor.IsTenantAdmin && !actor.IsPlatformAdmin) throw new WorkflowException(403, "AdminRequired", "Workflow administration permission is required.");
        ValidateComment(request.Comment, command != "withdraw");
        var tasks = await db.Tasks.Where(x => x.InstanceId == instance.Id && x.Status == "Pending").ToListAsync(ct);
        switch (command)
        {
            case "withdraw": case "cancel":
                if (instance.Status is not ("Pending" or "ConfigurationBlocked" or "PendingExecution")) throw new WorkflowException(409, "CannotCancel", "This request has closed or execution has started.");
                Close(instance, command == "withdraw" ? "Withdrawn" : "Canceled", tasks);
                break;
            case "resolve":
                if (instance.Status is not ("Pending" or "ConfigurationBlocked")) throw new WorkflowException(409, "CannotResolve", "Only an active or blocked review step can be re-resolved.");
                // Re-resolution creates a new generation and restarts the whole active quorum.
                // Prior decisions remain in history and never count toward the new assignee set.
                foreach (var task in tasks) { task.Status = "Canceled"; WorkflowAudit.Edited(task, actor.UserId); }
                instance.Generation++;
                await ActivateAsync(instance, ct);
                break;
            case "retry":
                if (instance.Status != "Executing" || instance.NextAttemptAtUtc != null || instance.LeaseUntilUtc > DateTime.UtcNow)
                    throw new WorkflowException(409, "CannotRetry", "Only an execution awaiting administrator retry can be retried.");
                instance.Attempts = 0; instance.NextAttemptAtUtc = DateTime.UtcNow;
                break;
            default: throw new WorkflowException(400, "InvalidCommand", "Unknown workflow command.");
        }
        Touch(instance); Audit(instance, actor.UserId, command, request.Comment);
        await NotifyAsync(instance, command, ct);
        await db.SaveChangesAsync(ct);
    }

    public async Task NotifyAsync(Instance instance, string kind, CancellationToken ct)
    {
        var userIds = await db.Tasks.Where(x => x.InstanceId == instance.Id).Select(x => x.UserId).Distinct().ToListAsync(ct);
        userIds.AddRange(db.Tasks.Local.Where(x => x.InstanceId == instance.Id).Select(x => x.UserId));
        userIds.Add(instance.SubmitterId);
        if (instance.Status is "ConfigurationBlocked" || instance.FailureCode != null)
        {
            try
            {
                var admins = await http.PostAsync<List<int>>("auth", "/internal/workflow/admins", new { instance.ClientId }, ct);
                userIds.AddRange(admins);
            }
            catch (WorkflowException ex) when (ex.Status >= 500) { /* Admin queue still exposes blocked instances; retry worker repairs directory outages. */ }
        }
        // Whoever this change just gave a pending task is asked to act; everyone else hears what happened.
        var assigned = db.Tasks.Local.Where(x => x.InstanceId == instance.Id && x.Status == "Pending" && db.Entry(x).State == EntityState.Added).Select(x => x.UserId).ToHashSet();
        var requested = instance.Status == "Pending" && Steps(instance)[instance.StepIndex].DecisionKind == "Recommendation" ? "RecommendationRequested" : "ApprovalRequested";
        foreach (var userId in userIds.Distinct())
        {
            db.Notifications.Add(WorkflowAudit.Added(new Notification { InstanceId = instance.Id, ClientId = instance.ClientId, UserId = userId, Kind = assigned.Contains(userId) ? requested : kind }, userId));
            db.Outbox.Add(WorkflowAudit.Added(new OutboxMessage { InstanceId = instance.Id, ClientId = instance.ClientId, UserId = userId }, userId));
        }
    }
    public void Audit(Instance instance, int userId, string kind, string? comment = null) => db.Events.Add(WorkflowAudit.Added(new AuditEvent { InstanceId = instance.Id, ClientId = instance.ClientId, UserId = userId, Kind = kind, Comment = comment?.Trim() ?? "" }, userId));
    public static void CheckVersion(Instance instance, int expected) { if (expected != instance.Version) throw new WorkflowException(409, "VersionConflict", "This request changed. Refresh before acting again."); }
    public static void Touch(Instance instance) { instance.Version++; instance.UpdatedAtUtc = DateTime.UtcNow; WorkflowAudit.Edited(instance); }
    public static void Close(Instance instance, string status, IEnumerable<UserTask> tasks)
    {
        instance.Status = status; instance.OpenKey = null; instance.NextAttemptAtUtc = null;
        foreach (var task in tasks.Where(x => x.Status == "Pending")) { task.Status = "Canceled"; WorkflowAudit.Edited(task); }
    }
    public static void ValidateComment(string? comment, bool required)
    {
        if (required && string.IsNullOrWhiteSpace(comment)) throw new WorkflowException(400, "CommentRequired", "Please provide an explanation.");
        if (comment?.Length > 4000) throw new WorkflowException(400, "CommentTooLong", "Comments must be at most 4000 characters.");
    }
}

/// <summary>A step's resolved assignees. RelatedUserMissing: a related user was configured but did not resolve.</summary>
public sealed record ResolvedAssignees(List<ApproverUser> Users, bool RelatedUserMissing = false);

/// <summary>
/// The sentence a notification shows. It is built from request metadata only — requester, screen, operation
/// and subject — never from the encrypted payload, so notifications stay free of submitted values.
/// </summary>
public static class WorkflowNotices
{
    public static string Message(string kind, string screenName, string operation, string subject, string submitterName, bool toSubmitter)
    {
        var request = operation switch { "Delete" => $"{screenName} deletion", "Update" => $"{screenName} change", _ => screenName };
        var described = string.IsNullOrWhiteSpace(subject) ? request : $"{request} ({subject})";
        var article = request.Length > 0 && "AEIOUaeiou".Contains(request[0]) ? "an" : "a";
        return kind switch
        {
            "ApprovalRequested" => $"{submitterName} has submitted {article} {described} for your approval.",
            "RecommendationRequested" => $"{submitterName} has submitted {article} {described} for your recommendation.",
            "Submitted" when toSubmitter => $"Your {described} was submitted for approval.",
            "Submitted" => $"{submitterName} has submitted {article} {described}.",
            _ => $"{(toSubmitter ? "Your" : $"{submitterName}'s")} {described}: {Outcome(kind)}."
        };
    }

    private static string Outcome(string kind) => kind switch
    {
        "Approve" => "approved", "Reject" => "rejected", "Recommend" => "recommended", "Decline" => "declined",
        "Return" => "returned for changes", "Escalated" => "rejected at one step and passed to the next approver", "Edited" => "changed by another user",
        "withdraw" => "withdrawn", "cancel" => "canceled by an administrator",
        "resolve" => "reassigned by an administrator", "retry" => "execution retried", "Comment" => "new comment",
        "Completed" => "completed", "ExecutionAttemptFailed" => "execution needs attention",
        "DirectoryRecovered" => "assigned after the directory recovered", _ => kind
    };
}

/// <summary>
/// The lines a change to a pending request adds to the history, one per changed value of the request's preview and
/// labelled from its field name. A value with a display name beside it ("loanTypeId" and "loanType") is
/// shown by that name rather than by its identifier.
/// </summary>
public static class WorkflowChanges
{
    public static List<string> Describe(JsonElement before, JsonElement after)
    {
        var old = Fields(before); var now = Fields(after);
        var lines = new List<string>();
        // A field only one side has is a change in the preview's shape (an older submission), not in a value.
        foreach (var key in now.Keys.Where(old.ContainsKey))
        {
            // A display name is reported where its identifier sits, so the lines follow the form's order.
            var id = IdOf(key, old, now);
            if (id != null && NameOf(id, old, now) == key) continue;
            var shown = NameOf(key, old, now) ?? key;
            // Both are compared: a renamed choice and a different choice of the same name each count.
            if (JsonElement.DeepEquals(old[key], now[key]) && JsonElement.DeepEquals(old[shown], now[shown])) continue;
            var label = Label(shown);
            lines.Add(old[shown].ValueKind is JsonValueKind.Object or JsonValueKind.Array || now[shown].ValueKind is JsonValueKind.Object or JsonValueKind.Array
                ? $"{label} changed" : $"{label}: {Format(old[shown])} → {Format(now[shown])}");
        }
        return lines;
    }

    private static Dictionary<string, JsonElement> Fields(JsonElement value) => value.ValueKind == JsonValueKind.Object
        ? value.EnumerateObject().ToDictionary(x => x.Name, x => x.Value) : [];
    // "loanTypeId" is described by "loanType" (or "loanTypeName") when both sides carry it.
    private static string? NameOf(string key, Dictionary<string, JsonElement> old, Dictionary<string, JsonElement> now)
    {
        if (key.Length <= 2 || !key.EndsWith("Id", StringComparison.Ordinal)) return null;
        var stem = key[..^2];
        return new[] { stem, stem + "Name" }.FirstOrDefault(name => old.ContainsKey(name) && now.ContainsKey(name));
    }
    private static string? IdOf(string key, Dictionary<string, JsonElement> old, Dictionary<string, JsonElement> now)
    {
        var id = (key.EndsWith("Name", StringComparison.Ordinal) ? key[..^4] : key) + "Id";
        return id != key && old.ContainsKey(id) && now.ContainsKey(id) ? id : null;
    }
    private static string Format(JsonElement value)
    {
        switch (value.ValueKind)
        {
            case JsonValueKind.True: return "Yes";
            case JsonValueKind.False: return "No";
            case JsonValueKind.Number: return value.TryGetDecimal(out var number) ? number.ToString("#,0.##", System.Globalization.CultureInfo.InvariantCulture) : value.GetRawText();
            case JsonValueKind.String:
                var text = value.GetString() ?? "";
                if (text.Length == 0) return "—";
                if (text.Length >= 19 && text[4] == '-' && text[10] == 'T' && DateTime.TryParse(text, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.RoundtripKind, out var date))
                    return date.ToString(date.TimeOfDay == TimeSpan.Zero ? "dd-MMM-yyyy" : "dd-MMM-yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture);
                return text.Length > 200 ? text[..200] + "…" : text;
            default: return "—";
        }
    }
    // "appliedAmount" reads "Applied amount".
    private static string Label(string key)
    {
        var words = System.Text.RegularExpressions.Regex.Replace(key, "(?<=[a-z0-9])(?=[A-Z])", " ").ToLowerInvariant();
        return words.Length == 0 ? key : char.ToUpperInvariant(words[0]) + words[1..];
    }
}
