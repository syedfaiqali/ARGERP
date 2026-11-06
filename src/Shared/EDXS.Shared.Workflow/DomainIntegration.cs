using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EDXS.Shared.Workflow;

[AttributeUsage(AttributeTargets.Class)]
public sealed class WorkflowActionAttribute(string screenKey) : Attribute { public string ScreenKey { get; } = screenKey; }

public interface IWorkflowActionAdapter
{
    ActionRegistration Registration { get; }
    DbContext Database { get; }
    Task<PreparedAction> PrepareAsync(string operation, string? recordKey, JsonElement payload, WorkflowActor actor, CancellationToken ct);
    Task<string?> CurrentHashAsync(PreparedAction action, CancellationToken ct);
    Task<ExecutionResult> ApplyAsync(PreparedAction action, CancellationToken ct);
    // Validates a change to a request awaiting approval. The result must keep the original's requester,
    // campus, record and snapshot; only its subject, payload and preview may change.
    Task<PreparedAction> AmendAsync(PreparedAction original, JsonElement payload, CancellationToken ct) =>
        throw new WorkflowException(409, "NotEditable", "This screen does not support changing a pending request.");
}

public interface IWorkflowSubmissionClient
{
    Task<SubmissionResult> SubmitAsync(SubmitAction action, CancellationToken ct);
}
public sealed class WorkflowSubmissionClient(WorkflowHttpClient client) : IWorkflowSubmissionClient
{
    public Task<SubmissionResult> SubmitAsync(SubmitAction action, CancellationToken ct) => client.PostAsync<SubmissionResult>("workflow", "/internal/workflow/submit", action, ct);
}

public sealed class DomainExecution(WorkflowPayloadStore payloads)
{
    private static DbCommand Command(DbContext db, string sql, params (string, object?)[] parameters)
    {
        var command = db.Database.GetDbConnection().CreateCommand();
        command.CommandText = sql;
        command.Transaction = db.Database.CurrentTransaction?.GetDbTransaction();
        foreach (var (name, value) in parameters)
        {
            var parameter = command.CreateParameter(); parameter.ParameterName = name; parameter.Value = value ?? DBNull.Value;
            command.Parameters.Add(parameter);
        }
        return command;
    }

    public async Task<ExecutionResult?> FindAsync(IWorkflowActionAdapter adapter, Guid id, string hash, CancellationToken ct)
    {
        var db = adapter.Database;
        await db.Database.OpenConnectionAsync(ct);
        await using var command = Command(db, "SELECT RequestHash, ResultEnvelope FROM dbo.WF_ExecutionReceipts WHERE ServiceName=@service AND ExecutionId=@id",
            ("@service", adapter.Registration.Service), ("@id", id));
        await using var reader = await command.ExecuteReaderAsync(ct);
        if (!await reader.ReadAsync(ct)) return null;
        if (reader.GetString(0) != hash) throw new WorkflowException(409, "IdempotencyConflict", "The request identifier was already used for different data.");
        return payloads.Read<ExecutionResult>(reader.GetString(1), $"receipt:{adapter.Registration.Service}:{id}");
    }

    public async Task<ExecutionResult> ExecuteAsync(IWorkflowActionAdapter adapter, Guid id, string hash,
        Func<Task<PreparedAction>> prepare, CancellationToken ct)
    {
        var db = adapter.Database;
        return await db.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
        {
            db.ChangeTracker.Clear();
            await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
            // The existing pilot handlers allocate MAX(Id)+1. This short transaction lock
            // serializes every workflow-enabled writer of this resource, including immediate saves.
            await using (var command = Command(db,
                "DECLARE @r int; EXEC @r=sp_getapplock @Resource=@resource, @LockMode='Exclusive', @LockOwner='Transaction', @LockTimeout=15000; IF @r < 0 THROW 51000, 'Workflow write lock unavailable', 1;",
                ("@resource", $"workflow:{adapter.Registration.ResourceType}"))) await command.ExecuteNonQueryAsync(ct);
            var prior = await FindAsync(adapter, id, hash, ct);
            if (prior != null) { await tx.CommitAsync(ct); return prior; }
            var action = await prepare();
            if (action.Operation != "Create" && await adapter.CurrentHashAsync(action, ct) != action.ExpectedHash)
                throw new WorkflowException(409, "RecordChanged", "The record changed after submission. Submit a new revision for approval.");
            var result = await adapter.ApplyAsync(action, ct);
            if (result.StatusCode >= 400)
            {
                // Expected validation results must not become durable success
                // receipts: the caller can retry after the references change.
                await tx.RollbackAsync(ct);
                return result;
            }
            var storedResult = payloads.Write(result);
            await using (var command = Command(db,
                "INSERT INTO dbo.WF_ExecutionReceipts (ServiceName,ExecutionId,RequestHash,ResultEnvelope,CompletedAtUtc) VALUES (@service,@id,@hash,@result,SYSUTCDATETIME())",
                ("@service", adapter.Registration.Service), ("@id", id), ("@hash", hash), ("@result", storedResult))) await command.ExecuteNonQueryAsync(ct);
            await tx.CommitAsync(ct);
            return result;
        });
    }
}

public sealed class WorkflowActionFilter(IConfiguration config, WorkflowHttpClient client,
    IWorkflowSubmissionClient submission, IEnumerable<IWorkflowActionAdapter> adapters, DomainExecution execution) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var descriptor = context.ActionDescriptor as ControllerActionDescriptor;
        var marker = descriptor?.ControllerTypeInfo.GetCustomAttribute<WorkflowActionAttribute>();
        var method = context.HttpContext.Request.Method;
        if (marker == null || method is not ("POST" or "PUT" or "DELETE") || !config.GetValue<bool>("Workflow:Enabled")) { await next(); return; }
        var adapter = adapters.Single(x => x.Registration.ScreenKey == marker.ScreenKey);
        var http = context.HttpContext; var ct = http.RequestAborted;
        var actor = WorkflowActor.From(http);
        var operation = method == "POST" ? "Create" : method == "PUT" ? "Update" : "Delete";
        await client.PostAsync<WorkflowIdentity>("auth", "/internal/workflow/identity", new IdentityRequest(actor, marker.ScreenKey, operation), ct);
        if (!Guid.TryParse(http.Request.Headers["Idempotency-Key"], out var key) || key == Guid.Empty)
            throw new WorkflowException(400, "IdempotencyKeyRequired", "A unique Idempotency-Key header is required.");
        var recordKey = context.ActionArguments.TryGetValue("id", out var rawId) ? Convert.ToString(rawId) : null;
        var body = context.ActionArguments.TryGetValue("request", out var payload) ? WorkflowJson.Element(payload) : WorkflowJson.Element(new { });
        var hash = WorkflowJson.Hash(new { actor.UserId, actor.ClientId, EntityId = adapter.Registration.EntityScoped ? actor.EntityId : null, marker.ScreenKey, operation, recordKey, body });
        var receipt = await execution.FindAsync(adapter, key, hash, ct);
        if (receipt != null) { context.Result = Result(receipt); return; }
        var existing = await client.PostAsync<SubmissionResult>("workflow", "/internal/workflow/lookup",
            new { actor, idempotencyKey = key, requestHash = hash }, ct);
        if (existing.Pending) { context.Result = new ObjectResult(existing) { StatusCode = 202 }; return; }
        PreparedAction action;
        try { action = await adapter.PrepareAsync(operation, recordKey, body, actor, ct); }
        catch (InvalidOperationException ex) { throw new WorkflowException(400, "BusinessValidationFailed", ex.Message); }
        Guid? previous = Guid.TryParse(http.Request.Headers["X-Workflow-Previous-Instance"], out var previousId) ? previousId : null;
        var submitted = await submission.SubmitAsync(new(action, key, hash, previous), ct);
        if (submitted.Pending) { context.Result = new ObjectResult(submitted) { StatusCode = 202 }; return; }
        context.Result = Result(await execution.ExecuteAsync(adapter, key, hash, () => Task.FromResult(action), ct));
    }
    private static IActionResult Result(ExecutionResult result) => result.Body.HasValue
        ? new ObjectResult(result.Body.Value) { StatusCode = result.StatusCode } : new StatusCodeResult(result.StatusCode);
}

public static class WorkflowIntegration
{
    public static IServiceCollection AddWorkflowTransport(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddSingleton<InternalAuthenticator>();
        services.AddSingleton<WorkflowNonceStore>();
        services.AddSingleton<WorkflowPayloadStore>();
        services.AddHttpClient<WorkflowHttpClient>();
        return services;
    }
    public static IServiceCollection AddWorkflowAdapter<T>(this IServiceCollection services) where T : class, IWorkflowActionAdapter
    {
        services.AddWorkflowTransport();
        services.AddScoped<IWorkflowActionAdapter, T>();
        services.AddScoped<DomainExecution>();
        services.AddScoped<IWorkflowSubmissionClient, WorkflowSubmissionClient>();
        services.Configure<MvcOptions>(options => options.Filters.Add<WorkflowActionFilter>());
        services.AddHostedService<WorkflowRegistrationWorker>();
        return services;
    }
    public static IApplicationBuilder UseWorkflowErrors(this IApplicationBuilder app) => app.Use(async (context, next) =>
    {
        try { await next(context); }
        catch (WorkflowException ex)
        {
            context.Response.StatusCode = ex.Status;
            await context.Response.WriteAsJsonAsync(new { error = ex.Message, code = ex.Code, instanceId = ex.InstanceId });
        }
        catch (DbUpdateConcurrencyException)
        {
            context.Response.StatusCode = 409;
            await context.Response.WriteAsJsonAsync(new { error = "This item changed. Refresh before acting again.", code = "VersionConflict" });
        }
    });

    public static void MapWorkflowAdapter(this WebApplication app)
    {
        app.MapPost("/internal/workflow/execute", async (HttpContext http, InternalAuthenticator auth,
            IEnumerable<IWorkflowActionAdapter> adapters, DomainExecution execution, WorkflowHttpClient client) =>
        {
            await auth.VerifyAsync(http.Request, ["workflow"], http.RequestAborted);
            var request = await http.Request.ReadFromJsonAsync<ExecuteAction>(WorkflowJson.Options, http.RequestAborted)
                ?? throw new WorkflowException(400, "InvalidRequest", "An execution request is required.");
            var adapter = adapters.SingleOrDefault(x => x.Registration.ScreenKey == request.Action.ScreenKey)
                ?? throw new WorkflowException(400, "UnknownAction", "Unregistered workflow action.");
            // Receipt lookup precedes current authorization: recover a committed response after
            // a timeout even if the requester was disabled after the business transaction committed.
            var prior = await execution.FindAsync(adapter, request.ExecutionId, request.RequestHash, http.RequestAborted);
            if (prior != null) return ExecutionResponse(prior);
            try
            {
                return ExecutionResponse(await execution.ExecuteAsync(adapter, request.ExecutionId, request.RequestHash,
                    async () =>
                    {
                        // Check AFTER the locked receipt lookup so a concurrent committed retry
                        // cannot be misclassified as permission/schema failure.
                        if (request.Action.SchemaVersion != adapter.Registration.SchemaVersion)
                            throw new WorkflowException(409, "SchemaChanged", "The form changed. Submit a new revision for approval.");
                        await client.PostAsync<WorkflowIdentity>("auth", "/internal/workflow/identity",
                            new IdentityRequest(new(request.Action.UserId, request.Action.ClientId, request.Action.EntityId), request.Action.ScreenKey, request.Action.Operation), http.RequestAborted);
                        return request.Action;
                    }, http.RequestAborted));
            }
            catch (WorkflowException) { throw; }
            catch (InvalidOperationException) { throw new WorkflowException(422, "BusinessValidationFailed", "The proposed action no longer passes business validation. Submit a new revision."); }
            catch (KeyNotFoundException) { throw new WorkflowException(409, "RecordChanged", "The record is no longer available. Submit a new revision."); }
            catch (DbUpdateException ex) when (ex.InnerException is Microsoft.Data.SqlClient.SqlException sql && sql.Number is 547 or 2601 or 2627 or 8152 or 2628)
            { throw new WorkflowException(422, "BusinessValidationFailed", "The proposed action conflicts with current data. Submit a new revision."); }
        }).AllowAnonymous(); // InternalAuthenticator is mandatory; JWT headers cannot authorize execution.

        // The workflow service has already checked that the user may edit the request's screen.
        app.MapPost("/internal/workflow/amend", async (HttpContext http, InternalAuthenticator auth, IEnumerable<IWorkflowActionAdapter> adapters) =>
        {
            await auth.VerifyAsync(http.Request, ["workflow"], http.RequestAborted);
            var request = await http.Request.ReadFromJsonAsync<AmendAction>(WorkflowJson.Options, http.RequestAborted)
                ?? throw new WorkflowException(400, "InvalidRequest", "The changed values are required.");
            var adapter = adapters.SingleOrDefault(x => x.Registration.ScreenKey == request.Original.ScreenKey)
                ?? throw new WorkflowException(400, "UnknownAction", "Unregistered workflow action.");
            try { return Results.Json(await adapter.AmendAsync(request.Original, request.Payload, http.RequestAborted), WorkflowJson.Options); }
            catch (InvalidOperationException ex) { throw new WorkflowException(400, "BusinessValidationFailed", ex.Message); }
        }).AllowAnonymous();
    }

    private static IResult ExecutionResponse(ExecutionResult result)
    {
        if (result.StatusCode < 400) return Results.Json(result);
        var message = result.Body is { } body && body.TryGetProperty("error", out var error)
            ? error.GetString() : "The proposed action no longer passes business validation.";
        return Results.Json(new { error = message, code = result.StatusCode == 404 ? "RecordChanged" : "BusinessValidationFailed" },
            statusCode: result.StatusCode == 404 ? 409 : 422);
    }
}

public sealed class WorkflowRegistrationWorker(IServiceScopeFactory scopes, IConfiguration config, ILogger<WorkflowRegistrationWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!config.GetValue<bool>("Workflow:Enabled")) return;
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopes.CreateScope();
                var client = scope.ServiceProvider.GetRequiredService<WorkflowHttpClient>();
                foreach (var adapter in scope.ServiceProvider.GetServices<IWorkflowActionAdapter>())
                    await client.PostAsync<object>("workflow", "/internal/workflow/register", adapter.Registration, stoppingToken);
            }
            catch (Exception ex) when (!stoppingToken.IsCancellationRequested) { logger.LogWarning("Workflow registration unavailable ({ExceptionType}); retrying.", ex.GetType().Name); }
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}
