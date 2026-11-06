using System.ComponentModel.DataAnnotations;

namespace HumanResource.Service.Entities
{
    public class EmployeeProfileLegacy
    {
        public int Id { get; set; }
        public int? ClientId { get; set; }
        public int? PreviousEntityId { get; set; }
        public int? EntityId { get; set; }
        [MaxLength(50)]
        public string? EmployeeCode { get; set; }
        public int? EmployeeCategoryId { get; set; }
        public int? EmployeeTypeId { get; set; }
        [MaxLength(150)]
        public string? FirstName { get; set; }
        [MaxLength(150)]
        public string? LastName { get; set; }
        [MaxLength(150)]
        public string? ParentName { get; set; }
        [MaxLength(20)]
        public string? CellNo { get; set; }
        public int? DepartmentId { get; set; }
        public int? DesignationId { get; set; }
        public int? ReportingToEmployeeId { get; set; }
        public int? SubstituteEmployeeId { get; set; }
        public int? StatusId { get; set; }
        public int? GradeId { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int? GenderId { get; set; }
        public int? BloodGroupId { get; set; }
        public int? MaritalStatusId { get; set; }
        public int? BirthCountryId { get; set; }
        public int? BirthCityId { get; set; }
        public int? NationalityId { get; set; }
        public int? ReligionId { get; set; }
        public DateTime? JoiningDate { get; set; }
        public DateTime? ResignDate { get; set; }
        public long? ImageId { get; set; }
        [MaxLength(50)]
        public string? EmailId { get; set; }
        public int? PositionTypeId { get; set; }
        public int? ShiftId { get; set; }
        [MaxLength(50)]
        public string? MachineCode { get; set; }
        public int? CourseCategoryId { get; set; }
        [MaxLength(500)]
        public string? ZoomUsername { get; set; }
        [MaxLength(500)]
        public string? ZoomPassword { get; set; }
        [MaxLength(50)]
        public string? ZoomMeetingId { get; set; }
        [MaxLength(50)]
        public string? ZoomMeetingPassword { get; set; }
        public string? ZoomToken { get; set; }
        [MaxLength(500)]
        public string? ZoomApiKey { get; set; }
        [MaxLength(500)]
        public string? ZoomApiSecret { get; set; }
        public string? ZoomSdkKey { get; set; }
        public string? ZoomSdkSecret { get; set; }
        public string? ZoomInvitePassword { get; set; }
        [MaxLength(50)]
        public string? EobiNo { get; set; }
        [MaxLength(50)]
        public string? SessiNo { get; set; }
        [MaxLength(50)]
        public string? CnicNo { get; set; }
        [MaxLength(50)]
        public string? CprNo { get; set; }
        public DateTime? CprExpiry { get; set; }
        [MaxLength(50)]
        public string? PassportNo { get; set; }
        public DateTime? PassportExpiry { get; set; }
        [MaxLength(50)]
        public string? VisaNo { get; set; }
        public DateTime? VisaExpiry { get; set; }
        public int? MoeId { get; set; }
        public DateTime? MoeExpiry { get; set; }
        /// <summary>EREmployeeinfohdr.address (separate from the EREmployeeaddress rows).</summary>
        public string? HeaderAddress { get; set; }
        [MaxLength(50)]
        public string? DrivingLicenseNo { get; set; }
        public DateTime? LicenseExpiry { get; set; }
        [MaxLength(50)]
        public string? EmergencyNo { get; set; }
        public int? VehicleId { get; set; }
        public int? HomeStopId { get; set; }
        public DateTime? TValidFrom { get; set; }
        public DateTime? TValidTill { get; set; }
        public bool? TActive { get; set; }
        public bool? UtBlock { get; set; }
        public DateTime? UtEditTime { get; set; }
        public int UtRecordVersion { get; set; }

        public virtual OrgDepartmentLegacy? Department { get; set; }
        public virtual OrgDesignationLegacy? Designation { get; set; }
    }

    public class EmployeeSubCoordinateLegacy
    {
        public int Id { get; set; }
        public int? EmployeeId { get; set; }
        public int? SubCampusId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? TillDate { get; set; }
        [MaxLength(500)]
        public string? Remarks { get; set; }
        public bool UtSysGen { get; set; }
        public bool UtIsPartial { get; set; }
        public DateTime? UtAddTime { get; set; }
        [MaxLength(25)]
        public string? UtAddUser { get; set; }
        public DateTime? UtEditTime { get; set; }
        [MaxLength(25)]
        public string? UtEditUser { get; set; }
        public int UtRecordVersion { get; set; }
        public bool? UtBlock { get; set; }

        public virtual EmployeeProfileLegacy? Employee { get; set; }
        public virtual OrgLegalentityLegacy? SubCampus { get; set; }
    }

    public class EmployeeAddressLegacy
    {
        public int Id { get; set; }
        [MaxLength(10)]
        public string? Code { get; set; }
        public int? EmployeeId { get; set; }
        [MaxLength(50)]
        public string? Ltxt { get; set; }
        public int? AddressTypeId { get; set; }
        public int? CountryId { get; set; }
        public int? StateId { get; set; }
        public int? CityId { get; set; }
        [MaxLength(500)]
        public string? Address { get; set; }
        public bool? PrimaryAddress { get; set; }
        [MaxLength(50)]
        public string? ContactName { get; set; }
        [MaxLength(15)]
        public string? Landline { get; set; }
        [MaxLength(5)]
        public string? Ext { get; set; }
        [MaxLength(15)]
        public string? Mobile { get; set; }
        public bool? UtBlock { get; set; }
        public bool? UtBlock1 { get; set; }
    }

    public class EmployeeRosterLegacy
    {
        public int Id { get; set; }
        public int? ShiftId { get; set; }
        public int? LeaveGroupId { get; set; }
        public bool? LateAllowed { get; set; }
        public bool? EarlyDeparture { get; set; }
        public bool? OvertimeAllowed { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? EmployeeId { get; set; }
        public int? TaxId { get; set; }
        public bool? TaxExempt { get; set; }
        public bool? UtBlock { get; set; }

        public virtual EmployeeProfileLegacy? Employee { get; set; }
        public virtual ShiftLegacy? Shift { get; set; }
        public virtual LeaveGroupLegacy? LeaveGroup { get; set; }
        public virtual IncomeTaxLegacy? Tax { get; set; }
    }

    public class EmployeeEducationLegacy
    {
        public int Id { get; set; }
        public int? StudentId { get; set; }
        public int? EmployeeId { get; set; }
        public int? QualificationTypeId { get; set; }
        public int? QualificationId { get; set; }
        [MaxLength(50)]
        public string? Institute { get; set; }
        public int? Year { get; set; }
        [MaxLength(3)]
        public string? Grade { get; set; }
        public double? Percentage { get; set; }
        public double? Cgpa { get; set; }
        public int? ObtainMarks { get; set; }
        [MaxLength(3)]
        public string? Division { get; set; }
        public bool UtSysGen { get; set; }
        public bool UtIsPartial { get; set; }
        public DateTime? UtAddTime { get; set; }
        [MaxLength(25)]
        public string? UtAddUser { get; set; }
        public DateTime? UtEditTime { get; set; }
        [MaxLength(25)]
        public string? UtEditUser { get; set; }
        public int UtRecordVersion { get; set; }
        public bool UtBlock { get; set; }

        public virtual EmployeeProfileLegacy? Employee { get; set; }
        public virtual ERQualificationtypeLegacy? QualificationType { get; set; }
        public virtual ERQualificationLegacy? Qualification { get; set; }
    }

    public class EmployeeContactInformationLegacy
    {
        public int Id { get; set; }
        [MaxLength(20)]
        public string? Code { get; set; }
        public int? EmployeeId { get; set; }
        public int? RelationTypeId { get; set; }
        public int? GenderId { get; set; }
        [MaxLength(50)]
        public string? FirstName { get; set; }
        [MaxLength(50)]
        public string? LastName { get; set; }
        [MaxLength(500)]
        public string? Address { get; set; }
        [MaxLength(15)]
        public string? PhoneNo { get; set; }
        [MaxLength(15)]
        public string? MobileNo { get; set; }
        [MaxLength(100)]
        public string? Email { get; set; }
        public bool? Emergency { get; set; }
        public bool? NextOfKin { get; set; }
        public bool UtSysGen { get; set; }
        public bool UtBlock1 { get; set; }
        public bool UtIsPartial { get; set; }
        public DateTime? UtAddTime { get; set; }
        [MaxLength(25)]
        public string? UtAddUser { get; set; }
        public DateTime? UtEditTime { get; set; }
        [MaxLength(25)]
        public string? UtEditUser { get; set; }
        public int UtRecordVersion { get; set; }
        public bool UtBlock { get; set; }

        public virtual EmployeeProfileLegacy? Employee { get; set; }
        public virtual RelationTypeLegacy? RelationType { get; set; }
        public virtual UtLovLegacy? Gender { get; set; }
    }

    public class RelationTypeLegacy
    {
        public int Id { get; set; }
        [MaxLength(15)]
        public string? RelationTypeNo { get; set; }
        [MaxLength(150)]
        public string? Stxt { get; set; }
        [MaxLength(500)]
        public string? Ltxt { get; set; }
        public int? ClientId { get; set; }
        public bool? UtBlock { get; set; }
    }

    public class IncomeTaxLegacy
    {
        public int Id { get; set; }
        [MaxLength(10)]
        public string? Code { get; set; }
        [MaxLength(500)]
        public string? Stxt { get; set; }
        public int? ClientId { get; set; }
        public bool UtBlock { get; set; }
    }

    public class EmployeeBankAccountLegacy
    {
        public int Id { get; set; }
        [MaxLength(20)]
        public string? Code { get; set; }
        public int? EmployeeId { get; set; }
        public int? BankGroupId { get; set; }
        [MaxLength(10)]
        public string? BranchCode { get; set; }
        [MaxLength(100)]
        public string? BranchName { get; set; }
        [MaxLength(30)]
        public string? AccountNo { get; set; }
        [MaxLength(30)]
        public string? IbanNo { get; set; }
        [MaxLength(100)]
        public string? Title { get; set; }
        public bool? PrimaryAccount { get; set; }
        public bool UtSysGen { get; set; }
        public bool UtBlock1 { get; set; }
        public bool UtIsPartial { get; set; }
        public DateTime? UtAddTime { get; set; }
        [MaxLength(25)]
        public string? UtAddUser { get; set; }
        public DateTime? UtEditTime { get; set; }
        [MaxLength(25)]
        public string? UtEditUser { get; set; }
        public int UtRecordVersion { get; set; }
        public bool UtBlock { get; set; }

        public virtual EmployeeProfileLegacy? Employee { get; set; }
        public virtual BankGroupLegacy? BankGroup { get; set; }
    }

    public class BankGroupLegacy
    {
        public int Id { get; set; }
        [MaxLength(10)]
        public string? Code { get; set; }
        [MaxLength(150)]
        public string? Stxt { get; set; }
        [MaxLength(500)]
        public string? Ltxt { get; set; }
        public int? EntityId { get; set; }
        public int? ClientId { get; set; }
        public bool UtBlock { get; set; }
    }

    public class EmployeeEmploymentLegacy
    {
        public int Id { get; set; }
        public int? ClientId { get; set; }
        public int? EntityId { get; set; }
        public int? EmployeeId { get; set; }
        public int? EmploymentTypeId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool UtSysGen { get; set; }
        public bool UtIsPartial { get; set; }
        public DateTime? UtAddTime { get; set; }
        [MaxLength(25)]
        public string? UtAddUser { get; set; }
        public DateTime? UtEditTime { get; set; }
        [MaxLength(25)]
        public string? UtEditUser { get; set; }
        public int UtRecordVersion { get; set; }
        public bool UtBlock { get; set; }
    }

    public class ShiftLegacy
    {
        public int Id { get; set; }
        [MaxLength(20)]
        public string? Code { get; set; }
        [MaxLength(50)]
        public string? Stxt { get; set; }
        public int? ShiftTypeId { get; set; }
        public bool? WoMonday { get; set; }
        public bool? WoTuesday { get; set; }
        public bool? WoWednesday { get; set; }
        public bool? WoThuresday { get; set; }
        public bool? WoFriday { get; set; }
        public bool? WoSaturday { get; set; }
        public bool? WoSunday { get; set; }
        public int? OtPolicyId { get; set; }
        public int? LateArrivPolicyId { get; set; }
        public int? EarlyDepPolicyId { get; set; }
        public int? AbsPolicyId { get; set; }
        public int? LovShiftClassId { get; set; }
        public int? LovSharedId { get; set; }
        public int? OtPolicyMnt { get; set; }
        public int? LateArrivPolicyMnt { get; set; }
        public int? EarlyDepPolicyMnt { get; set; }
        public int? EntityId { get; set; }
        public int? ClientId { get; set; }
        public bool? UtBlock { get; set; }
    }

    public class LeaveGroupLegacy
    {
        public int Id { get; set; }
        [MaxLength(20)]
        public string? Code { get; set; }
        [MaxLength(150)]
        public string? Stxt { get; set; }
        public int? AdjustmentPolicyId { get; set; }
        public int? EntityId { get; set; }
        public int? ClientId { get; set; }
        public bool? UtBlock { get; set; }
        public bool UtSysGen { get; set; }
        public bool UtIsPartial { get; set; }
        public int UtRecordVersion { get; set; }
        public virtual ICollection<LeaveGroupDetailLegacy> Details { get; set; } = new List<LeaveGroupDetailLegacy>();
    }

    public class LeaveGroupDetailLegacy
    {
        public int Id { get; set; }
        public int? LeaveGroupId { get; set; }
        public int? LeaveTypeId { get; set; }
        public int? LeaveDays { get; set; }
        public bool? UtBlock { get; set; }
        public bool UtSysGen { get; set; }
        public int? UtDocType { get; set; }
        public int? UtSubDocType { get; set; }
        public int UtRecordVersion { get; set; }
    }

    /// <summary>
    /// Per-employee online-meeting host credentials (SMmeetinghostinfo). Every meeting
    /// integration reads its host details from here: the legacy personal-room fields
    /// plus the Zoom Server-to-Server OAuth pair. Only the shared Client ID lives on
    /// the institution.
    /// </summary>
    public class MeetingHostInfoLegacy
    {
        public int Id { get; set; }
        public int? EmployeeId { get; set; }
        public int? ClientId { get; set; }
        public int? LovTypeId { get; set; }
        /// <summary>Host's Zoom account address. Legacy column: username.</summary>
        [MaxLength(500)]
        public string? ZoomHostEmail { get; set; }
        [MaxLength(500)]
        public string? ZoomPassword { get; set; }
        [MaxLength(50)]
        public string? ZoomMeetingId { get; set; }
        [MaxLength(50)]
        public string? ZoomMeetingPassword { get; set; }
        public string? ZoomInvitePassword { get; set; }
        public string? ZoomToken { get; set; }
        /// <summary>Server-to-Server OAuth Account ID. Legacy column: apiKey.</summary>
        [MaxLength(500)]
        public string? ZoomAccountId { get; set; }
        /// <summary>Server-to-Server OAuth Client Secret. Legacy column: apiSecret.</summary>
        [MaxLength(500)]
        public string? ZoomClientSecret { get; set; }
        public string? ZoomSdkKey { get; set; }
        public string? ZoomSdkSecret { get; set; }
        // SMmeetinghostinfo declares these NOT NULL with no default, so they are
        // non-nullable here and always written on insert.
        public bool UtSysGen { get; set; }
        public bool UtBlock { get; set; }
        public bool UtIsPartial { get; set; }
        public int UtRecordVersion { get; set; } = 1;
    }
}
