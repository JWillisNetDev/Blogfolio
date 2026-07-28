namespace Blogfolio.Data;

using Blogfolio.Data.Identity;
using Blogfolio.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public class BlogfolioDbContext(DbContextOptions<BlogfolioDbContext> options)
    : IdentityDbContext<BlogfolioUser>(options)
{
    private bool IsSqliteMode => Database.ProviderName == "Microsoft.EntityFrameworkCore.Sqlite";
    public DbSet<TodoItem> TodoItems => Set<TodoItem>();

    public override int SaveChanges()
    {
        AuditChangeTracker();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AuditChangeTracker();
        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Rename alls the base Asp.Net Identity tables
        builder.Entity<IdentityRole>().ToTable("Roles");
        builder.Entity<BlogfolioUser>().ToTable("Users");
        builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
        builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
        builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
        builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");
        builder.Entity<IdentityRoleClaim<string>>().ToTable("Roleclaims");

        // Set up converts to shim Sqlite for local dev and testing
        if (IsSqliteMode)
        {
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                var properties = entityType.GetProperties()
                    .Where(p => p.ClrType == typeof(DateTimeOffset) || p.ClrType == typeof(DateTimeOffset?));
                
                foreach (var property in properties)
                {
                    property.SetValueConverter(new DateTimeOffsetToStringConverter());
                }
            }
        }
    }

    // Apply timestamps to currently buffered entities in change tracking
    private void AuditChangeTracker()
    {
        var modifiedEntries = ChangeTracker.Entries()
            .Where(e => (e.State == EntityState.Modified || e.State == EntityState.Added) && e.Entity is IAuditable);
        foreach (var entry in modifiedEntries)
        {
            if (entry.Entity is IAuditable auditable)
            {
                if (entry.State == EntityState.Added)
                {
                    auditable.CreatedAt = DateTimeOffset.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    auditable.LastUpdatedAt = DateTimeOffset.UtcNow;
                }
            }
        }
    }
}