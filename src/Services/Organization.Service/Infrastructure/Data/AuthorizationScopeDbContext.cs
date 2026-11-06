using Microsoft.EntityFrameworkCore;

namespace Organization.Service.Infrastructure.Data
{
    /// <summary>
    /// Read-only access to the legacy user-to-entity authorization mapping.
    /// Dashboard scope must be resolved on the server, never supplied by the browser.
    /// </summary>
    public sealed class AuthorizationScopeDbContext : DbContext
    {
        public AuthorizationScopeDbContext(DbContextOptions<AuthorizationScopeDbContext> options) : base(options) { }

        public DbSet<UserEntityScope> UserEntityScopes => Set<UserEntityScope>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserEntityScope>(entity =>
            {
                entity.ToTable("utuserent");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("id");
                entity.Property(x => x.UserId).HasColumnName("userid");
                entity.Property(x => x.EntityId).HasColumnName("entityid");
                entity.Property(x => x.UtBlock).HasColumnName("utblock");
            });
        
            // Legacy databases allow NULL in text columns
            // that are declared non-nullable on the entity. EF then strips the `?? ""`
            // from projections and materialisation throws SqlNullValueException.
            // Marking every non-key string optional keeps reads safe; the database
            // still enforces its own NOT NULL constraints on write.
            foreach (var __entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var __property in __entityType.GetProperties())
                {
                    if (__property.ClrType == typeof(string) && !__property.IsKey())
                    {
                        __property.IsNullable = true;
                    }
                }
            }
        }
    }

    public sealed class UserEntityScope
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? EntityId { get; set; }
        public bool UtBlock { get; set; }
    }
}
