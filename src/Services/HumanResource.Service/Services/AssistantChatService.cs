using System.Net.Http.Json;
using System.Text.Json;

namespace HumanResource.Service.Services;

public sealed record AssistantHistory(string? Role, string? Text);
public sealed record AssistantRequest(string? Message, List<AssistantHistory>? History);
public sealed record AssistantAnswer(string? Reply, string? Error = null);

/// <summary>Standalone HR assistant using the same Gemini transport as EDXS v2.</summary>
public sealed class AssistantChatService(HttpClient http, IConfiguration config)
{
    private const string Instructions = """
        You are the Al Rahim Group ARG ERP assistant. Help with Human Resources,
        Organization and Auth only. Match the user's language, including Roman Urdu.
        Give concise practical answers. You have no database access and cannot view,
        approve or modify records. Never invent employee, salary, attendance or birthday
        information. Direct users to the relevant screen to see their actual data.
        The generic Workflow module, Loan, Vacancy and Organogram are excluded.
        Do not suggest other business modules or external EDXS services.
        When useful, use these exact internal Markdown links:
        [Employee Profile](/hr/employee-profile), [Change Password](/auth/change-password).
        The global search in the header finds the other available screens by name.
        Never claim to have executed an action, and never ask for passwords or API keys.
        """;

    public async Task<AssistantAnswer> ChatAsync(AssistantRequest request, CancellationToken ct)
    {
        var keys = (config.GetSection("Gemini:ApiKeys").Get<string[]>() ?? [])
            .Append(config["Gemini:ApiKey"] ?? "").Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim()).Where(x => !x.StartsWith("Gemini__", StringComparison.Ordinal)).Distinct().ToArray();
        var models = (config.GetSection("Gemini:Models").Get<string[]>() ?? [])
            .Prepend(config["Gemini:Model"] ?? "").Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToArray();
        if (keys.Length == 0 || models.Length == 0)
            return new(null, "AI Assistant is not configured. Ask your administrator to configure the server's Gemini settings.");

        var history = (request.History ?? []).Where(x => !string.IsNullOrWhiteSpace(x.Text)).TakeLast(8)
            .Select(x => new { role = x.Role == "assistant" ? "model" : "user", parts = new[] { new { text = x.Text!.Trim()[..Math.Min(x.Text.Trim().Length, 600)] } } })
            .SkipWhile(x => x.role == "model").ToList();
        history.Add(new { role = "user", parts = new[] { new { text = request.Message!.Trim() } } });
        var payload = new { system_instruction = new { parts = new[] { new { text = Instructions } } }, contents = history, generationConfig = new { maxOutputTokens = 2048 } };
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(ct);
        timeout.CancelAfter(TimeSpan.FromSeconds(45));
        // Bound retries to keep a failed upstream request within the UI timeout.
        foreach (var (model, key) in models.SelectMany(model => keys.Select(key => (model, key))).Take(2))
        {
            try
            {
                using var message = new HttpRequestMessage(HttpMethod.Post, $"https://generativelanguage.googleapis.com/v1beta/models/{Uri.EscapeDataString(model)}:generateContent") { Content = JsonContent.Create(payload) };
                message.Headers.Add("x-goog-api-key", key);
                using var response = await http.SendAsync(message, timeout.Token);
                if (!response.IsSuccessStatusCode) continue;
                await using var stream = await response.Content.ReadAsStreamAsync(timeout.Token);
                using var json = await JsonDocument.ParseAsync(stream, cancellationToken: timeout.Token);
                if (!json.RootElement.TryGetProperty("candidates", out var candidates)) continue;
                foreach (var candidate in candidates.EnumerateArray())
                {
                    if (!candidate.TryGetProperty("content", out var content) || !content.TryGetProperty("parts", out var parts)) continue;
                    var reply = string.Join("\n", parts.EnumerateArray().Where(x => !x.TryGetProperty("thought", out var thought) || thought.ValueKind != JsonValueKind.True)
                        .Where(x => x.TryGetProperty("text", out _)).Select(x => x.GetProperty("text").GetString()));
                    if (!string.IsNullOrWhiteSpace(reply)) return new(reply);
                }
            }
            catch (Exception ex) when (ex is HttpRequestException or JsonException or OperationCanceledException or InvalidOperationException)
            {
                if (ct.IsCancellationRequested) throw;
                if (timeout.IsCancellationRequested) break;
            }
        }
        return new(null, "AI Assistant is temporarily unavailable. Please try again shortly.");
    }
}
