using System.ComponentModel.DataAnnotations;

namespace HumanResource.Service.Entities;

public sealed class EmployeeGradeLegacy
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public int EntityId { get; set; }
    [MaxLength(30)] public string Code { get; set; } = string.Empty;
    public decimal MinimumSalary { get; set; }
    public decimal MaximumSalary { get; set; }
    public bool UtBlock { get; set; }
    public DateTime? UtAddTime { get; set; }
    [MaxLength(100)] public string? UtAddUser { get; set; }
    public DateTime? UtEditTime { get; set; }
    [MaxLength(100)] public string? UtEditUser { get; set; }
    public int UtRecordVersion { get; set; }
}
