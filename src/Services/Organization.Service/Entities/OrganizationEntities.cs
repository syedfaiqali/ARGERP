namespace Organization.Service.Entities
{
    /// <summary>
    /// Represents a country in the system
    /// </summary>
    public class Country
    {
        public int Id { get; set; }
        public string CountryName { get; set; }
        public string CountryCode { get; set; } // ISO 3166-1 alpha-2 (e.g., US, IN)
        public string? CountryCode3 { get; set; } // ISO 3166-1 alpha-3 (e.g., USA, IND)
        public string? DialCode { get; set; } // International dialing code
        public bool UtBlock { get; set; } = false; // Soft delete flag
        //public string CurrencyCode { get; set; } // ISO 4217 (e.g., USD, INR)
        //public string CurrencyName { get; set; }
        //public string CurrencySymbol { get; set; }
        //public bool IsActive { get; set; } = true;
        //public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        //public int CreatedBy { get; set; }
        //public DateTime? UpdatedAt { get; set; }
        //public int? UpdatedBy { get; set; }
        //public bool IsDeleted { get; set; } = false;

        // Navigation properties
        public virtual ICollection<State> States { get; set; } = new List<State>();
    }

    /// <summary>
    /// Represents a state or province within a country
    /// </summary>
    public class State
    {
        public int Id { get; set; }
        public int CountryId { get; set; }
        public string stxt { get; set; }
        public string StateCode { get; set; } // Abbreviation (e.g., CA, NY)
        //public bool IsActive { get; set; } = true;
        //public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        //public int CreatedBy { get; set; }
        //public DateTime? UpdatedAt { get; set; }
        //public int? UpdatedBy { get; set; }
        //public bool IsDeleted { get; set; } = false;

        // Navigation properties
        public virtual Country Country { get; set; }
        public virtual ICollection<City> Cities { get; set; } = new List<City>();
    }

    /// <summary>
    /// Represents a city within a state
    /// </summary>
    public class City
    {
        public int Id { get; set; }
        public int StateId { get; set; }
        public int CountryId { get; set; }
        public string Citystxt { get; set; }
        public string CityCode { get; set; }
        public string DialCode { get; set; }
        public bool UtBlock { get; set; }
        //public bool IsActive { get; set; } = true;
        //public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        //public int CreatedBy { get; set; }
        //public DateTime? UpdatedAt { get; set; }
        //public int? UpdatedBy { get; set; }
        //public bool IsDeleted { get; set; } = false;

        // Navigation properties
        public virtual State State { get; set; }
        public virtual Country Country { get; set; }
        public virtual ICollection<District> Districts { get; set; } = new List<District>();
    }

    /// <summary>
    /// Represents a district within a city
    /// </summary>
    public class District
    {
        public int DistrictId { get; set; }
        public int CityId { get; set; }
        public string DistrictName { get; set; }
        public string DistrictCode { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Navigation properties
        public virtual City City { get; set; }
    }

    /// <summary>
    /// Represents an organizational entity (e.g., main organization)
    /// </summary>
    public class OrgEntity
    {
        public int EntityId { get; set; }
        public int ClientId { get; set; }
        public string EntityCode { get; set; }
        public string EntityName { get; set; }
        public string EntityType { get; set; } // e.g., "Main", "Branch", "Regional"
        public int? TerminologySetId { get; set; }
        public virtual TerminologySet TerminologySet { get; set; }
        public int CountryId { get; set; }
        public int? StateId { get; set; }
        public int? CityId { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string WebsiteUrl { get; set; }
        public string LogoUrl { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Navigation properties
        public virtual Country Country { get; set; }
        public virtual State State { get; set; }
        public virtual City City { get; set; }
        public virtual ICollection<OrgCampus> Campuses { get; set; } = new List<OrgCampus>();
    }

    /// <summary>
    /// Represents a campus within an organization
    /// </summary>
    public class OrgCampus
    {
        public int CampusId { get; set; }
        public int EntityId { get; set; }
        public int ClientId { get; set; }
        public string CampusCode { get; set; }
        public string CampusName { get; set; }
        public string CampusType { get; set; } // e.g., "Main", "Branch", "Regional"
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
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Navigation properties
        public virtual OrgEntity Entity { get; set; }
        public virtual Country Country { get; set; }
        public virtual State State { get; set; }
        public virtual City City { get; set; }
        public virtual ICollection<OrgBuilding> Buildings { get; set; } = new List<OrgBuilding>();
        public virtual ICollection<OrgDepartment> Departments { get; set; } = new List<OrgDepartment>();
    }

    /// <summary>
    /// Represents a building within a campus
    /// </summary>
    public class OrgBuilding
    {
        public int BuildingId { get; set; }
        public int CampusId { get; set; }
        public string BuildingCode { get; set; }
        public string BuildingName { get; set; }
        public int FloorCount { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Navigation properties
        public virtual OrgCampus Campus { get; set; }
        public virtual ICollection<OrgFloor> Floors { get; set; } = new List<OrgFloor>();
    }

    /// <summary>
    /// Represents a floor within a building
    /// </summary>
    public class OrgFloor
    {
        public int FloorId { get; set; }
        public int BuildingId { get; set; }
        public int FloorNumber { get; set; }
        public string FloorName { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Navigation properties
        public virtual OrgBuilding Building { get; set; }
        public virtual ICollection<OrgRoom> Rooms { get; set; } = new List<OrgRoom>();
    }

    /// <summary>
    /// Represents a room within a floor
    /// </summary>
    public class OrgRoom
    {
        public int RoomId { get; set; }
        public int FloorId { get; set; }
        public string RoomNumber { get; set; }
        public string RoomName { get; set; }
        public string RoomType { get; set; } // e.g., "Classroom", "Lab", "Office", "Conference"
        public int? Capacity { get; set; }
        public bool HasProjector { get; set; }
        public bool HasSmartBoard { get; set; }
        public bool HasACUnit { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Navigation properties
        public virtual OrgFloor Floor { get; set; }
    }

    /// <summary>
    /// Represents a department within a campus
    /// </summary>
    public class OrgDepartment
    {
        public int DepartmentId { get; set; }
        public int CampusId { get; set; }
        public int ClientId { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentType { get; set; } // e.g., "Academic", "Administration", "Support"
        public string HeadName { get; set; }
        public string HeadEmail { get; set; }
        public string HeadPhone { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Navigation properties
        public virtual OrgCampus Campus { get; set; }
        public virtual ICollection<OrgDesignation> Designations { get; set; } = new List<OrgDesignation>();
    }

    /// <summary>
    /// Represents a job designation/title
    /// </summary>
    public class OrgDesignation
    {
        public int DesignationId { get; set; }
        public int DepartmentId { get; set; }
        public int ClientId { get; set; }
        public string DesignationCode { get; set; }
        public string DesignationName { get; set; }
        public string DesignationType { get; set; } // e.g., "Staff", "Management", "Support"
        public int? ReportingToDesignationId { get; set; } // For hierarchy
        public string Description { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Navigation properties
        public virtual OrgDepartment Department { get; set; }
        public virtual OrgDesignation ReportingToDesignation { get; set; }
    }

    /// <summary>
    /// Represents a cost center for financial tracking
    /// </summary>
    public class OrgCostCenter
    {
        public int CostCenterId { get; set; }
        public int ClientId { get; set; }
        public string CostCenterCode { get; set; }
        public string CostCenterName { get; set; }
        public string Description { get; set; }
        public decimal? BudgetAmount { get; set; }
        public string ManagerName { get; set; }
        public string ManagerEmail { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; } = false;
    }

    /// <summary>
    /// Represents a business unit
    /// </summary>
    public class OrgBusinessUnit
    {
        public int BusinessUnitId { get; set; }
        public int ClientId { get; set; }
        public string BusinessUnitCode { get; set; }
        public string BusinessUnitName { get; set; }
        public string BusinessUnitType { get; set; } // e.g., "Academic", "Administrative", "Support"
        public string Description { get; set; }
        public string HeadName { get; set; }
        public string HeadEmail { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; } = false;
    }

    /// <summary>
    /// Represents a geographical region
    /// </summary>
    public class OrgRegion
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int? CountryId { get; set; }
        public string Code { get; set; }
        public string stxt { get; set; }
        public bool UtSysGen { get; set; } = false;
        public bool UtBlock { get; set; } = false;
        public bool UtIsPartial { get; set; } = false;
        public int UtRecordVersion { get; set; } = 1;
        //public string Description { get; set; }
        //public bool IsActive { get; set; } = true;
        //public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        //public int CreatedBy { get; set; }
        //public DateTime? UpdatedAt { get; set; }
        //public int? UpdatedBy { get; set; }
        //public bool IsDeleted { get; set; } = false;

        // Navigation properties
        public virtual Country Country { get; set; }
    }

    /// <summary>
    /// Represents supported languages in the system
    /// </summary>
    public class OrgLanguage
    {
        public int LanguageId { get; set; }
        public string LanguageName { get; set; }
        public string LanguageCode { get; set; } // ISO 639-1 (e.g., en, hi, es)
        public bool IsDefault { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; } = false;
    }

    /// <summary>
    /// Represents a caste/community (for statistical/demographic purposes)
    /// </summary>
    public class OrgCaste
    {
        public int CasteId { get; set; }
        public string CasteName { get; set; }
        public string CasteCode { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; } = false;
    }

    /// <summary>
    /// Represents an occupation/profession
    /// </summary>
    public class Occupation
    {
        public int OccupationId { get; set; }
        public string OccupationName { get; set; }
        public string OccupationCode { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; } = false;
    }

    /// <summary>
    /// Represents a base set of terminology (e.g., School, College)
    /// </summary>
    public class TerminologySet
    {
        public int Id { get; set; }
        public string SetName { get; set; } // e.g., "K-12 School", "Higher Education"
        public string ConfigJson { get; set; } // JSON blob of base key-value pairs
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Represents organization-specific terminology overrides
    /// </summary>
    public class TenantTerminology
    {
        public int Id { get; set; }
        public int EntityId { get; set; }
        public string FromTerminology { get; set; } // The base key/term
        public string ToTerminology { get; set; } // The customized override
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public virtual OrgEntity Entity { get; set; }
    }
}
