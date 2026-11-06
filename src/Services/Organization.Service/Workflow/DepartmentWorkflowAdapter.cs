using System.Text.Json;
using EDXS.Shared.Workflow;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Organization.Service.Commands;
using Organization.Service.DTOs;
using Organization.Service.Handlers.Commands;
using Organization.Service.Infrastructure.Data;

namespace Organization.Service.Workflow;

public sealed class DepartmentWorkflowAdapter(OrganizationDbContext db, IMediator mediator) : SnapshotWorkflowAdapter
{
    public override ActionRegistration Registration => PilotActions.Department;
    public override DbContext Database => db;
    protected override async Task<JsonElement?> SnapshotAsync(int id, WorkflowActor actor, CancellationToken ct)
    {
        var row = await db.OrgDepartmentLegacy.AsNoTracking().Where(x => x.Id == id && x.clientid == actor.ClientId && x.utblock != true)
            .Select(x => new { x.Id, Code = x.depcode, DepartmentName = x.depstxt, Description = x.depltxt,
                ChartOfAccountId = x.ficoaid, ReceivingChartOfAccountId = x.ficoaid_rec,
                ChartOfAccount = x.Ficoa != null ? x.Ficoa.Title : null, ReceivingChartOfAccount = x.FicoaRec != null ? x.FicoaRec.Title : null }).FirstOrDefaultAsync(ct);
        return row == null ? null : WorkflowJson.Element(row);
    }
    protected override async Task<(JsonElement Payload, JsonElement Preview, string Subject)> NormalizeAsync(JsonElement payload, int? id, WorkflowActor actor, CancellationToken ct)
    {
        var request = Read<CreateDepartmentV3Request>(payload);
        await new DepartmentV3CommandHandlers(db).ValidateProposalAsync(request, actor.ClientId, id, ct);
        var accounts = new[] { request.ChartOfAccountId, request.ReceivingChartOfAccountId }.Where(x => x.HasValue).Select(x => x!.Value).Distinct().ToArray();
        var lookups = await db.FicoaLegacy.Where(x => x.clientid == actor.ClientId && x.Active == true && accounts.Contains(x.Id)).Select(x => new { x.Id, x.Title }).ToListAsync(ct);
        if (accounts.Length != lookups.Count) throw new InvalidOperationException("Choose active chart-of-account records belonging to this tenant.");
        var preview = new { request.Code, request.DepartmentName, request.Description, request.ChartOfAccountId, request.ReceivingChartOfAccountId,
            ChartOfAccount = lookups.FirstOrDefault(x => x.Id == request.ChartOfAccountId)?.Title,
            ReceivingChartOfAccount = lookups.FirstOrDefault(x => x.Id == request.ReceivingChartOfAccountId)?.Title };
        return (WorkflowJson.Element(request), WorkflowJson.Element(preview), $"{request.Code} · {request.DepartmentName}");
    }
    public override async Task<ExecutionResult> ApplyAsync(PreparedAction action, CancellationToken ct)
    {
        if (action.Operation == "Delete")
        {
            var deleted = await mediator.Send(new DeleteDepartmentV3Command(int.Parse(action.RecordKey!), action.ClientId), ct);
            if (!deleted.Found) return new(404, WorkflowJson.Element(new { error = "Department not found" }), action.RecordKey);
            if (!deleted.Deleted) return new(400, WorkflowJson.Element(new { error = deleted.Message("Department"), usedIn = deleted.UsedIn }), action.RecordKey);
            return new(204, null, action.RecordKey);
        }
        await NormalizeAsync(action.Payload, action.RecordKey == null ? null : int.Parse(action.RecordKey), new(action.UserId, action.ClientId, null), ct);
        if (action.Operation == "Create")
        {
            var row = await mediator.Send(new CreateDepartmentV3Command(Read<CreateDepartmentV3Request>(action.Payload), action.ClientId), ct);
            return new(201, WorkflowJson.Element(row), row.Id.ToString());
        }
        var update = Read<UpdateDepartmentV3Request>(action.Payload); update.Id = int.Parse(action.RecordKey!);
        if (!await mediator.Send(new UpdateDepartmentV3Command(update, action.ClientId), ct)) throw new KeyNotFoundException();
        return new(204, null, action.RecordKey);
    }
}
