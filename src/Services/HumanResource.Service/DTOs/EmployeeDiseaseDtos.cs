namespace HumanResource.Service.DTOs
{
    public sealed record EmployeeDiseaseV3Dto(
        int Id,
        int? EmployeeId,
        int? DiseaseCategoryId,
        string? DiseaseCategory,
        int? DiseaseTypeId,
        string? DiseaseType,
        int? DiseaseId,
        string? Disease,
        string? DiseaseName,
        string? Description);

    public sealed record EmployeeDiseaseLookupV3Dto(
        int Id,
        string? Code,
        string Name,
        int? DiseaseCategoryId = null,
        int? DiseaseTypeId = null);

    public sealed record UpsertEmployeeDiseaseV3Request(
        int? Id,
        int EmployeeId,
        int? DiseaseCategoryId,
        int? DiseaseTypeId,
        int? DiseaseId,
        string? DiseaseName,
        string? Description);
}
