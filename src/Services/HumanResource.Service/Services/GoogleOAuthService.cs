using System.Text.Json;
using System.Text.Json.Serialization;

namespace HumanResource.Service.Services
{
    /// <summary>
    /// The Google Cloud OAuth client an institution registers once, read from
    /// configuration. Only the consent flow needs these; a host that authenticates with
    /// a service-account key instead never touches them.
    /// </summary>
    public sealed class GoogleOAuthOptions
    {
        public string? ClientId { get; set; }
        public string? ClientSecret { get; set; }
        /// <summary>
        /// Must match a redirect URI registered on the OAuth client exactly, including
        /// scheme, port and path, or Google refuses the exchange.
        /// </summary>
        public string? RedirectUri { get; set; }

        public bool IsConfigured =>
            !string.IsNullOrWhiteSpace(ClientId)
            && !string.IsNullOrWhiteSpace(ClientSecret)
            && !string.IsNullOrWhiteSpace(RedirectUri);
    }

    public sealed class GoogleTokenExchangeResult
    {
        public bool Success { get; init; }
        public string? RefreshToken { get; init; }
        public string? Email { get; init; }
        public string? Message { get; init; }
    }

    /// <summary>
    /// Runs the per-employee Google consent flow: builds the URL the host is sent to,
    /// then trades the code Google hands back for a refresh token.
    ///
    /// This is the free alternative to a Workspace service account - it works with an
    /// ordinary @gmail.com because the host grants access to their own calendar rather
    /// than the institution impersonating them. The refresh token is stored on the same
    /// SMmeetinghostinfo row a service-account key would use.
    /// </summary>
    public sealed class GoogleOAuthService
    {
        private const string AuthEndpoint = "https://accounts.google.com/o/oauth2/v2/auth";
        private const string TokenEndpoint = "https://oauth2.googleapis.com/token";
        private const string UserInfoEndpoint = "https://www.googleapis.com/oauth2/v2/userinfo";
        private const string CalendarScope = "https://www.googleapis.com/auth/calendar.events";

        private readonly GoogleOAuthOptions _options;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<GoogleOAuthService> _logger;

        public GoogleOAuthService(
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            ILogger<GoogleOAuthService> logger)
        {
            _options = configuration.GetSection("GoogleOAuth").Get<GoogleOAuthOptions>() ?? new GoogleOAuthOptions();
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public bool IsConfigured => _options.IsConfigured;

        /// <summary>
        /// The consent URL for one employee. <paramref name="state"/> comes back on the
        /// callback unchanged, which is how the code is tied to the employee who started.
        /// </summary>
        public string BuildConsentUrl(string state)
        {
            var query = new Dictionary<string, string?>
            {
                ["client_id"] = _options.ClientId,
                ["redirect_uri"] = _options.RedirectUri,
                ["response_type"] = "code",
                // openid is what makes Google return an id_token, which is where the
                // address is read from without a second call.
                ["scope"] = $"{CalendarScope} openid email",
                // offline + consent together are what actually return a refresh token:
                // without prompt=consent Google omits it for an already-approved account.
                ["access_type"] = "offline",
                ["prompt"] = "consent",
                ["include_granted_scopes"] = "true",
                ["state"] = state
            };

            var encoded = string.Join("&", query
                .Where(pair => !string.IsNullOrWhiteSpace(pair.Value))
                .Select(pair => $"{Uri.EscapeDataString(pair.Key)}={Uri.EscapeDataString(pair.Value!)}"));

            return $"{AuthEndpoint}?{encoded}";
        }

        /// <summary>
        /// Trades the authorization code for a refresh token and resolves which Google
        /// account approved it, so the tab can show who is connected.
        /// </summary>
        public async Task<GoogleTokenExchangeResult> ExchangeCodeAsync(string code, CancellationToken cancellationToken)
        {
            if (!IsConfigured)
                return Failed("Google OAuth is not configured on this server.");

            try
            {
                var client = _httpClientFactory.CreateClient("googleoauth");
                using var content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["code"] = code,
                    ["client_id"] = _options.ClientId!,
                    ["client_secret"] = _options.ClientSecret!,
                    ["redirect_uri"] = _options.RedirectUri!,
                    ["grant_type"] = "authorization_code"
                });

                using var response = await client.PostAsync(TokenEndpoint, content, cancellationToken);
                var body = await response.Content.ReadAsStringAsync(cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Google refused the code exchange: {Status} {Body}", (int)response.StatusCode, Truncate(body));
                    return Failed("Google rejected the authorisation. Please try connecting again.");
                }

                var token = JsonSerializer.Deserialize<GoogleTokenResponse>(body);
                if (string.IsNullOrWhiteSpace(token?.RefreshToken))
                {
                    // Google only returns a refresh token on the first consent unless
                    // prompt=consent forces it; a missing one means the grant already
                    // existed and the user should revoke it before reconnecting.
                    return Failed(
                        "Google did not return a refresh token. Remove this app at myaccount.google.com/permissions and connect again.");
                }

                // The id_token already carries the address, so prefer it: it needs no
                // second call, which also keeps this working where the userinfo host
                // is unreachable. The API call stays as a fallback.
                var email = ReadEmailFromIdToken(token.IdToken)
                    ?? await ResolveEmailAsync(token.AccessToken, cancellationToken);
                return new GoogleTokenExchangeResult
                {
                    Success = true,
                    RefreshToken = token.RefreshToken,
                    Email = email
                };
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "The Google code exchange failed.");
                return Failed("Could not reach Google to complete the connection.");
            }
        }

        private async Task<string?> ResolveEmailAsync(string? accessToken, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(accessToken))
                return null;

            try
            {
                var client = _httpClientFactory.CreateClient("googleoauth");
                using var request = new HttpRequestMessage(HttpMethod.Get, UserInfoEndpoint);
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                using var response = await client.SendAsync(request, cancellationToken);
                if (!response.IsSuccessStatusCode)
                    return null;

                using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync(cancellationToken));
                return document.RootElement.TryGetProperty("email", out var email) ? email.GetString() : null;
            }
            catch (Exception ex)
            {
                // The connection still works without this; it only labels the tab.
                _logger.LogInformation(ex, "Could not read the Google account address after connecting.");
                return null;
            }
        }

        private static GoogleTokenExchangeResult Failed(string message) => new()
        {
            Success = false,
            Message = message
        };

        private static string Truncate(string value) => value.Length <= 400 ? value : value[..400];

        /// <summary>
        /// Pulls the email claim out of the id_token without validating it. The token
        /// came straight from Google's token endpoint over TLS in response to our own
        /// request, so it is not attacker-supplied; it is only ever used as a label.
        /// </summary>
        private static string? ReadEmailFromIdToken(string? idToken)
        {
            if (string.IsNullOrWhiteSpace(idToken))
                return null;

            try
            {
                var parts = idToken.Split('.');
                if (parts.Length < 2)
                    return null;

                var payload = parts[1].Replace('-', '+').Replace('_', '/');
                payload = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');

                using var document = JsonDocument.Parse(Convert.FromBase64String(payload));
                return document.RootElement.TryGetProperty("email", out var email) ? email.GetString() : null;
            }
            catch
            {
                return null;
            }
        }

        private sealed class GoogleTokenResponse
        {
            [JsonPropertyName("access_token")] public string? AccessToken { get; set; }
            [JsonPropertyName("refresh_token")] public string? RefreshToken { get; set; }
            [JsonPropertyName("id_token")] public string? IdToken { get; set; }
        }
    }
}
