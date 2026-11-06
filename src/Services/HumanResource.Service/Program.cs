using ARGERP.Runtime;
using EDXS.Shared.Common;
using HumanResource.Service.PayrollAccounting;
using HumanResource.Service.PayrollAccounting.Commands;
using HumanResource.Service.PayrollAccounting.Contracts;
using HumanResource.Service.Infrastructure.Data;
using HumanResource.Service.Posting;
using HumanResource.Service.Services;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);
builder.AddArgAuthentication();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<HumanResourceDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("OrganizationDB"), sql => sql.TranslateParameterizedCollectionsToConstants()));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly, typeof(PostJvCommand).Assembly));
builder.Services.AddScoped<DbContext>(sp => sp.GetRequiredService<HumanResourceDbContext>());
builder.Services.AddJvPosting();
builder.Services.AddScoped<PayrollJvBuilder>();
builder.Services.AddScoped<IJvBacklogProvider, PayrollJvBacklogProvider>();
builder.Services.Configure<ZkAttendanceSyncOptions>(builder.Configuration.GetSection("ZkAttendance"));
builder.Services.AddScoped<IZkAttendanceSyncService, ZkAttendanceSyncService>();
builder.Services.AddHostedService<HumanResource.Service.Workers.ZkAttendanceSyncWorker>();

builder.Services.AddScoped<IVacancyFulfillmentService, VacancyFulfillmentService>();

builder.Services.AddScoped<Auth.Service.Services.HrAttritionDashboardService>();
builder.Services.AddHttpClient("googleoauth", client => client.Timeout = TimeSpan.FromSeconds(30));
builder.Services.AddScoped<GoogleOAuthService>();
builder.Services.AddHttpClient<AssistantChatService>(client => client.Timeout = TimeSpan.FromSeconds(50));
var app = builder.Build();
app.UseArgRuntime();
app.UseScopeHeaderNormalization();
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.Run();
