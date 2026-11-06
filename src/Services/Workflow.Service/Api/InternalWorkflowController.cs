using System.Text.Json;
using EDXS.Shared.Workflow;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Workflow.Service.Data;
using Workflow.Service.Services;

namespace Workflow.Service.Api;

[ApiController, AllowAnonymous, Route("internal/workflow")]
public sealed class InternalWorkflowController(WorkflowDbContext db, WorkflowEngine engine, InternalAuthenticator authenticator, WorkflowHttpClient directory) : ControllerBase
{
    private Task<string> Verify(CancellationToken ct) => authenticator.VerifyAsync(Request, ["academics", "organization", "transport", "humanresource"], ct);
    public sealed record LoanReviewersRequest(int ClientId, int? EntityId, string RecordKey);
    [HttpPost("loan-reviewers")]
    public async Task<List<WorkflowUser>> LoanReviewers(CancellationToken ct)
    {
        // Only the owning HR service can request names for its linked loan history.
        await authenticator.VerifyAsync(Request, ["humanresource"], ct);
        var request = await Request.ReadFromJsonAsync<LoanReviewersRequest>(WorkflowJson.Options, ct)
            ?? throw new WorkflowException(400, "InvalidRequest", "Loan record is required.");
        if (request.ClientId <= 0 || !int.TryParse(request.RecordKey, out var recordId) || recordId <= 0)
            throw new WorkflowException(400, "InvalidRequest", "Loan record and tenant are required.");
        var ids = db.Instances.Where(x => x.ClientId == request.ClientId &&
            (x.EntityId == null || x.EntityId == request.EntityId) && x.ScreenKey == "EPLoanrequest" &&
            x.Operation != "Delete" && x.Status == "Completed" && x.RecordKey == request.RecordKey).Select(x => x.Id);
        var users = await db.Events.AsNoTracking().Where(x => x.ClientId == request.ClientId && x.InstanceId.HasValue &&
            ids.Contains(x.InstanceId.Value) && x.UserId > 0 &&
            (x.Kind == "Approve" || x.Kind == "Reject" || x.Kind == "Return" || x.Kind == "Recommend" || x.Kind == "Decline" || x.Kind == "Comment"))
            .Select(x => x.UserId).Distinct().ToArrayAsync(ct);
        return users.Length == 0 ? [] : await directory.PostAsync<List<WorkflowUser>>("auth", "/internal/workflow/users",
            new UserNamesRequest(request.ClientId, users), ct);
    }
    [HttpPost("register")]
    public async Task<object> Register(CancellationToken ct)
    {
        try
        {
            var service = await Verify(ct);
            var action = await Request.ReadFromJsonAsync<ActionRegistration>(WorkflowJson.Options, ct) ?? throw new WorkflowException(400, "InvalidRegistration", "Registration required.");
            var allowed = PilotActions.All.SingleOrDefault(x => x.ScreenKey == action.ScreenKey && x.Service == service);
            // Deployment-reviewed allowlist prevents a service from registering arbitrary callback
            // addresses or impersonating another domain. Additional adapters extend this manifest.
            if (allowed == null || WorkflowJson.Hash(allowed) != WorkflowJson.Hash(action)) throw new WorkflowException(403, "InvalidRegistration", "The adapter does not match the deployment manifest.");
                var row = await db.Actions.FindAsync([action.ScreenKey], ct);
            // Registration is a service-to-service call with no interactive user, so the audit trail records the service itself.
            if (row == null) { row = WorkflowAudit.Added(new ActionCatalog { ScreenKey = action.ScreenKey }); db.Actions.Add(row); }
            else WorkflowAudit.Edited(row);
            row.Service = service; row.RegistrationJson = JsonSerializer.Serialize(action, WorkflowJson.Options); row.RegisteredAtUtc = DateTime.UtcNow;
            await db.SaveChangesAsync(ct); return new { registered = true };
        }
        // The registering service abandoned the request (it stopped, restarted or timed out). Nobody is
        // waiting for this response and its next 30-second registration retries, so there is nothing to report.
        catch (OperationCanceledException) when (ct.IsCancellationRequested) { return new { registered = false }; }
    }
    [HttpPost("submit")]
    public async Task<SubmissionResult> Submit(CancellationToken ct)
    {
        var service = await Verify(ct);
        var request = await Request.ReadFromJsonAsync<SubmitAction>(WorkflowJson.Options, ct) ?? throw new WorkflowException(400, "InvalidRequest", "Submission is required.");
        return await engine.SubmitAsync(request, service, ct);
    }
    public sealed record LookupRequest(WorkflowActor Actor, Guid IdempotencyKey, string RequestHash);
    [HttpPost("lookup")]
    public async Task<SubmissionResult> Lookup(CancellationToken ct)
    {
        await Verify(ct);
        var request = await Request.ReadFromJsonAsync<LookupRequest>(WorkflowJson.Options, ct) ?? throw new WorkflowException(400, "InvalidRequest", "Request is required.");
        return await engine.LookupAsync(request.Actor.ClientId, request.Actor.UserId, request.IdempotencyKey, request.RequestHash, ct);
    }
}
