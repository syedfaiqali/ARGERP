using System.Collections.Generic;

namespace HumanResource.Service.DTOs
{
    public record RatingModelV3Dto(
        int Id,
        string Code,
        string RatingModel,
        bool IsActive,
        List<RatingModelItemV3Dto> Items
    );

    public record RatingModelItemV3Dto(
        int Id,
        string Code,
        string Key,
        string Description,
        double Factor
    );

    public record CreateRatingModelV3Request(
        string Code,
        string RatingModel,
        bool IsActive,
        List<CreateRatingModelItemV3Request> Items
    );

    public record CreateRatingModelItemV3Request(
        string Code,
        string Key,
        string Description,
        double Factor
    );

    public record UpdateRatingModelV3Request(
        int Id,
        string Code,
        string RatingModel,
        bool IsActive,
        List<UpdateRatingModelItemV3Request> Items
    );

    public record UpdateRatingModelItemV3Request(
        int Id, // if 0 or null, consider as new
        string Code,
        string Key,
        string Description,
        double Factor,
        bool Delete // if true, it will be soft-deleted
    );
}
