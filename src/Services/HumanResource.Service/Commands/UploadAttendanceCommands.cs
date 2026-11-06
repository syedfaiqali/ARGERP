using HumanResource.Service.DTOs;
using MediatR;

namespace HumanResource.Service.Commands
{
    public sealed record ProcessUploadAttendanceV3Command(
        UploadAttendanceProcessV3Request Request,
        int ClientId,
        int? EntityId
    ) : IRequest<UploadAttendanceProcessResultV3Dto>;
}
