using System.Security.Claims;
using Microsoft.Data.SqlClient;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
namespace ARGERP.Runtime;
public static class RuntimeExtensions
{
    public static void AddArgAuthentication(this WebApplicationBuilder builder)
    {
        builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: false).AddEnvironmentVariables();
        var key = builder.Configuration["JwtSettings:SecretKey"] ?? throw new InvalidOperationException("Run scripts/setup.ps1 first to configure local signing keys.");
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => { options.TokenValidationParameters = new()
        {
            ValidateIssuer = true, ValidateAudience = true, ValidateLifetime = true, ValidateIssuerSigningKey = true,
            ValidIssuer = "ARGERP", ValidAudience = "ARGERP", IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), ClockSkew = TimeSpan.FromSeconds(30)
        };
            options.Events = new JwtBearerEvents { OnTokenValidated = async context =>
            {
                if (!int.TryParse(context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier), out var id)
                    || !int.TryParse(context.Principal?.FindFirstValue("sessionVersion"), out var version))
                { context.Fail("Invalid session."); return; }
                await using var connection = new SqlConnection(builder.Configuration.GetConnectionString("OrganizationDB"));
                await connection.OpenAsync(context.HttpContext.RequestAborted);
                await using var command = new SqlCommand("SELECT COUNT(*) FROM dbo.ArgUsers WHERE Id=@id AND Blocked=0 AND ISNULL(SessionVersion,0)=@version", connection);
                command.Parameters.AddWithValue("@id", id); command.Parameters.AddWithValue("@version", version);
                if (Convert.ToInt32(await command.ExecuteScalarAsync(context.HttpContext.RequestAborted)) != 1) context.Fail("Session expired.");
            } };
        });
        builder.Services.AddAuthorization(options => options.FallbackPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddProblemDetails();
    }
    public static void UseArgRuntime(this WebApplication app)
    {
        app.UseExceptionHandler(handler => handler.Run(async context =>
        {
            var error = context.Features.Get<IExceptionHandlerFeature>()?.Error;
            context.Response.StatusCode = error is ArgumentException ? 400 : error is UnauthorizedAccessException ? 403 : 500;
            await context.Response.WriteAsJsonAsync(new { error = error is ArgumentException ? error.Message : "The request could not be completed. Please try again.", traceId = context.TraceIdentifier });
        }));
        app.UseAuthentication(); app.UseAuthorization();
        app.MapGet("/health", () => Results.Ok(new { status = "Healthy" })).AllowAnonymous();
        app.MapControllers();
    }
}
