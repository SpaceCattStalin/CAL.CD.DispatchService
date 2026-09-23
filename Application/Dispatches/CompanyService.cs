using Application.Dispatches;
using Application.Interfaces;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application;

public class CompanyService
{
    private readonly IApplicationDbContext _db;
    private readonly ILogger<CompanyService> _logger;
    private readonly ICurrentUserService _currentUser;
    public CompanyService(
        IApplicationDbContext db,
        ILogger<CompanyService> logger,
        ICurrentUserService currentUser
    )
    {
        _db = db;
        _logger = logger;
        _currentUser = currentUser;
    }

    public async Task<IEnumerable<CompanyResponse>> GetCarriersAsync()
    {
        return await _db.Companies
            .Where(c => c.CompanyType == CompanyType.Carrier)
            .Select(c => new CompanyResponse(
                c.CompanyId,
                c.CompanyName,
                c.CompanyPhone,
                c.CompanyEmail))
            .ToListAsync();
    }

    public async Task<IEnumerable<DriverResponse>> GetDriversAsync()
    {
        return await _db.Users
            .Where(
                u => u.UserRole == UserRole.Driver
                && u.CompanyId.Equals(_currentUser.CompanyId))
            .Select(u => new DriverResponse(
                u.UserId,
                u.FirstName,
                u.LastName,
                u.Phone,
                u.Email
            ))
            .ToListAsync();
    }
}
