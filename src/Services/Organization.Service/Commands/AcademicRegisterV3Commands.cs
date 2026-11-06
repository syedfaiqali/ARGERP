using MediatR;
using Organization.Service.DTOs;

namespace Organization.Service.Commands
{
    public sealed class CreateAcademicRegisterV3Command : IRequest<AcademicRegisterV3Dto>
    {
        public string Code { get; set; } = string.Empty;
        public string RegisterName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int ClientId { get; set; }
        public int CreatedBy { get; set; }
    }

    public sealed class UpdateAcademicRegisterV3Command : IRequest<AcademicRegisterV3Dto>
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string RegisterName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int ClientId { get; set; }
        public int UpdatedBy { get; set; }
    }

    public sealed class DeleteAcademicRegisterV3Command : IRequest<bool>
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int DeletedBy { get; set; }
    }
}
