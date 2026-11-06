namespace Organization.Service.DTOs
{
    public class PublishDocumentV3Dto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public int? SharedWithId { get; set; }
        public string SharedWithName { get; set; }
        public int? DocumentTypeId { get; set; }
        public string DocumentTypeName { get; set; }
        public string PublishDocument { get; set; }
        public DateTime? PublishDate { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTill { get; set; }
        public int? DocumentCategoryId { get; set; }
        public string DocumentCategoryName { get; set; }
        public int? CourseId { get; set; }
        public string CourseName { get; set; }
        public int? SubjectId { get; set; }
        public string SubjectName { get; set; }
        public int? AcademicYearId { get; set; }
        public string AcademicYearName { get; set; }

        public List<PublishDocumentDtlV3Dto> Details { get; set; } = new List<PublishDocumentDtlV3Dto>();
    }

    public class PublishDocumentDtlV3Dto
    {
        public int Id { get; set; }
        public int? PublishId { get; set; }
        public int? ShareTypeId { get; set; }
        public string ShareTypeName { get; set; }
        public int? ClientId { get; set; }
        public string ClientName { get; set; }
        public int? EntityId { get; set; }
        public string EntityName { get; set; }
        public int? RegionId { get; set; }
        public string RegionName { get; set; }
        public int? CampusLevelId { get; set; }
        public string CampusLevelName { get; set; }
        public int? CourseId { get; set; }
        public string CourseName { get; set; }
    }

    public class CreatePublishDocumentV3Request
    {
        public int? SharedWithId { get; set; }
        public int? DocumentTypeId { get; set; }
        public string PublishDocument { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTill { get; set; }
        public int? DocumentCategoryId { get; set; }
        public int? CourseId { get; set; }
        public int? SubjectId { get; set; }
        public int? AcademicYearId { get; set; }
        public List<PublishDocumentDtlV3Dto> Details { get; set; } = new List<PublishDocumentDtlV3Dto>();
    }

    public class UpdatePublishDocumentV3Request : CreatePublishDocumentV3Request
    {
        public int Id { get; set; }
    }

    public class UpdateBulkPublishDocumentV3Request
    {
        public List<int> PublishIds { get; set; }
        public DateTime NewValidTill { get; set; }
    }
}
