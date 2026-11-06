using System.Text.Json;
using EDXS.Shared.Workflow;
using HumanResource.Service.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.Service.Workflow;

/// <summary>
/// Shared adapter for Human Resource setup screens whose Save and Delete are plain create,
/// update and delete commands. The screen's create request is the payload for both Create and
/// Update, so "revise and resubmit" posts back exactly the fields the screen itself sends.
/// </summary>
public abstract class HrSetupWorkflowAdapter<TRequest>(HumanResourceDbContext db) : SnapshotWorkflowAdapter
{
    public override DbContext Database => db;

    protected abstract Task<object?> FindAsync(int id, int clientId, int? entityId, CancellationToken ct);
    protected abstract Task<(object Row, int Id)> CreateAsync(TRequest request, int clientId, int? entityId, CancellationToken ct);
    protected abstract Task<bool> UpdateAsync(int id, TRequest request, int clientId, int? entityId, CancellationToken ct);
    protected abstract Task<bool> DeleteAsync(int id, int clientId, int? entityId, CancellationToken ct);
    protected abstract string Subject(TRequest request);

    protected override async Task<JsonElement?> SnapshotAsync(int id, WorkflowActor actor, CancellationToken ct)
    {
        var row = await FindAsync(id, actor.ClientId, actor.EntityId, ct);
        return row == null ? null : WorkflowJson.Element(row);
    }

    protected override Task<(JsonElement Payload, JsonElement Preview, string Subject)> NormalizeAsync(JsonElement payload, int? id, WorkflowActor actor, CancellationToken ct)
    {
        var request = Read<TRequest>(payload);
        var element = WorkflowJson.Element(request);
        return Task.FromResult((element, element, Subject(request)));
    }

    public override async Task<ExecutionResult> ApplyAsync(PreparedAction action, CancellationToken ct)
    {
        var previousAuditUser = db.WorkflowAuditUserId;
        db.WorkflowAuditUserId = action.UserId;
        try
        {
            if (action.Operation == "Delete")
            {
                if (!await DeleteAsync(int.Parse(action.RecordKey!), action.ClientId, action.EntityId, ct)) throw new KeyNotFoundException();
                return new(204, null, action.RecordKey);
            }
            var request = Read<TRequest>(action.Payload);
            if (action.Operation == "Create")
            {
                var (row, id) = await CreateAsync(request, action.ClientId, action.EntityId, ct);
                return new(201, WorkflowJson.Element(row), id.ToString());
            }
            if (!await UpdateAsync(int.Parse(action.RecordKey!), request, action.ClientId, action.EntityId, ct)) throw new KeyNotFoundException();
            return new(204, null, action.RecordKey);
        }
        // With no published workflow this runs inside the user's own request, where the screen's
        // controller used to answer these with 400/404. Keep the business message for the user;
        // under an approved workflow the same codes mark the request for resubmission.
        catch (InvalidOperationException ex) { throw new WorkflowException(400, "BusinessValidationFailed", ex.Message); }
        catch (KeyNotFoundException) { throw new WorkflowException(409, "RecordChanged", "The record is no longer available. Refresh and try again."); }
        finally { db.WorkflowAuditUserId = previousAuditUser; }
    }
}
