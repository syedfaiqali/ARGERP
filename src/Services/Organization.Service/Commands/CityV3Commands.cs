using MediatR;
using Organization.Service.DTOs;

namespace Organization.Service.Commands
{
    public sealed class CreateCityV3Command : IRequest<CityV3Dto>
    {
        public string CityCode { get; set; }
        public string CityName { get; set; }
        public string DialCode { get; set; }
        public int StateId { get; set; }
        public int CountryId { get; set; }
        public bool IsActive { get; set; } = true;
        public int CreatedBy { get; set; }
        public int? ClientId { get; set; }
    }

    public sealed class UpdateCityV3Command : IRequest<CityV3Dto>
    {
        public int CityId { get; set; }
        public string CityCode { get; set; }
        public string CityName { get; set; }
        public string DialCode { get; set; }
        public int StateId { get; set; }
        public int CountryId { get; set; }
        public bool IsActive { get; set; }
        public int UpdatedBy { get; set; }
        public int? ClientId { get; set; }
    }

    public sealed class DeleteCityV3Command : IRequest<bool>
    {
        public int CityId { get; set; }
        public int DeletedBy { get; set; }
        public int? ClientId { get; set; }
    }
}
