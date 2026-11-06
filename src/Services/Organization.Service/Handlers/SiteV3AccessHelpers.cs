using Microsoft.EntityFrameworkCore;
using Organization.Service.Infrastructure.Data;

namespace Organization.Service.Handlers
{
    internal static class SiteV3AccessHelpers
    {
        internal static int EnsurePositive(int value, string fieldName)
        {
            if (value <= 0)
                throw new InvalidOperationException($"{fieldName} is required");
            return value;
        }

        internal static string EnsureTrimmed(string? value, string fieldName)
        {
            var trimmed = value?.Trim();
            if (string.IsNullOrWhiteSpace(trimmed))
                throw new InvalidOperationException($"{fieldName} is required");
            return trimmed;
        }

        internal static async Task ValidateLovAsync(
            OrganizationDbContext context,
            int lovId,
            string expectedType,
            string fieldName,
            CancellationToken cancellationToken)
        {
            EnsurePositive(lovId, fieldName);

            var exists = await context.Lovs
                .AsNoTracking()
                .AnyAsync(l =>
                    l.Id == lovId
                    && l.vtype != null
                    && l.vtype.ToUpper() == expectedType.ToUpper(), cancellationToken);

            if (!exists)
                throw new InvalidOperationException($"{fieldName} is invalid");
        }
    }
}
