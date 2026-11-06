using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HumanResource.Service.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.Service.Controllers;

[ApiController, Authorize, Route("api/Settings/v3")]
public sealed class HrSettingsController(HumanResourceDbContext db, IConfiguration configuration) : ControllerBase
{
    [HttpGet]
    public IActionResult General() => Ok(new { isSuccessCall = true, data = new { dateFormat = "dd/MM/yyyy", gridPaging = 20 } });

    [HttpGet("grid-paging")]
    public IActionResult Paging() => Ok(new { gridPaging = 20, pageSize = 20, rowsPerPage = 20, pageSizeOptions = new[] { 10, 20, 50, 100 } });

    [HttpGet("attachments")]
    public IActionResult Attachments() => Ok(Array.Empty<object>());

    [HttpGet("attachments/lookups")]
    public async Task<IActionResult> AttachmentLookups(CancellationToken ct) => Ok(new
    {
        documentTypes = await db.DocumentTypes.AsNoTracking().Where(x => x.UtBlock != true && x.ClientId == 1).Select(x => new { id = x.Id, code = x.Code, name = x.Stxt }).ToListAsync(ct),
        storageTypes = Array.Empty<object>(), downloadDrives = Array.Empty<object>(), uploadDrives = Array.Empty<object>()
    });

    [HttpGet("online-meeting/{provider}")]
    public IActionResult Meeting(string provider) => provider == "zoom"
        ? Ok(new { configuration = new { clientId = configuration["HR:Zoom:ClientId"] ?? "" } })
        : BadRequest(new { error = "Unknown meeting provider." });

    public sealed record ZoomTest(string? ZoomAccountId, string? ZoomClientSecret, string? ZoomHostEmail, string? ZoomUserId);
    [HttpPost("online-meeting/zoom/test-connection")]
    public async Task<IActionResult> TestZoom(ZoomTest request, [FromServices] IHttpClientFactory clients, CancellationToken ct)
    {
        var clientId = configuration["HR:Zoom:ClientId"];
        if (string.IsNullOrWhiteSpace(clientId)) return BadRequest(new { success = false, message = "Configure HR:Zoom:ClientId in HR settings first." });
        var host = string.IsNullOrWhiteSpace(request.ZoomUserId) ? request.ZoomHostEmail : request.ZoomUserId;
        if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(request.ZoomAccountId) || string.IsNullOrWhiteSpace(request.ZoomClientSecret))
            return BadRequest(new { success = false, message = "Account ID, client secret and host are required." });
        using var client = clients.CreateClient("googleoauth");
        using var tokenRequest = new HttpRequestMessage(HttpMethod.Post, "https://zoom.us/oauth/token");
        tokenRequest.Headers.Authorization = new("Basic", Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(clientId + ":" + request.ZoomClientSecret)));
        tokenRequest.Content = new FormUrlEncodedContent(new Dictionary<string, string> { ["grant_type"] = "account_credentials", ["account_id"] = request.ZoomAccountId });
        using var tokenResponse = await client.SendAsync(tokenRequest, ct);
        if (!tokenResponse.IsSuccessStatusCode) return BadRequest(new { success = false, message = "Zoom rejected the credentials." });
        using var token = System.Text.Json.JsonDocument.Parse(await tokenResponse.Content.ReadAsStringAsync(ct));
        client.DefaultRequestHeaders.Authorization = new("Bearer", token.RootElement.GetProperty("access_token").GetString());
        using var userResponse = await client.GetAsync("https://api.zoom.us/v2/users/" + Uri.EscapeDataString(host), ct);
        if (!userResponse.IsSuccessStatusCode) return BadRequest(new { success = false, message = "Zoom could not find the host." });
        using var user = System.Text.Json.JsonDocument.Parse(await userResponse.Content.ReadAsStringAsync(ct));
        return Ok(new { success = true, message = "Zoom connection successful.", zoomUserId = user.RootElement.GetProperty("id").GetString(), zoomHostEmail = user.RootElement.GetProperty("email").GetString() });
    }
}
