using HumanResource.Service.DTOs;
using MediatR;
using System.Collections.Generic;

namespace HumanResource.Service.Queries
{
    public record GetRatingModelsV3Query(int PageNumber, int PageSize, int ClientId, int? EntityId, string? SearchTerm) : IRequest<PaginationResponse<RatingModelV3Dto>>;
    public record GetRatingModelByIdV3Query(int Id, int ClientId, int? EntityId) : IRequest<RatingModelV3Dto?>;
}
