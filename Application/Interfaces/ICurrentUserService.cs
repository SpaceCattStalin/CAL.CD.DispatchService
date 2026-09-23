using Domain;

namespace Application.Interfaces;

public interface ICurrentUserService
{
    public Guid UserId { get; }
    public Guid CompanyId { get; }

    public CompanyType CompanyType { get; }
    bool HasPermission(string permissionName);
}
