using System.Text.Json;
using EDXS.Shared.Workflow;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Organization.Service.Infrastructure.Data;
namespace ARGERP.Auth;

[ApiController, AllowAnonymous, Route("internal/workflow")]
public sealed class WorkflowDirectoryController(ArgAuthDbContext users, OrganizationDbContext organization, InternalAuthenticator auth) : ControllerBase
{
    public sealed record TenantRequest(int ClientId);
    private async Task<T> Read<T>(CancellationToken ct)
    {
        await auth.VerifyAsync(Request, ["workflow", "organization", "humanresource"], ct);
        return await Request.ReadFromJsonAsync<T>(WorkflowJson.Options, ct) ?? throw new WorkflowException(400, "InvalidRequest", "Request body is required.");
    }
    private Task<int[]> Entities(int client, CancellationToken ct) => organization.OrgLegalentityLegacy.AsNoTracking().Where(x => x.clientid == client && !x.utblock).Select(x => x.Id).ToArrayAsync(ct);
    [HttpPost("identity")]
    public async Task<WorkflowIdentity> Identity(CancellationToken ct)
    {
        var request = await Read<IdentityRequest>(ct);
        var user = await users.Users.AsNoTracking().SingleOrDefaultAsync(x => x.Id == request.Actor.UserId && x.ClientId == request.Actor.ClientId && !x.Blocked, ct)
            ?? throw new WorkflowException(403, "AccountUnavailable", "The user account is unavailable.");
        var entities = await Entities(user.ClientId, ct);
        if (request.Actor.EntityId is int entity && !entities.Contains(entity)) throw new WorkflowException(400, "InvalidBranch", "Choose an existing branch.");
        // This installation deliberately has no screen/action permission system.
        return new(user.Id, user.ClientId, user.FullName, true, true, entities, true);
    }
    [HttpPost("directory")]
    public async Task<WorkflowDirectory> Directory(CancellationToken ct)
    {
        var request = await Read<TenantRequest>(ct);
        return await BuildDirectory(request.ClientId, ct);
    }
    private async Task<WorkflowDirectory> BuildDirectory(int client, CancellationToken ct)
    {
        var ids = await Entities(client, ct);
        var rows = await users.Users.AsNoTracking().Where(x => x.ClientId == client && !x.Blocked).ToListAsync(ct);
        var entities = await organization.OrgLegalentityLegacy.AsNoTracking().Where(x => x.clientid == client && !x.utblock).Select(x => new WorkflowLookup(x.Id, x.stxt ?? x.entityno ?? "Branch")).ToListAsync(ct);
        return new(rows.Select(x => new ApproverUser(x.Id, x.FullName, null, null, true, ids)).ToList(), [], entities);
    }
    [HttpPost("resolve")]
    public async Task<List<ApproverUser>> Resolve(CancellationToken ct)
    {
        var request = await Read<ResolveUsersRequest>(ct);
        var directory = await BuildDirectory(request.ClientId, ct);
        var selected = request.Selectors.Where(x => x.Kind.Equals("User", StringComparison.OrdinalIgnoreCase)).Select(x => int.TryParse(x.Id, out var id) ? id : 0).ToHashSet();
        if (request.RelatedUser == RelatedUsers.ReportTo)
        {
            var requester = await users.Users.FindAsync([request.RequesterId], ct);
            if (requester?.EmployeeId is int employee)
            {
                var managers = await organization.Database.SqlQueryRaw<int>("SELECT reportingtoemployeeid AS [Value] FROM dbo.EREmployeeinfohdr WHERE Id=@employee AND reportingtoemployeeid IS NOT NULL", new SqlParameter("@employee", employee)).ToListAsync(ct);
                var managerUsers = await users.Users.Where(x => x.ClientId == request.ClientId && x.EmployeeId.HasValue && managers.Contains(x.EmployeeId.Value) && !x.Blocked).Select(x => x.Id).ToListAsync(ct);
                if (managerUsers.Count > 0) selected = managerUsers.ToHashSet();
            }
        }
        return directory.Users.Where(x => selected.Contains(x.UserId) && (request.AllowSelfAction || x.UserId != request.RequesterId)).ToList();
    }
    [HttpPost("users")]
    public async Task<List<WorkflowUser>> Names(CancellationToken ct)
    {
        var request = await Read<UserNamesRequest>(ct);
        return await users.Users.Where(x => x.ClientId == request.ClientId && request.UserIds.Contains(x.Id)).Select(x => new WorkflowUser(x.Id, x.FullName, null)).ToListAsync(ct);
    }
    [HttpPost("tenant")]
    public async Task<WorkflowLookup> Tenant(CancellationToken ct) { var request = await Read<TenantRequest>(ct); return new(request.ClientId, "ARG ERP"); }
    [HttpPost("tenants")]
    public async Task<List<WorkflowLookup>> Tenants(CancellationToken ct) { var request = await Read<IdentityRequest>(ct); return [new(request.Actor.ClientId, "ARG ERP")]; }
    [HttpPost("screens")]
    public async Task<List<WorkflowScreen>> Screens(CancellationToken ct)
    {
        await auth.VerifyAsync(Request, ["workflow"], ct);
        return PilotActions.All.Where(x => x.Service is "humanresource" or "organization" && x.ScreenKey != "EPLoanrequest").Select(x => new WorkflowScreen(x.ScreenKey, x.Name, x.Module)).ToList();
    }
    [HttpPost("admins")]
    public async Task<List<int>> Admins(CancellationToken ct) { var request = await Read<TenantRequest>(ct); return await users.Users.Where(x => x.ClientId == request.ClientId && !x.Blocked).Select(x => x.Id).ToListAsync(ct); }
}
