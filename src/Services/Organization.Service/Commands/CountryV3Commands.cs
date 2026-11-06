using MediatR;
using Organization.Service.DTOs;

namespace Organization.Service.Commands
{
    public class CreateCountryV3Command : IRequest<CountryV3Dto>
    {
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string IsoCode { get; set; }
        public string DialCode { get; set; }
        public int CreatedBy { get; set; }
        public int? ClientId { get; set; }
    }

    public class UpdateCountryV3Command : IRequest<CountryV3Dto>
    {
        public int CountryId { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string IsoCode { get; set; }
        public string DialCode { get; set; }
        public int UpdatedBy { get; set; }
        public int? ClientId { get; set; }
    }

    public class DeleteCountryV3Command : IRequest<bool>
    {
        public int CountryId { get; set; }
        public int DeletedBy { get; set; }
        public int? ClientId { get; set; }
    }
}
