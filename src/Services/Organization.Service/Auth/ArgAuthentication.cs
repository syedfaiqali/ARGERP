using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
namespace ARGERP.Auth;
public sealed class ArgUser
{
    public int Id { get; set; }
    public string Username { get; set; } = "";
    public string FullName { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public int ClientId { get; set; } = 1;
    public int EntityId { get; set; } = 1;
    public int? EmployeeId { get; set; }
    public bool Blocked { get; set; }
    public int? SessionVersion { get; set; }
}
public sealed class ArgAuthDbContext(DbContextOptions<ArgAuthDbContext> options) : DbContext(options)
{
    public DbSet<ArgUser> Users => Set<ArgUser>();
    protected override void OnModelCreating(ModelBuilder model)
    {
        model.Entity<ArgUser>().ToTable("ArgUsers");
        model.Entity<ArgUser>().Property(x => x.Username).HasMaxLength(100);
        model.Entity<ArgUser>().HasIndex(x => x.Username).IsUnique();
    }
}
[ApiController, Route("api/auth")]
public sealed class ArgAuthController(ArgAuthDbContext db, IConfiguration config) : ControllerBase
{
    public sealed record LoginRequest(string Username, string Password);
    public sealed record PasswordRequest(string CurrentPassword, string NewPassword);
    [HttpPost("login"), AllowAnonymous]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrEmpty(request.Password)) return Unauthorized(new { error = "Invalid username or password." });
        var user = await db.Users.SingleOrDefaultAsync(x => x.Username == request.Username.Trim() && !x.Blocked, ct);
        if (user is null || new PasswordHasher<ArgUser>().VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
            return Unauthorized(new { error = "Invalid username or password." });
        var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, user.Id.ToString()), new(ClaimTypes.Name, user.Username), new("sub", user.Id.ToString()) };
        foreach (var name in new[] { "clientId", "ClientId", "client_id", "clientid" }) claims.Add(new(name, user.ClientId.ToString()));
        foreach (var name in new[] { "entityId", "EntityId", "entity_id", "CampusId", "campusId", "campus_id" }) claims.Add(new(name, user.EntityId.ToString()));
        foreach (var name in new[] { "userId", "userid", "UserId" }) claims.Add(new(name, user.Id.ToString()));
        if (user.EmployeeId is int employee) { claims.Add(new("empId", employee.ToString())); claims.Add(new("employee_id", employee.ToString())); }
        claims.Add(new("sessionVersion", (user.SessionVersion ?? 0).ToString()));
        var expires = DateTime.UtcNow.AddHours(24);
        var token = new JwtSecurityToken("ARGERP", "ARGERP", claims, expires: expires, signingCredentials: new(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JwtSettings:SecretKey"]!)), SecurityAlgorithms.HmacSha256));
        return Ok(new { accessToken = new JwtSecurityTokenHandler().WriteToken(token), expiresAt = expires, user = new { userid = user.Id, username = user.Username, fullName = user.FullName, clientId = user.ClientId, entityId = user.EntityId, empId = user.EmployeeId, displayRole = "HR User" } });
    }
    [HttpGet("me")]
    public async Task<IActionResult> Me(CancellationToken ct) => Ok(await db.Users.AsNoTracking().Where(x => x.Id == int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!)).Select(x => new { x.Id, x.Username, x.FullName, x.EntityId }).SingleAsync(ct));
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(PasswordRequest request, CancellationToken ct)
    {
        var user = await db.Users.SingleAsync(x => x.Id == int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!), ct);
        var hasher = new PasswordHasher<ArgUser>();
        if (hasher.VerifyHashedPassword(user, user.PasswordHash, request.CurrentPassword) == PasswordVerificationResult.Failed) return BadRequest(new { error = "Current password is incorrect." });
        if (string.IsNullOrEmpty(request.NewPassword) || request.NewPassword.Length < 12) return BadRequest(new { error = "Use at least 12 characters." });
        user.PasswordHash = hasher.HashPassword(user, request.NewPassword); user.SessionVersion = (user.SessionVersion ?? 0) + 1; await db.SaveChangesAsync(ct); return NoContent();
    }
}
