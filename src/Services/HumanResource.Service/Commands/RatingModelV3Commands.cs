using HumanResource.Service.DTOs;
using MediatR;

namespace HumanResource.Service.Commands
{
    public record CreateRatingModelV3Command(CreateRatingModelV3Request Request, int ClientId, int? EntityId) : IRequest<RatingModelV3Dto>;
    public record UpdateRatingModelV3Command(UpdateRatingModelV3Request Request, int ClientId, int? EntityId) : IRequest<bool>;
    public record DeleteRatingModelV3Command(int Id, int ClientId, int? EntityId) : IRequest<bool>;
}
