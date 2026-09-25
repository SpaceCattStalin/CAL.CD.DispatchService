using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Application.UnitTests.TestHelpers;
/// <summary>
/// Create an in-memory (RAM) database with tables like the real database 
/// </summary>
public static class InMemoryDbContextFactory
{
    public static ApplicationDbContext Create(string? databaseName = null)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }
}
