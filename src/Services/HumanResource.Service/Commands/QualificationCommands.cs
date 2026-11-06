using MediatR;
using HumanResource.Service.DTOs;

namespace HumanResource.Service.Commands
{
    public sealed record CreateQualificationV3Command(CreateQualificationV3Request Request, int ClientId, int? EntityId) : IRequest<QualificationV3Dto>;
    public sealed record UpdateQualificationV3Command(UpdateQualificationV3Request Request, int ClientId, int? EntityId) : IRequest<bool>;
    public sealed record DeleteQualificationV3Command(int Id, int ClientId, int? EntityId) : IRequest<bool>;
}
