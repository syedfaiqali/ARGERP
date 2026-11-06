using Microsoft.AspNetCore.Mvc;

namespace Organization.Service.Controllers
{
    internal static class RequestScopeHelper
    {
        public static int GetClientId(ControllerBase controller)
        {
            var clientId = TryReadInt(controller, "X-Client-Id", "Client-Id", "clientId", "ClientId", "ClientId", "clientId", "client_id", "clientid");
            if (clientId.HasValue) return clientId.Value;

            throw new UnauthorizedAccessException("ClientId not found in token or headers");
        }

        public static int GetClientIdOrDefault(ControllerBase controller, int defaultValue = 0)
        {
            return TryReadInt(controller, "X-Client-Id", "Client-Id", "clientId", "ClientId", "ClientId", "clientId", "client_id", "clientid") ?? defaultValue;
        }

        public static int? GetOptionalEntityId(ControllerBase controller)
        {
            return TryReadInt(
                controller,
                new[] { "X-Campus-Id", "Campus-Id", "X-Entity-Id", "Entity-Id" },
                new[] { "entityId", "EntityId", "campusId", "CampusId" },
                new[] { "EntityId", "entityId", "entity_id", "entityid", "campusId", "CampusId" });
        }

        public static int? GetHeaderOrClaimEntityId(ControllerBase controller)
        {
            return TryReadInt(
                controller,
                new[] { "X-Campus-Id", "Campus-Id", "X-Entity-Id", "Entity-Id" },
                Array.Empty<string>(),
                new[] { "EntityId", "entityId", "entity_id", "entityid", "campusId", "CampusId" });
        }

        private static int? TryReadInt(ControllerBase controller, params string[] names)
        {
            return TryReadInt(controller, names.Take(2), names.Skip(2).Take(2), names.Skip(4));
        }

        private static int? TryReadInt(
            ControllerBase controller,
            IEnumerable<string> headerNames,
            IEnumerable<string> queryNames,
            IEnumerable<string> claimNames)
        {
            foreach (var name in headerNames)
            {
                if (controller.Request.Headers.TryGetValue(name, out var headerValue)
                    && int.TryParse(headerValue.ToString(), out var parsedHeaderValue)
                    && parsedHeaderValue > 0)
                {
                    return parsedHeaderValue;
                }
            }

            foreach (var name in queryNames)
            {
                if (controller.Request.Query.TryGetValue(name, out var queryValue)
                    && int.TryParse(queryValue.ToString(), out var parsedQueryValue)
                    && parsedQueryValue > 0)
                {
                    return parsedQueryValue;
                }
            }

            foreach (var name in claimNames)
            {
                var claimValue = controller.User.Claims.FirstOrDefault(c => c.Type == name)?.Value;
                if (int.TryParse(claimValue, out var parsedClaimValue) && parsedClaimValue > 0)
                {
                    return parsedClaimValue;
                }
            }

            return null;
        }
    }
}
