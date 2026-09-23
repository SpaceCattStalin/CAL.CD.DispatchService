using Domain;
using Microsoft.EntityFrameworkCore;

namespace Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Dispatch> Dispatches { get; }
    DbSet<User> Users { get; }
    DbSet<Role> Roles { get; }
    DbSet<Stop> Stops { get; }
    DbSet<Vehicle> Vehicles { get; }
    DbSet<Company> Companies { get; }
    DbSet<DispatchDriver> DispatchDrivers { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
