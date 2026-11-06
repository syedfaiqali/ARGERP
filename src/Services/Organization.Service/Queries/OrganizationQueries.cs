using MediatR;
using Organization.Service.DTOs;

namespace Organization.Service.Queries
{
    // ==================== Country Queries ====================
    public class GetAllCountriesQuery : IRequest<PaginationResponse<CountryDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SearchTerm { get; set; }
        public bool IncludeInactive { get; set; } = false;
    }

    public class GetCountryByIdQuery : IRequest<CountryDto>
    {
        public int CountryId { get; set; }
    }

    public class GetCountriesByCodeQuery : IRequest<IEnumerable<CountryDto>>
    {
        public string CountryCode { get; set; }
    }

    // ==================== State Queries ====================
    public class GetStatesByCountryQuery : IRequest<PaginationResponse<StateDto>>
    {
        public int CountryId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 100;
        public bool IncludeInactive { get; set; } = false;
    }

    public class GetAllStatesQuery : IRequest<PaginationResponse<StateDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SearchTerm { get; set; }
        public bool IncludeInactive { get; set; } = false;
    }

    public class GetStateByIdQuery : IRequest<StateDto>
    {
        public int StateId { get; set; }
    }

    // ==================== City Queries ====================
    public class GetCitiesByStateQuery : IRequest<PaginationResponse<CityDto>>
    {
        public int StateId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 100;
        public bool IncludeInactive { get; set; } = false;
    }

    public class GetCitiesByCountryQuery : IRequest<PaginationResponse<CityDto>>
    {
        public int CountryId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 100;
        public bool IncludeInactive { get; set; } = false;
    }

    public class GetAllCitiesQuery : IRequest<PaginationResponse<CityDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SearchTerm { get; set; }
        public bool IncludeInactive { get; set; } = false;
    }

    public class GetCityByIdQuery : IRequest<CityDto>
    {
        public int CityId { get; set; }
    }

    // ==================== District Queries ====================
    public class GetDistrictsByCityQuery : IRequest<PaginationResponse<DistrictDto>>
    {
        public int CityId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 100;
        public bool IncludeInactive { get; set; } = false;
    }

    public class GetAllDistrictsQuery : IRequest<PaginationResponse<DistrictDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SearchTerm { get; set; }
        public bool IncludeInactive { get; set; } = false;
    }

    public class GetDistrictByIdQuery : IRequest<DistrictDto>
    {
        public int DistrictId { get; set; }
    }

    // ==================== OrgCampus Queries ====================
    public class GetAllOrgCampusesQuery : IRequest<PaginationResponse<OrgCampusDto>>
    {
        public int? ClientId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SearchTerm { get; set; }
        public bool IncludeInactive { get; set; } = false;
    }

    public class GetOrgCampusesByEntityQuery : IRequest<PaginationResponse<OrgCampusDto>>
    {
        public int EntityId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 100;
        public bool IncludeInactive { get; set; } = false;
    }

    public class GetOrgCampusesByClientQuery : IRequest<PaginationResponse<OrgCampusDto>>
    {
        public int ClientId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 100;
        public bool IncludeInactive { get; set; } = false;
    }

    public class GetOrgCampusByIdQuery : IRequest<OrgCampusDto>
    {
        public int CampusId { get; set; }
    }

    // ==================== OrgDepartment Queries ====================
    public class GetAllOrgDepartmentsQuery : IRequest<PaginationResponse<OrgDepartmentDto>>
    {
        public int? ClientId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SearchTerm { get; set; }
        public bool IncludeInactive { get; set; } = false;
    }

    public class GetOrgDepartmentsByCampusQuery : IRequest<PaginationResponse<OrgDepartmentDto>>
    {
        public int CampusId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 100;
        public bool IncludeInactive { get; set; } = false;
    }

    public class GetOrgDepartmentByIdQuery : IRequest<OrgDepartmentDto>
    {
        public int DepartmentId { get; set; }
    }

    // ==================== OrgDesignation Queries ====================
    public class GetAllOrgDesignationsQuery : IRequest<PaginationResponse<OrgDesignationDto>>
    {
        public int? ClientId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SearchTerm { get; set; }
        public bool IncludeInactive { get; set; } = false;
    }

    public class GetOrgDesignationsByDepartmentQuery : IRequest<PaginationResponse<OrgDesignationDto>>
    {
        public int DepartmentId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 100;
        public bool IncludeInactive { get; set; } = false;
    }

    public class GetOrgDesignationByIdQuery : IRequest<OrgDesignationDto>
    {
        public int DesignationId { get; set; }
    }

    // ==================== Reference Data Queries ====================
    public class GetAllLanguagesQuery : IRequest<IEnumerable<OrgLanguageDto>>
    {
        public bool IncludeInactive { get; set; } = false;
    }

    public class GetAllCastesQuery : IRequest<IEnumerable<OrgCasteDto>>
    {
        public bool IncludeInactive { get; set; } = false;
    }

    public class GetAllOccupationsQuery : IRequest<IEnumerable<OccupationDto>>
    {
        public bool IncludeInactive { get; set; } = false;
    }
}
