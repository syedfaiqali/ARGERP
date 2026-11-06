using EDXS.Shared.Auth;
using EDXS.Shared.Workflow;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Workflow.Service.Data;
using Workflow.Service.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true).AddEnvironmentVariables();
builder.Configuration["Workflow:ServiceName"] = "workflow";
builder.Services.AddControllers();
builder.Services.AddWorkflowTransport();
builder.Services.AddScoped<WorkflowEngine>();
builder.Services.AddDbContext<WorkflowDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("OrganizationDB") ?? throw new InvalidOperationException("ConnectionStrings:OrganizationDB is required."),
    sql =>
    {
        sql.CommandTimeout(30);
        // SQL Server 2014 has no OPENJSON, so collection Contains() must be sent as constants.
        sql.TranslateParameterizedCollectionsToConstants();
    }));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    // Same configuration shape as the other services (JwtSettings:SecretKey/Issuer/Audience),
    // so one set of JWT settings covers every service in the solution.
    var jwtSettings = builder.Configuration.GetSection("JwtSettings");
    var jwt = new JwtValidationOptions
    {
        SigningKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JwtSettings:SecretKey is not configured."),
        Issuer = jwtSettings["Issuer"] ?? "EDXS.Auth.Service",
        Audience = jwtSettings["Audience"] ?? "EDXS.Services"
    };
    options.TokenValidationParameters = JwtValidationParametersFactory.Create(jwt);
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (context.Request.Path.StartsWithSegments("/api/Workflow/v3/hub")) context.Token = context.Request.Query["access_token"];
            return Task.CompletedTask;
        }
    };
});
builder.Services.AddAuthorization();
var signalR = builder.Services.AddSignalR(options => options.MaximumReceiveMessageSize = 16 * 1024);
var redis = builder.Configuration.GetConnectionString("redis");
if (!string.IsNullOrWhiteSpace(redis)) signalR.AddStackExchangeRedis(redis, options => options.Configuration.ChannelPrefix = StackExchange.Redis.RedisChannel.Literal("EDXS.Workflow"));
else if (!builder.Environment.IsDevelopment() && !builder.Configuration.GetValue<bool>("Workflow:SingleReplica"))
    throw new InvalidOperationException("SignalR Redis backplane is required unless Workflow:SingleReplica is explicitly enabled.");
builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy.WithOrigins(
    builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? ["http://localhost:3000"]).AllowAnyHeader().AllowAnyMethod().AllowCredentials()));
builder.Services.AddHostedService<WorkflowWorker>();
builder.Services.AddHealthChecks().AddCheck<WorkflowHealthCheck>("workflow-database");
var app = builder.Build();
app.UseWorkflowErrors();
app.Use(async (context, next) =>
{
    try { await next(); }
    catch (DbUpdateException)
    {
        context.Response.StatusCode = 409;
        await context.Response.WriteAsJsonAsync(new { code = "Conflict", error = "The workflow changed or this item already exists. Refresh and retry." });
    }
});
app.UseCors(); app.UseAuthentication(); app.UseAuthorization();
app.MapControllers();
app.MapHub<WorkflowHub>("/api/Workflow/v3/hub", options => options.CloseOnAuthenticationExpiration = true);
app.MapHealthChecks("/health");
app.MapGet("/health/live", () => Results.Ok(new { status = "alive" })).AllowAnonymous();
// Migrations are applied explicitly by the release job, never against an implicit default DB.
app.Run();

public partial class Program { }
public sealed class WorkflowHealthCheck(IServiceScopeFactory scopes) : Microsoft.Extensions.Diagnostics.HealthChecks.IHealthCheck
{
    public async Task<Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult> CheckHealthAsync(Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckContext context, CancellationToken ct = default)
    {
        using var scope = scopes.CreateScope(); var db = scope.ServiceProvider.GetRequiredService<WorkflowDbContext>();
        return await db.Database.CanConnectAsync(ct) && !(await db.Database.GetPendingMigrationsAsync(ct)).Any()
            ? Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy()
            : Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Unhealthy("Workflow database is unavailable or needs migration.");
    }
}
