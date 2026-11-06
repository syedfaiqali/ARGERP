using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Organization.Service.Infrastructure.Data;

namespace Organization.Service.Infrastructure.Authentication
{
    public sealed class SignupClientHashKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string SchemeName = "SignupClientHashKey";
        public const string HeaderName = "X-Client-Hash-Key";
        public const string QueryParameterName = "hashKey";

        private readonly OrganizationDbContext _context;

        public SignupClientHashKeyAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            OrganizationDbContext context)
            : base(options, logger, encoder)
        {
            _context = context;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var hashKey = GetHashKey();
            if (hashKey == null)
            {
                return AuthenticateResult.NoResult();
            }

            var signupClient = await _context.SignupClientHashKeys
                .AsNoTracking()
                .Where(x => x.IsActive && x.HashKey == hashKey)
                .Select(x => new { x.ClientId, x.ClientName })
                .FirstOrDefaultAsync(Context.RequestAborted);

            if (signupClient == null)
            {
                return AuthenticateResult.Fail("Invalid client hash key.");
            }

            var requestedClientId = GetRequestedClientId();
            if (requestedClientId.HasValue && requestedClientId.Value != signupClient.ClientId)
            {
                return AuthenticateResult.Fail("Client hash key does not match the requested client.");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, $"signup-client:{signupClient.ClientId}"),
                new Claim(ClaimTypes.Name, signupClient.ClientName),
                new Claim("clientid", signupClient.ClientId.ToString()),
                new Claim("clientId", signupClient.ClientId.ToString()),
                new Claim("ClientId", signupClient.ClientId.ToString()),
                new Claim("client_id", signupClient.ClientId.ToString()),
                new Claim("client_name", signupClient.ClientName),
                new Claim("auth_type", "signup_client_hash_key")
            };

            var identity = new ClaimsIdentity(claims, SchemeName);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, SchemeName);

            return AuthenticateResult.Success(ticket);
        }

        private string? GetHashKey()
        {
            string? hashKey = null;
            if (Request.Headers.TryGetValue(HeaderName, out var headerHashKey))
            {
                hashKey = headerHashKey.FirstOrDefault();
            }

            if (string.IsNullOrWhiteSpace(hashKey)
                && Request.Query.TryGetValue(QueryParameterName, out var queryHashKey))
            {
                hashKey = queryHashKey.FirstOrDefault();
            }

            hashKey = hashKey?.Trim();
            return string.IsNullOrWhiteSpace(hashKey)
                ? null
                : hashKey.ToLowerInvariant();
        }

        private int? GetRequestedClientId()
        {
            if (Request.Query.TryGetValue("clientId", out var queryClientId)
                && int.TryParse(queryClientId.FirstOrDefault(), out var parsedQueryClientId))
            {
                return parsedQueryClientId;
            }

            if (Request.Headers.TryGetValue("X-Client-Id", out var headerClientId)
                && int.TryParse(headerClientId.FirstOrDefault(), out var parsedHeaderClientId))
            {
                return parsedHeaderClientId;
            }

            return null;
        }
    }
}
