using MediatR;
using Organization.Service.DTOs;

namespace Organization.Service.Commands
{
    public class CreateCallMediumV3Request
    {
        public string? Code { get; set; }
        public string? ShortName { get; set; }
        public string? Description { get; set; }
    }

    public class UpdateCallMediumV3Request
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? ShortName { get; set; }
        public string? Description { get; set; }
    }

    public record CreateCallMediumV3Command(CreateCallMediumV3Request Request) : IRequest<CallMediumV3Dto>;
    public record UpdateCallMediumV3Command(UpdateCallMediumV3Request Request) : IRequest<bool>;
    public record DeleteCallMediumV3Command(int Id) : IRequest<bool>;
}
