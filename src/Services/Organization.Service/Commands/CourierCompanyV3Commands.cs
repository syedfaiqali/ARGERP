using MediatR;
using Organization.Service.DTOs;

namespace Organization.Service.Commands
{
    public class CreateCourierCompanyV3Request
    {
        public string? Code { get; set; }
        public string? ShortName { get; set; }
        public string? Description { get; set; }
    }

    public class UpdateCourierCompanyV3Request
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? ShortName { get; set; }
        public string? Description { get; set; }
    }

    public record CreateCourierCompanyV3Command(CreateCourierCompanyV3Request Request) : IRequest<CourierCompanyV3Dto>;
    public record UpdateCourierCompanyV3Command(UpdateCourierCompanyV3Request Request) : IRequest<bool>;
    public record DeleteCourierCompanyV3Command(int Id) : IRequest<bool>;
}
