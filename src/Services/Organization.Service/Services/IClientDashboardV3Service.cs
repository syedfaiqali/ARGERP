using Organization.Service.DTOs;

namespace Organization.Service.Services
{
    public interface IClientDashboardV3Service
    {
        Task<CampusDashboardDto> GetDashboardAsync(int clientId, int? selectedEntityId, IReadOnlyCollection<int> allowedEntityIds, int duration, CancellationToken cancellationToken = default);
    }
}