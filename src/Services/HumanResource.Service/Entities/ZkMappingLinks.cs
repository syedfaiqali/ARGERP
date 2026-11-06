namespace HumanResource.Service.Entities;

public sealed class ZkEmployeeLink
{
    public int EmployeeId { get; set; }
    public string SourceKey { get; set; } = "";
    public int? SourceClientId { get; set; }
    public string MachineCode { get; set; } = "";
}
public sealed class ZkDeviceLink
{
    public int MachineId { get; set; }
    public string SourceKey { get; set; } = "";
    public int? SourceClientId { get; set; }
}
