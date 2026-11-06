using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Organization.Service.Entities
{
    [Table("SMStudentinfo")]
    public sealed class CampusStudentInfoLegacy
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("clientid")]
        public int? ClientId { get; set; }

        [Column("entityid")]
        public int? EntityId { get; set; }

        [Column("regdate")]
        public DateTime? RegDate { get; set; }

        [Column("lovstatusid")]
        public int? StatusId { get; set; }

        [Column("genderid")]
        public int? GenderId { get; set; }

        [Column("curcourseid")]
        public int? CurrentCourseId { get; set; }

        [Column("appid")]
        public int? AppId { get; set; }
    }

    [Table("SMStudentwithdrawal")]
    public sealed class CampusStudentWithdrawalLegacy
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("studentid")]
        public int? StudentId { get; set; }

        [Column("entityid")]
        public int? EntityId { get; set; }

        [Column("leavingdate")]
        public DateTime? LeavingDate { get; set; }

        [Column("lovstatusid")]
        public int? StatusId { get; set; }
    }

    [Table("SMGuardian")]
    public sealed class CampusGuardianLegacy
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("clientid")]
        public int? ClientId { get; set; }

        [Column("entityid")]
        public int? EntityId { get; set; }
    }

    [Table("SMStudentguardian")]
    public sealed class CampusStudentGuardianLegacy
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("studentid")]
        public int? StudentId { get; set; }

        [Column("guardianid")]
        public int? GuardianId { get; set; }
    }

    [Table("FOAdmEnquiry")]
    public sealed class CampusAdmissionEnquiryLegacy
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("clientid")]
        public int? ClientId { get; set; }

        [Column("entityid")]
        public int? EntityId { get; set; }
    }

    [Table("FOAdmEnquirydtl")]
    public sealed class CampusAdmissionEnquiryDetailLegacy
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("enquiryid")]
        public int? EnquiryId { get; set; }
    }

    [Table("SMAdmApplication")]
    public sealed class CampusAdmissionApplicationLegacy
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("enquirydtlid")]
        public int? EnquiryDetailId { get; set; }

        [Column("appdate")]
        public DateTime? AppDate { get; set; }
    }

    [Table("SMCourse")]
    public sealed class CampusCourseLegacy
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("stxt")]
        public string? Name { get; set; }

        [Column("indx")]
        public int? Index { get; set; }
    }

    [Table("SMCourseextend")]
    public sealed class CampusCourseExtendLegacy
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("entityid")]
        public int? EntityId { get; set; }

        [Column("courseid")]
        public int? CourseId { get; set; }
    }
}
