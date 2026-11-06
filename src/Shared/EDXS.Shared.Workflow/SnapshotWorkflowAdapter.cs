using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace EDXS.Shared.Workflow;

/// <summary>Small adapter contract; no staging writes or arbitrary controller invocation.</summary>
public abstract class SnapshotWorkflowAdapter : IWorkflowActionAdapter
{
    public abstract ActionRegistration Registration { get; }
    public abstract DbContext Database { get; }
    protected abstract Task<JsonElement?> SnapshotAsync(int id, WorkflowActor actor, CancellationToken ct);
    protected abstract Task<(JsonElement Payload, JsonElement Preview, string Subject)> NormalizeAsync(JsonElement payload, int? id, WorkflowActor actor, CancellationToken ct);
    public abstract Task<ExecutionResult> ApplyAsync(PreparedAction action, CancellationToken ct);
    protected static T Read<T>(JsonElement value) => value.Deserialize<T>(WorkflowJson.Options)
        ?? throw new WorkflowException(400, "InvalidRequest", "Form data is required.");
    public async Task<PreparedAction> PrepareAsync(string operation, string? recordKey, JsonElement payload, WorkflowActor actor, CancellationToken ct)
    {
        if (operation is not ("Create" or "Update" or "Delete")) throw new WorkflowException(400, "UnknownAction", "Unsupported action.");
        actor = actor with { EntityId = Registration.EntityScoped ? actor.EntityId : null };
        if (Registration.EntityScoped && actor.EntityId is null) throw new WorkflowException(400, "CampusRequired", "Select a campus first.");
        int? id = operation == "Create" ? null : int.TryParse(recordKey, out var parsed) && parsed > 0 ? parsed
            : throw new WorkflowException(400, "InvalidRecord", "A valid record ID is required.");
        var before = id.HasValue ? await SnapshotAsync(id.Value, actor, ct)
            ?? throw new WorkflowException(404, "RecordChanged", "The record is no longer available in this scope.") : WorkflowJson.Element(new { });
        var normalized = operation == "Delete" ? (Payload: WorkflowJson.Element(new { id }), Preview: before, Subject: $"{Registration.Name} #{id}")
            : await NormalizeAsync(payload, id, actor, ct);
        return new(Registration.ScreenKey, operation, actor.ClientId, actor.EntityId, actor.UserId, id?.ToString(),
            normalized.Subject, id.HasValue ? WorkflowJson.Hash(before) : null, normalized.Payload, before, normalized.Preview, Registration.SchemaVersion);
    }
    // A change by another user is validated as if the requester had submitted it. The requester, campus, record
    // and the snapshot the request was based on stay as submitted, so only the proposed values change.
    public async Task<PreparedAction> AmendAsync(PreparedAction original, JsonElement payload, CancellationToken ct)
    {
        if (original.Operation == "Delete") throw new WorkflowException(409, "NotEditable", "A deletion request has no values to change.");
        var normalized = await NormalizeAmendmentAsync(payload, original, ct);
        return original with { Subject = normalized.Subject, Payload = normalized.Payload, After = normalized.Preview };
    }
    // Runs inside a service-to-service call: there is no signed-in requester, only the original submission.
    protected virtual Task<(JsonElement Payload, JsonElement Preview, string Subject)> NormalizeAmendmentAsync(JsonElement payload, PreparedAction original, CancellationToken ct) =>
        NormalizeAsync(payload, int.TryParse(original.RecordKey, out var id) ? id : null, new(original.UserId, original.ClientId, original.EntityId), ct);
    public async Task<string?> CurrentHashAsync(PreparedAction action, CancellationToken ct)
    {
        if (!int.TryParse(action.RecordKey, out var id)) return null;
        var current = await SnapshotAsync(id, new(action.UserId, action.ClientId, action.EntityId), ct);
        return current.HasValue ? WorkflowJson.Hash(current.Value) : null;
    }
}
