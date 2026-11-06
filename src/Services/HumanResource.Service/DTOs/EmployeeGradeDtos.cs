namespace HumanResource.Service.DTOs;

public sealed record EmployeeGradeV3Dto(int Id, string Code, decimal MinimumSalary, decimal MaximumSalary);

public sealed record UpsertEmployeeGradeV3Request(int Id, string? Code, decimal? MinimumSalary, decimal? MaximumSalary);
