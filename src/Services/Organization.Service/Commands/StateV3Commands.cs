using MediatR;
using Organization.Service.DTOs;

namespace Organization.Service.Commands
{
    public sealed class CreateStateV3Command : IRequest<StateV3Dto>
    {
        public int CountryId { get; set; }
        public string StateName { get; set; }
        public string StateCode { get; set; }
        public bool IsActive { get; set; } = true;
        public int CreatedBy { get; set; }
        public int? ClientId { get; set; }
    }

    public sealed class UpdateStateV3Command : IRequest<StateV3Dto>
    {
        public int StateId { get; set; }
        public int CountryId { get; set; }
        public string StateName { get; set; }
        public string StateCode { get; set; }
        public bool IsActive { get; set; }
        public int UpdatedBy { get; set; }
        public int? ClientId { get; set; }
    }

    public sealed class DeleteStateV3Command : IRequest<bool>
    {
        public int StateId { get; set; }
        public int DeletedBy { get; set; }
        public int? ClientId { get; set; }
    }
}
