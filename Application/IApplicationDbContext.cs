using Domain;
using Microsoft.EntityFrameworkCore;

namespace Application;

public interface IApplicationDbContext
{
    DbSet<Dispatch> Dispatches { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
