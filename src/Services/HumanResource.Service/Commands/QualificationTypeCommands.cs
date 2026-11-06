using MediatR;
using HumanResource.Service.DTOs;

namespace HumanResource.Service.Commands
{
    public sealed record CreateQualificationTypeV3Command(CreateQualificationTypeV3Request Request, int ClientId, int? EntityId) : IRequest<QualificationTypeV3Dto>;
    public sealed record UpdateQualificationTypeV3Command(UpdateQualificationTypeV3Request Request, int ClientId, int? EntityId) : IRequest<bool>;
    public sealed record DeleteQualificationTypeV3Command(int Id, int ClientId, int? EntityId) : IRequest<bool>;
}
