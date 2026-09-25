using Domain;
using Infrastructure;

namespace Application.UnitTests.TestHelpers;

public static class DbSeedExtensions
{
    /// <summary>
    /// Adds a user with the given id, company and role, filling the required fields with placeholders.
    /// </summary>
    public static async Task<User> SeedUserAsync(this ApplicationDbContext db, Guid userId, Guid companyId,
        UserRole userRole, bool isActive = true)
    {
        var user = new User { UserId = userId, CompanyId = companyId, UserRole = userRole };
        var entry = db.Users.Add(user);

        // Required fields have private setters, so set them through the change tracker
        entry.Property(u => u.FirstName).CurrentValue = "Tester";
        entry.Property(u => u.LastName).CurrentValue = "Tester";
        entry.Property(u => u.Phone).CurrentValue = "5550000000";
        entry.Property(u => u.Email).CurrentValue = "tester@example.com";
        entry.Property(u => u.UserName).CurrentValue = "tester";
        entry.Property(u => u.PasswordHash).CurrentValue = "hash";
        entry.Property(u => u.IsActive).CurrentValue = isActive;

        await db.SaveChangesAsync();
        return user;
    }

    /// <summary>
    /// Adds 2 companies, one for Shipper, one for Carrier.
    /// </summary>
    public static async Task SeedCompaniesAsync(this ApplicationDbContext db, Guid shipperCompanyId,
        Guid carrierCompanyId)
    {
        AddCompany(db, shipperCompanyId, CompanyType.Shipper);
        AddCompany(db, carrierCompanyId, CompanyType.Carrier);

        await db.SaveChangesAsync();
    }

    /// <summary>
    /// Adds a dispatch between the given companies with a pickup stop, a dropoff stop and one vehicle.
    /// Seed the companies first: Shipper and Carrier are required relationships, so a query that
    /// includes them will not return a dispatch whose companies are missing.
    /// </summary>
    public static async Task<Dispatch> SeedDispatchAsync(this ApplicationDbContext db, Guid shipperCompanyId,
        Guid carrierCompanyId, DispatchStatus status = DispatchStatus.NotSigned)
    {
        var pickupStop = Stop.Create(1,
            "12345 Main Accord Street", "Warehouse A", "John Doe", "555-1234-678", "john@example.com");
        var dropoffStop = Stop.Create(2,
            "45678 Oak Accord Street", "Warehouse B", "Jane Doe", "555-5678-123", "jane@example.com");

        var dispatch = Dispatch.Create(shipperCompanyId, carrierCompanyId, 500m,
            DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(2), "Test dispatch",
            pickupStop, dropoffStop, [("1HGCM82633A123", 2020, "Honda", "Accord", "Blue")]);
        dispatch.UpdateStatus(status);

        db.Dispatches.Add(dispatch);
        await db.SaveChangesAsync();
        return dispatch;
    }

    /// <summary>
    /// Assigns a driver to a dispatch. Seed the dispatch and the driver user first.
    /// </summary>
    public static async Task SeedDispatchDriverAsync(this ApplicationDbContext db, Guid dispatchId, Guid driverId)
    {
        db.DispatchDrivers.Add(new DispatchDriver { DispatchId = dispatchId, DriverId = driverId });
        await db.SaveChangesAsync();
    }

    private static void AddCompany(ApplicationDbContext db, Guid companyId, CompanyType companyType)
    {
        var entry = db.Companies.Add(new Company { CompanyId = companyId, CompanyType = companyType });

        // Required fields have private setters, so set them through the change tracker
        entry.Property(c => c.CompanyName).CurrentValue = $"Test {companyType}";
        entry.Property(c => c.CompanyPhone).CurrentValue = "5550000000";
        entry.Property(c => c.CompanyEmail).CurrentValue = "company@example.com";
    }
}
