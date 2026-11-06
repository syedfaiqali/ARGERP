using HumanResource.Service.Approvals.Data;
using HumanResource.Service.Approvals.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.Service.Approvals;

public static class HrApprovalHosting
{
    public static void AddHrApprovals(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<HrApprovalClient>();
        builder.Services.AddScoped<WorkflowEngine>();
        builder.Services.AddDbContext<WorkflowDbContext>(options => options.UseSqlServer(
            builder.Configuration.GetConnectionString("OrganizationDB"), sql => sql.TranslateParameterizedCollectionsToConstants()));
        builder.Services.AddHostedService<WorkflowWorker>();
        var signalR = builder.Services.AddSignalR(options => options.MaximumReceiveMessageSize = 16 * 1024);
        var redis = builder.Configuration.GetConnectionString("redis");
        if (!string.IsNullOrWhiteSpace(redis))
            signalR.AddStackExchangeRedis(redis, options => options.Configuration.ChannelPrefix = StackExchange.Redis.RedisChannel.Literal("ARGERP.HR.Approvals"));
        builder.Services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            options.Events.OnMessageReceived = context =>
            {
                if (context.Request.Path.StartsWithSegments("/api/Workflow/v3/hub"))
                    context.Token = context.Request.Query["access_token"];
                return Task.CompletedTask;
            };
        });
    }

    public static void MapHrApprovals(this WebApplication app) =>
        app.MapHub<WorkflowHub>("/api/Workflow/v3/hub", options => options.CloseOnAuthenticationExpiration = true);
}
