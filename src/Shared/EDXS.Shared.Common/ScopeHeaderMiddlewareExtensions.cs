using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace EDXS.Shared.Common;

public static class ScopeHeaderMiddlewareExtensions
{
    private static readonly string[] ClientHeaderNames = ["X-Client-Id", "Client-Id"];
    private static readonly string[] ClientQueryNames = ["clientId", "ClientId"];
    private static readonly string[] ClientClaimNames = ["clientId", "ClientId", "client_id", "clientid", "ClientID"];

    private static readonly string[] EntityHeaderNames = ["X-Campus-Id", "Campus-Id", "X-Entity-Id", "Entity-Id"];
    private static readonly string[] EntityQueryNames = ["entityId", "EntityId", "campusId", "CampusId"];
    private static readonly string[] EntityClaimNames = ["entityId", "EntityId", "entity_id", "entityid", "EntityID", "campusId", "CampusId"];

    public static IApplicationBuilder UseScopeHeaderNormalization(this IApplicationBuilder app)
    {
        return app.Use(async (context, next) =>
        {
            var clientId = ReadScopeValue(context, ClientHeaderNames, ClientQueryNames, ClientClaimNames);
            var entityId = ReadScopeValue(context, EntityHeaderNames, EntityQueryNames, EntityClaimNames);

            if (!string.IsNullOrWhiteSpace(clientId))
            {
                context.Request.Headers["X-Client-Id"] = clientId;
                context.Request.Headers["Client-Id"] = clientId;
                context.Items["ClientId"] = clientId;
            }

            if (!string.IsNullOrWhiteSpace(entityId))
            {
                context.Request.Headers["X-Entity-Id"] = entityId;
                context.Request.Headers["Entity-Id"] = entityId;
                context.Request.Headers["X-Campus-Id"] = entityId;
                context.Request.Headers["Campus-Id"] = entityId;
                context.Items["EntityId"] = entityId;
            }

            if (context.User.Identity?.IsAuthenticated == true)
            {
                context.User = NormalizeClaims(context.User, clientId, entityId);
            }

            await next(context);
        });
    }

    private static string? ReadScopeValue(HttpContext context, string[] headerNames, string[] queryNames, string[] claimNames)
    {
        foreach (var name in headerNames)
        {
            if (context.Request.Headers.TryGetValue(name, out var headerValue)
                && IsPositiveInt(headerValue.ToString()))
            {
                return headerValue.ToString();
            }
        }

        foreach (var name in queryNames)
        {
            if (context.Request.Query.TryGetValue(name, out var queryValue)
                && IsPositiveInt(queryValue.ToString()))
            {
                return queryValue.ToString();
            }
        }

        foreach (var name in claimNames)
        {
            var claimValue = context.User.Claims.FirstOrDefault(c => string.Equals(c.Type, name, StringComparison.OrdinalIgnoreCase))?.Value;
            if (IsPositiveInt(claimValue))
            {
                return claimValue;
            }
        }

        return null;
    }

    private static ClaimsPrincipal NormalizeClaims(ClaimsPrincipal principal, string? clientId, string? entityId)
    {
        if (string.IsNullOrWhiteSpace(clientId) && string.IsNullOrWhiteSpace(entityId))
        {
            return principal;
        }

        var identities = principal.Identities.Select(identity =>
        {
            var claims = new List<Claim>();
            if (!string.IsNullOrWhiteSpace(clientId))
            {
                claims.Add(new Claim("clientId", clientId));
                claims.Add(new Claim("ClientId", clientId));
                claims.Add(new Claim("client_id", clientId));
            }

            if (!string.IsNullOrWhiteSpace(entityId))
            {
                claims.Add(new Claim("entityId", entityId));
                claims.Add(new Claim("EntityId", entityId));
                claims.Add(new Claim("entity_id", entityId));
                claims.Add(new Claim("campusId", entityId));
                claims.Add(new Claim("CampusId", entityId));
            }

            claims.AddRange(identity.Claims.Where(claim =>
                (string.IsNullOrWhiteSpace(clientId) || !ClientClaimNames.Contains(claim.Type, StringComparer.OrdinalIgnoreCase)) &&
                (string.IsNullOrWhiteSpace(entityId) || !EntityClaimNames.Contains(claim.Type, StringComparer.OrdinalIgnoreCase))));

            return new ClaimsIdentity(claims, identity.AuthenticationType, identity.NameClaimType, identity.RoleClaimType);
        });

        return new ClaimsPrincipal(identities);
    }

    private static bool IsPositiveInt(string? value)
    {
        return int.TryParse(value, out var parsed) && parsed > 0;
    }
}
