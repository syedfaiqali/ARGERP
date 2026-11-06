using ARGERP.Runtime;
using EDXS.Shared.Common;
using EDXS.Shared.Finance;
using EDXS.Shared.Finance.Commands;
using EDXS.Shared.Finance.Contracts;
using HumanResource.Service.Infrastructure.Data;
using HumanResource.Service.Posting;
using HumanResource.Service.Services;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);
builder.AddArgAuthentication();
builder.Configuration["Workflow:ServiceName"] = "humanresource";
EDXS.Shared.Workflow.WorkflowIntegration.AddWorkflowAdapter<HumanResource.Service.Workflow.QualificationWorkflowAdapter>(builder.Services);
// Further adapters reuse the action filter and registration worker installed above; calling
// AddWorkflowAdapter again would add the MVC action filter once more for every screen.

builder.Services.AddScoped<EDXS.Shared.Workflow.IWorkflowActionAdapter, HumanResource.Service.Workflow.QualificationTypeWorkflowAdapter>();
builder.Services.AddScoped<EDXS.Shared.Workflow.IWorkflowActionAdapter, HumanResource.Service.Workflow.SkillWorkflowAdapter>();
builder.Services.AddScoped<EDXS.Shared.Workflow.IWorkflowActionAdapter, HumanResource.Service.Workflow.SkillTypeWorkflowAdapter>();
builder.Services.AddScoped<EDXS.Shared.Workflow.IWorkflowActionAdapter, HumanResource.Service.Workflow.PositionTypeWorkflowAdapter>();
builder.Services.AddScoped<EDXS.Shared.Workflow.IWorkflowActionAdapter, HumanResource.Service.Workflow.DocumentTypeWorkflowAdapter>();
builder.Services.AddScoped<EDXS.Shared.Workflow.IWorkflowActionAdapter, HumanResource.Service.Workflow.EarningTypeWorkflowAdapter>();
builder.Services.AddScoped<EDXS.Shared.Workflow.IWorkflowActionAdapter, HumanResource.Service.Workflow.EarningGroupWorkflowAdapter>();
builder.Services.AddScoped<EDXS.Shared.Workflow.IWorkflowActionAdapter, HumanResource.Service.Workflow.DeductionTypeWorkflowAdapter>();
builder.Services.AddScoped<EDXS.Shared.Workflow.IWorkflowActionAdapter, HumanResource.Service.Workflow.DeductionGroupWorkflowAdapter>();
builder.Services.AddScoped<EDXS.Shared.Workflow.IWorkflowActionAdapter, HumanResource.Service.Workflow.PayrollPeriodWorkflowAdapter>();
builder.Services.AddScoped<EDXS.Shared.Workflow.IWorkflowActionAdapter, HumanResource.Service.Workflow.HolidayTypeWorkflowAdapter>();
builder.Services.AddScoped<EDXS.Shared.Workflow.IWorkflowActionAdapter, HumanResource.Service.Workflow.AttendanceActivityWorkflowAdapter>();
builder.Services.AddScoped<EDXS.Shared.Workflow.IWorkflowActionAdapter, HumanResource.Service.Workflow.DeviceTypeWorkflowAdapter>();
builder.Services.AddScoped<EDXS.Shared.Workflow.IWorkflowActionAdapter, HumanResource.Service.Workflow.DeviceRegistrationWorkflowAdapter>();
builder.Services.AddScoped<EDXS.Shared.Workflow.IWorkflowActionAdapter, HumanResource.Service.Workflow.AttendancePolicyWorkflowAdapter>();
// The adapter reads the submitting employee from the signed-in identity while the request is
// prepared; the approved execution callback carries no user of its own.
builder.Services.AddHttpContextAccessor();


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
builder.Services.AddScoped<IVacancyNumberService, VacancyNumberService>();
builder.Services.AddScoped<IVacancyFulfillmentService, VacancyFulfillmentService>();

builder.Services.AddScoped<Auth.Service.Services.HrAttritionDashboardService>();
builder.Services.AddHttpClient("googleoauth", client => client.Timeout = TimeSpan.FromSeconds(30));
builder.Services.AddScoped<GoogleOAuthService>();
var app = builder.Build();
EDXS.Shared.Workflow.WorkflowIntegration.UseWorkflowErrors(app);
app.UseArgRuntime();
EDXS.Shared.Workflow.WorkflowIntegration.MapWorkflowAdapter(app);
app.UseScopeHeaderNormalization();
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.Run();
