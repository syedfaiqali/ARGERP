using HumanResource.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HumanResource.Service.Controllers;

[ApiController, Authorize, Route("api/Assistant/v3")]
public sealed class AssistantV3Controller(AssistantChatService assistant) : ControllerBase
{
    [HttpPost("chat")]
    [RequestSizeLimit(32 * 1024)]
    public async Task<IActionResult> Chat(AssistantRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Message)) return BadRequest(new { message = "Message is required." });
        if (request.Message.Length > 2000) return BadRequest(new { message = "Message cannot exceed 2000 characters." });
        var answer = await assistant.ChatAsync(request, ct);
        return answer.Reply is null ? StatusCode(503, new { message = answer.Error }) : Ok(new { reply = answer.Reply });
    }
}
