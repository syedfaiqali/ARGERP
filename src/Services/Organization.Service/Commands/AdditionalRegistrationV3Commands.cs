using MediatR;
using Organization.Service.DTOs;

namespace Organization.Service.Commands
{
    public sealed class CreateAdditionalRegistrationV3Command : IRequest<AdditionalRegistrationV3Dto>
    {
        public int ConfigId { get; set; }
        public string? Caption { get; set; }
        public int InfoForId { get; set; }
        public int? Sequance { get; set; }
        public bool Visible { get; set; }
        public int ClientId { get; set; }
        public int EntityId { get; set; }
        public int CreatedBy { get; set; }
    }

    public sealed class UpdateAdditionalRegistrationV3Command : IRequest<AdditionalRegistrationV3Dto>
    {
        public int Id { get; set; }
        public int ConfigId { get; set; }
        public string? Caption { get; set; }
        public int InfoForId { get; set; }
        public int? Sequance { get; set; }
        public bool Visible { get; set; }
        public int ClientId { get; set; }
        public int EntityId { get; set; }
        public int UpdatedBy { get; set; }
    }

    public sealed class DeleteAdditionalRegistrationV3Command : IRequest<bool>
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int EntityId { get; set; }
        public int DeletedBy { get; set; }
    }
}
