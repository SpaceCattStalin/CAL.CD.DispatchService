using System.Security.Claims;
using Application;
using Application.Auth;
using Application.Interfaces;
using Domain;

namespace Presentation.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public bool HasPermission(string permissionName) =>
        _httpContextAccessor.HttpContext?.User.HasClaim(CustomClaimTypes.Permission, permissionName) ?? false;
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId
    {
        get
        {
            var value = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(value, out var userId))
                throw new UnauthorizedAccessException("No authenticated user");

            return userId;
        }
    }

    public Guid CompanyId
    {
        get
        {
            var value = _httpContextAccessor.HttpContext?.User.FindFirstValue(CustomClaimTypes.CompanyId);
            if (!Guid.TryParse(value, out var companyId))
                throw new UnauthorizedAccessException("No authenticated user");

            return companyId;
        }
    }

    public CompanyType CompanyType
    {
        get
        {
            var value = _httpContextAccessor.HttpContext?.User.FindFirstValue(CustomClaimTypes.CompanyType);
            if (!Enum.TryParse(value, out CompanyType type))
                throw new UnauthorizedAccessException("No authenticated user");

            return type;
        }
    }
}
