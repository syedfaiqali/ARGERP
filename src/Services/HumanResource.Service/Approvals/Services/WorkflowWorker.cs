using EDXS.Shared.Workflow;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using HumanResource.Service.Approvals.Data;

namespace HumanResource.Service.Approvals.Services;

[Authorize]
public sealed class WorkflowHub(WorkflowEngine engine) : Hub
{
    public override async Task OnConnectedAsync()
    {
        var actor = WorkflowActor.From(Context.GetHttpContext()!) with { EntityId = null };
        await engine.IdentityAsync(actor, Context.ConnectionAborted);
        await Groups.AddToGroupAsync(Context.ConnectionId, Group(actor.ClientId, actor.UserId));
        await base.OnConnectedAsync();
    }
    public static string Group(int clientId, int userId) => $"workflow:{clientId}:{userId}";
}

public sealed class WorkflowWorker(IServiceScopeFactory scopes, ILogger<WorkflowWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ExecuteOne(stoppingToken);
                await DeliverNotifications(stoppingToken);
                await RepairDirectoryBlocks(stoppingToken);
            }
            catch (DbUpdateConcurrencyException) { /* Another replica won the lease. */ }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { break; }
            catch (Exception ex) { logger.LogError("Workflow worker iteration failed ({ExceptionType}).", ex.GetType().Name); }
            await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
        }
    }

    private async Task ExecuteOne(CancellationToken ct)
    {
        using var scope = scopes.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<WorkflowDbContext>();
        var engine = scope.ServiceProvider.GetRequiredService<WorkflowEngine>();
        var client = scope.ServiceProvider.GetRequiredService<HrApprovalClient>();
        var now = DateTime.UtcNow;
        var instance = await db.Instances.Where(x => (x.Status == "PendingExecution" || x.Status == "Executing") &&
            x.NextAttemptAtUtc != null && x.NextAttemptAtUtc <= now && (x.LeaseUntilUtc == null || x.LeaseUntilUtc <= now)).OrderBy(x => x.NextAttemptAtUtc).FirstOrDefaultAsync(ct);
        if (instance == null) return;
        // Saving the aggregate version is the atomic boundary competing with withdrawal.
        instance.Status = "Executing"; instance.ExecutionToken = Guid.NewGuid(); instance.LeaseUntilUtc = now.AddMinutes(2);
        instance.Attempts++; WorkflowEngine.Touch(instance);
        await db.SaveChangesAsync(ct);
        var token = instance.ExecutionToken;
        WorkflowTelemetry.ExecutionAttempts.Add(1, new KeyValuePair<string, object?>("screen", instance.ScreenKey));
        var started = System.Diagnostics.Stopwatch.GetTimestamp();
        try
        {
            var action = engine.Payload(instance);
            var registration = await engine.RegistrationAsync(instance.ScreenKey, ct);
            if (WorkflowJson.Hash(action) != instance.PayloadHash) throw new WorkflowException(409, "PayloadChanged", "The approved payload failed its integrity check.");
            var result = await client.PostAsync<ExecutionResult>(registration.Service, "/internal/workflow/execute", new ExecuteAction(instance.Id, action, instance.PayloadHash), ct);
            await db.Entry(instance).ReloadAsync(ct);
            if (instance.ExecutionToken != token) return;
            instance.RecordKey = result.RecordKey ?? instance.RecordKey;
            WorkflowEngine.Close(instance, "Completed", []); instance.FailureCode = null; instance.LeaseUntilUtc = null;
            WorkflowEngine.Touch(instance); engine.Audit(instance, 0, "Completed");
            await engine.NotifyAsync(instance, "Completed", ct);
            await db.SaveChangesAsync(ct);
        }
        catch (Exception ex) when (ex is not OperationCanceledException || !ct.IsCancellationRequested)
        {
            db.ChangeTracker.Clear(); instance = await db.Instances.SingleAsync(x => x.Id == instance.Id, ct);
            if (instance.ExecutionToken != token) return;
            instance.LeaseUntilUtc = null;
            instance.FailureCode = ex is WorkflowException wf ? wf.Code : "ExecutionOutcomeUnknown";
            if (ex is WorkflowException { Code: "RecordChanged" or "SchemaChanged" or "BusinessValidationFailed" or "ScreenActionDenied" or "AccessRevoked" or "CampusAccessDenied" or "PayloadUnavailable" or "PayloadInvalid" or "PayloadChanged" })
                WorkflowEngine.Close(instance, "NeedsResubmission", []);
            else
            {
                // A timeout may mean the domain transaction committed. Keep Executing and
                // retain OpenKey until its durable receipt resolves the outcome. Never cancel.
                instance.NextAttemptAtUtc = instance.Attempts < 5 ? DateTime.UtcNow.AddSeconds(Math.Pow(2, instance.Attempts) * 5) : null;
            }
            WorkflowEngine.Touch(instance); engine.Audit(instance, 0, "ExecutionAttemptFailed", instance.FailureCode);
            await engine.NotifyAsync(instance, "ExecutionAttemptFailed", ct);
            await db.SaveChangesAsync(ct);
            logger.LogWarning("Workflow execution {InstanceId} requires retry or resubmission ({FailureCode}).", instance.Id, instance.FailureCode);
        }
        finally { WorkflowTelemetry.CallbackDuration.Record(System.Diagnostics.Stopwatch.GetElapsedTime(started).TotalMilliseconds); }
    }

    private async Task DeliverNotifications(CancellationToken ct)
    {
        using var scope = scopes.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<WorkflowDbContext>();
        var hub = scope.ServiceProvider.GetRequiredService<IHubContext<WorkflowHub>>();
        var engine = scope.ServiceProvider.GetRequiredService<WorkflowEngine>();
        var messages = await db.Outbox.Where(x => x.DeliveredAtUtc == null).OrderBy(x => x.Id).Take(100).ToListAsync(ct);
        foreach (var group in messages.GroupBy(x => new { x.ClientId, x.UserId }))
        {
            try
            {
                // Re-check every delivery, so a long-lived socket never receives new events
                // after account revocation. Payloads are fetched separately with fresh auth.
                await engine.IdentityAsync(new(group.Key.UserId, group.Key.ClientId, null), ct);
                await hub.Clients.Group(WorkflowHub.Group(group.Key.ClientId, group.Key.UserId)).SendAsync("workflowChanged",
                    new { eventId = group.Max(x => x.Id), instanceIds = group.Select(x => x.InstanceId).Distinct() }, ct);
            }
            catch (WorkflowException ex) when (ex.Status is 401 or 403) { /* Revoked recipients are intentionally not notified. */ }
            foreach (var message in group) { message.DeliveredAtUtc = DateTime.UtcNow; WorkflowAudit.Edited(message); }
        }
        await db.SaveChangesAsync(ct);
    }

    private async Task RepairDirectoryBlocks(CancellationToken ct)
    {
        using var scope = scopes.CreateScope(); var db = scope.ServiceProvider.GetRequiredService<WorkflowDbContext>();
        var engine = scope.ServiceProvider.GetRequiredService<WorkflowEngine>(); var before = DateTime.UtcNow.AddMinutes(-1);
        var instance = await db.Instances.FirstOrDefaultAsync(x => x.Status == "ConfigurationBlocked" && x.FailureCode == "DirectoryUnavailable" && x.UpdatedAtUtc < before, ct);
        if (instance == null) return;
        await engine.ActivateAsync(instance, ct); WorkflowEngine.Touch(instance);
        if (instance.FailureCode != "DirectoryUnavailable") { engine.Audit(instance, 0, "DirectoryRecovered"); await engine.NotifyAsync(instance, "DirectoryRecovered", ct); }
        await db.SaveChangesAsync(ct);
    }
}
