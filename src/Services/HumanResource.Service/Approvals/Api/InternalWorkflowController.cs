using System.Text.Json;
using EDXS.Shared.Workflow;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HumanResource.Service.Approvals.Data;
using HumanResource.Service.Approvals.Services;

namespace HumanResource.Service.Approvals.Api;

[ApiController, AllowAnonymous, Route("internal/workflow")]
public sealed class InternalWorkflowController(WorkflowDbContext db, WorkflowEngine engine, InternalAuthenticator authenticator, HrApprovalClient directory) : ControllerBase
{
    private Task<string> Verify(CancellationToken ct) => authenticator.VerifyAsync(Request, ["organization", "humanresource"], ct);
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
