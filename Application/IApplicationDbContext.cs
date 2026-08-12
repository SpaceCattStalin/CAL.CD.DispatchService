using Domain;
using Microsoft.EntityFrameworkCore;

namespace Application;

public interface IApplicationDbContext
{
    DbSet<Dispatch> Dispatches { get; }
    DbSet<User> Users { get; }
    DbSet<Role> Roles { get; }
    DbSet<Stop> Stops { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
