using MediatR;
using HumanResource.Service.DTOs;

namespace HumanResource.Service.Commands
{
    public sealed record CreateHolidayTypeV3Command(CreateHolidayTypeV3Request Request, int ClientId, int? EntityId) : IRequest<HolidayTypeV3Dto>;
    public sealed record UpdateHolidayTypeV3Command(UpdateHolidayTypeV3Request Request, int ClientId, int? EntityId) : IRequest<bool>;
    public sealed record DeleteHolidayTypeV3Command(int Id, int ClientId, int? EntityId) : IRequest<bool>;
}
