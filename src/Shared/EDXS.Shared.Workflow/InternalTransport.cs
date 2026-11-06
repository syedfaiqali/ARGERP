using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace EDXS.Shared.Workflow;

public sealed class InternalAuthenticator(IConfiguration config, IMemoryCache cache, WorkflowNonceStore? nonces = null)
{
    private byte[] Key(string service)
    {
        var key = config[$"Workflow:ServiceKeys:{service}"];
        if (string.IsNullOrEmpty(key) || Encoding.UTF8.GetByteCount(key) < 32)
            throw new WorkflowException(503, "ConfigurationMissing", "Workflow service authentication is not configured.");
        return Encoding.UTF8.GetBytes(key);
    }

    public void Sign(HttpRequestMessage request, string service, string body)
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        var nonce = Guid.NewGuid().ToString("N");
        var path = request.RequestUri!.PathAndQuery;
        var input = $"{service}\n{request.Method}\n{path}\n{timestamp}\n{nonce}\n{WorkflowJson.Hash(body)}";
        request.Headers.Add("X-Workflow-Service", service);
        request.Headers.Add("X-Workflow-Time", timestamp);
        request.Headers.Add("X-Workflow-Nonce", nonce);
        request.Headers.Add("X-Workflow-Signature", Convert.ToHexString(HMACSHA256.HashData(Key(service), Encoding.UTF8.GetBytes(input))));
    }

    public async Task<string> VerifyAsync(HttpRequest request, string[] allowedServices, CancellationToken ct)
    {
        var service = request.Headers["X-Workflow-Service"].ToString();
        var timestamp = request.Headers["X-Workflow-Time"].ToString();
        var nonce = request.Headers["X-Workflow-Nonce"].ToString();
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        if (!allowedServices.Contains(service, StringComparer.Ordinal) || !long.TryParse(timestamp, out var unix) ||
            unix < now - 120 || unix > now + 120 || !Guid.TryParseExact(nonce, "N", out _))
            throw new WorkflowException(401, "InvalidServiceIdentity", "Invalid internal service identity.");
        request.EnableBuffering();
        request.Body.Position = 0;
        using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
        var body = await reader.ReadToEndAsync(ct);
        request.Body.Position = 0;
        var input = $"{service}\n{request.Method}\n{request.PathBase}{request.Path}{request.QueryString}\n{timestamp}\n{nonce}\n{WorkflowJson.Hash(body)}";
        byte[] signature;
        try { signature = Convert.FromHexString(request.Headers["X-Workflow-Signature"].ToString()); }
        catch (FormatException) { throw new WorkflowException(401, "InvalidSignature", "Invalid service signature."); }
        var expected = HMACSHA256.HashData(Key(service), Encoding.UTF8.GetBytes(input));
        if (!CryptographicOperations.FixedTimeEquals(signature, expected)) throw new WorkflowException(401, "InvalidSignature", "Invalid service signature.");
        if (nonces != null && await nonces.CheckAsync(service, nonce)) return service;
        if (!config.GetValue<bool>("Workflow:SingleReplica"))
            throw new WorkflowException(503, "ReplayProtectionUnavailable", "Shared workflow replay protection is unavailable.");
        // Local nonce protection is permitted only for an explicitly single-replica setup.
        lock (cache)
        {
            var key = $"workflow-nonce:{service}:{nonce}";
            if (cache.TryGetValue(key, out _)) throw new WorkflowException(401, "ReplayedRequest", "This service request has already been received.");
            cache.Set(key, true, TimeSpan.FromMinutes(5));
        }
        return service;
    }
}

public sealed class WorkflowNonceStore(IConfiguration config) : IDisposable
{
    private readonly Lazy<StackExchange.Redis.ConnectionMultiplexer?> connection = new(() =>
    {
        var address = config.GetConnectionString("redis");
        if (string.IsNullOrWhiteSpace(address)) return null;
        var options = StackExchange.Redis.ConfigurationOptions.Parse(address);
        options.AbortOnConnectFail = false; // Recover when Redis returns, without restarting the service.
        // Every internal workflow call checks a nonce, so an unreachable Redis must fail fast.
        // The library defaults (5s timeouts, queueing commands while disconnected) stack up across
        // the chained service calls until the gateway aborts the request.
        options.ConnectRetry = 1;
        options.ConnectTimeout = 1000;
        options.SyncTimeout = 1000;
        options.AsyncTimeout = 1000;
        options.BacklogPolicy = StackExchange.Redis.BacklogPolicy.FailFast;
        return StackExchange.Redis.ConnectionMultiplexer.Connect(options);
    });
    public async Task<bool> CheckAsync(string service, string nonce)
    {
        try
        {
            if (connection.Value is not { IsConnected: true } redis) return false;
            if (!await redis.GetDatabase().StringSetAsync($"EDXS:workflow:nonce:{service}:{nonce}", "1", TimeSpan.FromMinutes(5), StackExchange.Redis.When.NotExists))
                throw new WorkflowException(401, "ReplayedRequest", "This service request has already been received.");
            return true;
        }
        // Let the caller select the configured policy. A local single-replica setup is
        // explicitly allowed to use its in-memory nonce store when Redis is down; a
        // multi-replica setup will still reject the request with a 503 in
        // InternalAuthenticator.
        // RedisTimeoutException derives from TimeoutException, not RedisException.
        catch (Exception ex) when (ex is StackExchange.Redis.RedisException or TimeoutException) { return false; }
    }
    public void Dispose() { if (connection.IsValueCreated) connection.Value?.Dispose(); }
}

public sealed class WorkflowHttpClient(HttpClient http, InternalAuthenticator authenticator, IConfiguration config)
{
    public async Task<T> PostAsync<T>(string destination, string path, object body, CancellationToken ct, string? sender = null)
    {
        var address = config[$"Workflow:Services:{destination}"];
        if (!Uri.TryCreate(address, UriKind.Absolute, out var baseUri) || baseUri.Scheme is not ("http" or "https"))
            throw new WorkflowException(503, "ConfigurationMissing", "A workflow service address is not configured.");
        var json = JsonSerializer.Serialize(body, WorkflowJson.Options);
        using var request = new HttpRequestMessage(HttpMethod.Post, new Uri(baseUri, path)) { Content = new StringContent(json, Encoding.UTF8, "application/json") };
        authenticator.Sign(request, sender ?? config["Workflow:ServiceName"] ?? throw new InvalidOperationException("Workflow:ServiceName is required."), json);
        HttpResponseMessage response;
        try { response = await http.SendAsync(request, ct); }
        catch (HttpRequestException) { throw new WorkflowException(503, "ServiceUnavailable", "Workflow services are temporarily unavailable. Please retry."); }
        // Only a cancellation the caller actually asked for propagates. HttpClient's own timeout
        // also surfaces as TaskCanceledException, and when it races a linked token the caller's
        // token can read as cancelled too, so the caller's token alone decides.
        catch (OperationCanceledException) when (ct.IsCancellationRequested) { throw; }
        catch (OperationCanceledException) { throw new WorkflowException(503, "ServiceUnavailable", "Workflow service request timed out."); }
        using (response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var payload = await response.Content.ReadAsStringAsync(ct);
                string? code = null, message = null; Guid? instanceId = null;
                try
                {
                    var error = JsonDocument.Parse(payload).RootElement;
                    code = error.TryGetProperty("code", out var c) ? c.GetString() : null;
                    message = error.TryGetProperty("error", out var m) ? m.GetString() : null;
                    if (error.TryGetProperty("instanceId", out var i) && i.ValueKind == JsonValueKind.String && i.TryGetGuid(out var id)) instanceId = id;
                }
                catch (JsonException) { }
                 throw new WorkflowException((int)response.StatusCode >= 500 ? 503 : (int)response.StatusCode, code ?? "ServiceError", message ?? "The workflow service could not complete the operation.", instanceId);
            }
            return (await response.Content.ReadFromJsonAsync<T>(WorkflowJson.Options, ct))!;
        }
    }
}

// Versioned AES-GCM envelopes support key rotation without losing indefinitely retained drafts.
// Keys must be injected by the environment/secret store; there are no built-in keys.
public sealed class WorkflowPayloadStore(IConfiguration config)
{
    // New submissions and execution receipts are ordinary JSON. The database columns are
    // reused so this change does not require a schema migration or rewrite approval history.
    public string Write<T>(T value) => JsonSerializer.Serialize(value, WorkflowJson.Options);

    public T Read<T>(string stored, string purpose)
    {
        var json = stored.TrimStart().StartsWith('{') ? stored : ReadLegacy(stored, purpose);
        try
        {
            return JsonSerializer.Deserialize<T>(json, WorkflowJson.Options)
                ?? throw new JsonException("Missing workflow details.");
        }
        catch (JsonException)
        {
            throw new WorkflowException(409, "PayloadInvalid", "The saved request details are invalid. Restore the original details before processing this request.");
        }
    }

    // Read compatibility only: do not strand older requests or already committed receipts
    // whose original key is still available. No new data is encrypted.
    private string ReadLegacy(string envelope, string purpose)
    {
        try
        {
            var parts = envelope.Split('.');
            if (parts.Length != 4) throw new FormatException();
            var key = config[$"Workflow:PayloadKeys:{parts[0]}"];
            if (string.IsNullOrWhiteSpace(key)) throw new FormatException();
            var cipher = Convert.FromBase64String(parts[3]); var output = new byte[cipher.Length];
            using var aes = new AesGcm(Convert.FromBase64String(key), 16);
            aes.Decrypt(Convert.FromBase64String(parts[1]), cipher, Convert.FromBase64String(parts[2]), output, Encoding.UTF8.GetBytes(purpose));
            return Encoding.UTF8.GetString(output);
        }
        catch (Exception ex) when (ex is FormatException or CryptographicException or ArgumentException)
        {
            throw new WorkflowException(409, "PayloadUnavailable", "The saved details of this older request are unavailable. Its assignment and approval history are preserved; restore the original details before the approved record can be saved.");
        }
    }
}
