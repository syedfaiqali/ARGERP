namespace EDXS.Shared.Models;

public abstract class AuditableEntity
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public int? ClientId { get; set; }
	public int? EntityId { get; set; }
	public DateTime? UtAddTime { get; set; }
	public string? UtAddUser { get; set; }
	public DateTime? UtEditTime { get; set; }
	public string? UtEditUser { get; set; }
	public bool UtBlock { get; set; }
}
