using EDXS.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace EDXS.Shared.Infrastructure;

public interface IRepository<T> where T : AuditableEntity
{
	Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
	Task<IReadOnlyList<T>> ListAsync(int skip = 0, int take = 100, CancellationToken cancellationToken = default);
	Task AddAsync(T entity, CancellationToken cancellationToken = default);
	void Update(T entity);
	void Remove(T entity);
}

public interface IUnitOfWork
{
	Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

public class GenericRepository<T> : IRepository<T> where T : AuditableEntity
{
	protected readonly BaseDbContext Context;
	protected readonly DbSet<T> DbSet;

	public GenericRepository(BaseDbContext context)
	{
		Context = context;
		DbSet = context.Set<T>();
	}

	public virtual Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
		DbSet.AsQueryable().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

	public virtual async Task<IReadOnlyList<T>> ListAsync(int skip = 0, int take = 100, CancellationToken cancellationToken = default)
	{
		return await DbSet.AsQueryable().Skip(skip).Take(take).ToListAsync(cancellationToken);
	}

	public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
	{
		await DbSet.AddAsync(entity, cancellationToken);
	}

	public void Update(T entity) => DbSet.Update(entity);

	public void Remove(T entity) => DbSet.Remove(entity);
}

public abstract class BaseDbContext(DbContextOptions options) : DbContext(options), IUnitOfWork
{
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		// Apply global query filter for soft-delete to all AuditableEntity types
		foreach (var entityType in modelBuilder.Model.GetEntityTypes())
		{
			if (typeof(AuditableEntity).IsAssignableFrom(entityType.ClrType))
			{
				var parameter = System.Linq.Expressions.Expression.Parameter(entityType.ClrType, "e");
				var property = System.Linq.Expressions.Expression.Property(parameter, nameof(AuditableEntity.UtBlock));
				var nullCheck = System.Linq.Expressions.Expression.Equal(property, System.Linq.Expressions.Expression.Constant(null, typeof(bool?)));
				var falseCheck = System.Linq.Expressions.Expression.Equal(property, System.Linq.Expressions.Expression.Constant(false));
				var condition = System.Linq.Expressions.Expression.OrElse(nullCheck, falseCheck);
				var filter = System.Linq.Expressions.Expression.Lambda(condition, parameter);

				modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
			}
		}
	}

	public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
		{
			switch (entry.State)
			{
				case EntityState.Added:
					entry.Entity.UtAddTime ??= DateTime.UtcNow;
					break;
				case EntityState.Modified:
					entry.Entity.UtEditTime = DateTime.UtcNow;
					break;
				case EntityState.Deleted:
					entry.State = EntityState.Modified;
					entry.Entity.UtBlock = true;
					entry.Entity.UtEditTime = DateTime.UtcNow;
					break;
			}
		}

		return await base.SaveChangesAsync(cancellationToken);
	}
}
