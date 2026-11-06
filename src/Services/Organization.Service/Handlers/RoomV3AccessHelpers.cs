using Microsoft.EntityFrameworkCore;
using Organization.Service.Infrastructure.Data;

namespace Organization.Service.Handlers
{
    internal static class RoomV3AccessHelpers
    {
        internal static int EnsurePositive(int value, string fieldName)
        {
            if (value <= 0)
                throw new InvalidOperationException($"{fieldName} is required");
            return value;
        }

        internal static string EnsureRequiredText(string? value, string fieldName)
        {
            var trimmed = value?.Trim();
            if (string.IsNullOrWhiteSpace(trimmed))
                throw new InvalidOperationException($"{fieldName} is required");
            return trimmed;
        }

        internal static async Task EnsureRoomTypeAsync(OrganizationDbContext context, int roomTypeId, CancellationToken cancellationToken)
        {
            var exists = await context.Lovs
                .AsNoTracking()
                .AnyAsync(l => l.Id == roomTypeId && l.vtype != null && l.vtype.ToUpper() == "RMTP", cancellationToken);

            if (!exists)
                throw new InvalidOperationException("Room Type is invalid");
        }

        internal static async Task EnsureBuildingAsync(OrganizationDbContext context, int clientId, int entityId, int buildingId, CancellationToken cancellationToken)
        {
            var exists = await context.OrgBuildingLegacy
                .AsNoTracking()
                .AnyAsync(b =>
                    b.Id == buildingId
                    && b.clientid == clientId
                    && b.entityid == entityId
                    && !b.utblock, cancellationToken);

            if (!exists)
                throw new InvalidOperationException("Building is invalid");
        }

        internal static async Task EnsureFloorAsync(OrganizationDbContext context, int clientId, int entityId, int floorId, int buildingId, CancellationToken cancellationToken)
        {
            var exists = await context.OrgFloorLegacy
                .AsNoTracking()
                .AnyAsync(f =>
                    f.Id == floorId
                    && f.buildingid == buildingId
                    && f.clientid == clientId
                    && f.entityid == entityId
                    && !f.utblock, cancellationToken);

            if (!exists)
                throw new InvalidOperationException("Floor is invalid");
        }
    }
}
