using MediatR;
using Organization.Service.DTOs;

namespace Organization.Service.Queries
{
    public sealed class GetAdditionalRegistrationsV3Query : IRequest<List<AdditionalRegistrationV3Dto>>
    {
        public int ClientId { get; set; }
        public int EntityId { get; set; }
        public int InfoForId { get; set; }
    }

    public sealed class GetAdditionalRegistrationByIdV3Query : IRequest<AdditionalRegistrationV3Dto>
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int EntityId { get; set; }
    }

    public sealed class GetAdditionalRegistrationInfoForOptionsV3Query : IRequest<List<AdditionalRegistrationInfoForOptionV3Dto>>
    {
    }
}
