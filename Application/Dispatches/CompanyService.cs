using Application.Interfaces;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application;

public class CompanyService
{
    private readonly IApplicationDbContext _db;
    private readonly ILogger<CompanyService> _logger;

    public CompanyService(
        IApplicationDbContext db,
        ILogger<CompanyService> logger
    )
    {
        _db = db;
        _logger = logger;
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
}
