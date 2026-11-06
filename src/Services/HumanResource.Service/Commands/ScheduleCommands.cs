using HumanResource.Service.DTOs;
using MediatR;

namespace HumanResource.Service.Commands
{
    public sealed record CreateScheduleV3Command(CreateScheduleV3Request Request, int ClientId, int? EntityId)
        : IRequest<ScheduleHeaderV3Dto>;

    public sealed record UpdateScheduleV3Command(UpdateScheduleV3Request Request, int ClientId, int? EntityId)
        : IRequest<bool>;

    public sealed record DeleteScheduleV3Command(int Id, int ClientId, int? EntityId)
        : IRequest<bool>;
}
