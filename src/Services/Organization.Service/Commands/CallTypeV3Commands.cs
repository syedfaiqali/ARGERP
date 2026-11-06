using MediatR;
using Organization.Service.DTOs;

namespace Organization.Service.Commands
{
    public class CreateCallTypeV3Request
    {
        public string? Code { get; set; }
        public string? ShortName { get; set; }
        public string? Description { get; set; }
    }

    public class UpdateCallTypeV3Request
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? ShortName { get; set; }
        public string? Description { get; set; }
    }

    public record CreateCallTypeV3Command(CreateCallTypeV3Request Request) : IRequest<CallTypeV3Dto>;
    public record UpdateCallTypeV3Command(UpdateCallTypeV3Request Request) : IRequest<bool>;
    public record DeleteCallTypeV3Command(int Id) : IRequest<bool>;
}
