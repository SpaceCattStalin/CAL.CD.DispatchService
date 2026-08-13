using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Application.UnitTests.TestHelpers;

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
