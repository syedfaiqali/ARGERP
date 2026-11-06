using EDXS.Shared.Common;
using EDXS.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace EDXS.Shared.Infrastructure;

/// <summary>
/// Tenant-scoped repository that automatically filters by ClientId and EntityId
/// </summary>
/// <typeparam name="T">Entity type that inherits from AuditableEntity</typeparam>
public class TenantScopedRepository<T> : GenericRepository<T> where T : AuditableEntity
{
	private readonly TenantContext _tenantContext;

	public TenantScopedRepository(BaseDbContext context, TenantContext tenantContext) 
		: base(context)
	{
		_tenantContext = tenantContext ?? throw new ArgumentNullException(nameof(tenantContext));
		
		if (!_tenantContext.IsValid())
		{
			throw new InvalidOperationException("TenantContext must have a valid ClientId");
		}
	}

	/// <summary>
	/// Gets a queryable with automatic tenant filtering
	/// </summary>
	protected IQueryable<T> TenantQuery
	{
		get
		{
			var query = DbSet.AsQueryable().Where(x => x.ClientId == _tenantContext.ClientId);

			// Apply entity scope if specified (precedence rule: tenant-level OR entity-specific OR entity-null shared config)
			if (_tenantContext.HasEntityScope())
			{
				query = query.Where(x => x.EntityId == _tenantContext.EntityId || x.EntityId == null);
			}

			return query;
		}
	}

	public override async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
	{
		return await TenantQuery.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
	}

	public override async Task<IReadOnlyList<T>> ListAsync(int skip = 0, int take = 100, CancellationToken cancellationToken = default)
	{
		return await TenantQuery
			.Skip(skip)
			.Take(take)
			.ToListAsync(cancellationToken);
	}

	public override async Task AddAsync(T entity, CancellationToken cancellationToken = default)
	{
		// Auto-populate tenant context
		entity.ClientId = _tenantContext.ClientId;
		
		// Only set EntityId if entity doesn't already have one and context has entity scope
		if (entity.EntityId == null && _tenantContext.HasEntityScope())
		{
			entity.EntityId = _tenantContext.EntityId;
		}

		await base.AddAsync(entity, cancellationToken);
	}

	/// <summary>
	/// Gets all records for the current tenant (ignores entity scope)
	/// Use for tenant-wide admin operations
	/// </summary>
	public async Task<IReadOnlyList<T>> ListAllForTenantAsync(int skip = 0, int take = 100, CancellationToken cancellationToken = default)
	{
		return await DbSet.AsQueryable()
			.Where(x => x.ClientId == _tenantContext.ClientId)
			.Skip(skip)
			.Take(take)
			.ToListAsync(cancellationToken);
	}

	/// <summary>
	/// Gets records by entity-specific scope (null entityId returns shared tenant-level config)
	/// </summary>
	public async Task<IReadOnlyList<T>> ListByEntityAsync(int? entityId, int skip = 0, int take = 100, CancellationToken cancellationToken = default)
	{
		var query = DbSet.AsQueryable()
			.Where(x => x.ClientId == _tenantContext.ClientId);

		if (entityId.HasValue)
		{
			// Get entity-specific OR shared (null entityId) records
			query = query.Where(x => x.EntityId == entityId || x.EntityId == null);
		}
		else
		{
			// Get only shared tenant-level records
			query = query.Where(x => x.EntityId == null);
		}

		return await query
			.Skip(skip)
			.Take(take)
			.ToListAsync(cancellationToken);
	}
}
