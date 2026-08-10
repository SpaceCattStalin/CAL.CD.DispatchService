using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }
    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditableEntities();
        return _context.SaveChangesAsync(cancellationToken);
    }
    /// <summary>
    /// Auto update the CreatedAt and UpdatedAt property of the affected Entity
    /// </summary>
    private void UpdateAuditableEntities()
    {
        IEnumerable<EntityEntry<BaseEntity>> entries = _context.ChangeTracker.Entries<BaseEntity>();

        foreach (var entityEntry in entries)
        {
            if (entityEntry.State == EntityState.Added)
            {
                entityEntry.Property(a => a.CreatedAt).CurrentValue = DateTime.UtcNow;
            }

            if (entityEntry.State == EntityState.Modified)
            {
                entityEntry.Property(a => a.UpdatedAt).CurrentValue = DateTime.UtcNow;
            }
        }
    }
}
