namespace EDXS.Shared.Common;

/// <summary>
/// Represents the current tenant context for multi-tenant operations
/// </summary>
public class TenantContext
{
	/// <summary>
	/// The client (tenant) identifier
	/// </summary>
	public int ClientId { get; set; }

	/// <summary>
	/// The entity (school/campus/branch) identifier. Null means tenant-level access.
	/// </summary>
	public int? EntityId { get; set; }

	/// <summary>
	/// The current user identifier
	/// </summary>
	public int? UserId { get; set; }

	/// <summary>
	/// The current user name
	/// </summary>
	public string? UserName { get; set; }

	/// <summary>
	/// Creates an empty tenant context (for system operations)
	/// </summary>
	public TenantContext()
	{
		ClientId = 0;
	}

	/// <summary>
	/// Creates a tenant context with client and optional entity
	/// </summary>
	public TenantContext(int clientId, int? entityId = null, int? userId = null, string? userName = null)
	{
		ClientId = clientId;
		EntityId = entityId;
		UserId = userId;
		UserName = userName;
	}

	/// <summary>
	/// Validates that the tenant context has required values
	/// </summary>
	public bool IsValid() => ClientId > 0;

	/// <summary>
	/// Checks if the context has entity-level scope
	/// </summary>
	public bool HasEntityScope() => EntityId.HasValue && EntityId.Value > 0;
}
