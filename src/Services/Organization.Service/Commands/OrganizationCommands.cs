using MediatR;
using Organization.Service.DTOs;

namespace Organization.Service.Commands
{
    // ==================== Country Commands ====================
    public class CreateCountryCommand : IRequest<CountryDto>
    {
        public string CountryName { get; set; }
        public string CountryCode { get; set; }
        public string CountryCode3 { get; set; }
        public string PhoneCode { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencyName { get; set; }
        public string CurrencySymbol { get; set; }
        public int CreatedBy { get; set; }
    }

    public class UpdateCountryCommand : IRequest<CountryDto>
    {
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public string PhoneCode { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencyName { get; set; }
        public string CurrencySymbol { get; set; }
        public bool IsActive { get; set; }
        public int UpdatedBy { get; set; }
    }

    public class DeleteCountryCommand : IRequest<bool>
    {
        public int CountryId { get; set; }
        public int DeletedBy { get; set; }
    }

    // ==================== State Commands ====================
    public class CreateStateCommand : IRequest<StateDto>
    {
        public int CountryId { get; set; }
        public string StateName { get; set; }
        public string StateCode { get; set; }
        public int CreatedBy { get; set; }
    }

    public class UpdateStateCommand : IRequest<StateDto>
    {
        public int StateId { get; set; }
        public string StateName { get; set; }
        public string StateCode { get; set; }
        public bool IsActive { get; set; }
        public int UpdatedBy { get; set; }
    }

    public class DeleteStateCommand : IRequest<bool>
    {
        public int StateId { get; set; }
        public int DeletedBy { get; set; }
    }

    // ==================== City Commands ====================
    public class CreateCityCommand : IRequest<CityDto>
    {
        public int StateId { get; set; }
        public string CityName { get; set; }
        public string CityCode { get; set; }
        public int CreatedBy { get; set; }
    }

    public class UpdateCityCommand : IRequest<CityDto>
    {
        public int CityId { get; set; }
        public string CityName { get; set; }
        public string CityCode { get; set; }
        public bool IsActive { get; set; }
        public int UpdatedBy { get; set; }
    }

    public class DeleteCityCommand : IRequest<bool>
    {
        public int CityId { get; set; }
        public int DeletedBy { get; set; }
    }

    // ==================== OrgCampus Commands ====================
    public class CreateOrgCampusCommand : IRequest<OrgCampusDto>
    {
        public int EntityId { get; set; }
        public int ClientId { get; set; }
        public string CampusCode { get; set; }
        public string CampusName { get; set; }
        public string CampusType { get; set; }
        public int CountryId { get; set; }
        public int? StateId { get; set; }
        public int? CityId { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string PrincipalName { get; set; }
        public string PrincipalEmail { get; set; }
        public string PrincipalPhone { get; set; }
        public DateTime EstablishedDate { get; set; }
        public int CreatedBy { get; set; }
    }

    public class UpdateOrgCampusCommand : IRequest<OrgCampusDto>
    {
        public int CampusId { get; set; }
        public string CampusName { get; set; }
        public string CampusType { get; set; }
        public int? StateId { get; set; }
        public int? CityId { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string PrincipalName { get; set; }
        public string PrincipalEmail { get; set; }
        public string PrincipalPhone { get; set; }
        public bool IsActive { get; set; }
        public int UpdatedBy { get; set; }
    }

    public class DeleteOrgCampusCommand : IRequest<bool>
    {
        public int CampusId { get; set; }
        public int DeletedBy { get; set; }
    }

    // ==================== OrgDepartment Commands ====================
    public class CreateOrgDepartmentCommand : IRequest<OrgDepartmentDto>
    {
        public int CampusId { get; set; }
        public int ClientId { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentType { get; set; }
        public string HeadName { get; set; }
        public string HeadEmail { get; set; }
        public string HeadPhone { get; set; }
        public string Description { get; set; }
        public int CreatedBy { get; set; }
    }

    public class UpdateOrgDepartmentCommand : IRequest<OrgDepartmentDto>
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentType { get; set; }
        public string HeadName { get; set; }
        public string HeadEmail { get; set; }
        public string HeadPhone { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int UpdatedBy { get; set; }
    }

    public class DeleteOrgDepartmentCommand : IRequest<bool>
    {
        public int DepartmentId { get; set; }
        public int DeletedBy { get; set; }
    }

    // ==================== OrgDesignation Commands ====================
    public class CreateOrgDesignationCommand : IRequest<OrgDesignationDto>
    {
        public int DepartmentId { get; set; }
        public int ClientId { get; set; }
        public string DesignationCode { get; set; }
        public string DesignationName { get; set; }
        public string DesignationType { get; set; }
        public int? ReportingToDesignationId { get; set; }
        public string Description { get; set; }
        public int CreatedBy { get; set; }
    }

    public class UpdateOrgDesignationCommand : IRequest<OrgDesignationDto>
    {
        public int DesignationId { get; set; }
        public string DesignationName { get; set; }
        public string DesignationType { get; set; }
        public int? ReportingToDesignationId { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int UpdatedBy { get; set; }
    }

    public class DeleteOrgDesignationCommand : IRequest<bool>
    {
        public int DesignationId { get; set; }
        public int DeletedBy { get; set; }
    }
}
