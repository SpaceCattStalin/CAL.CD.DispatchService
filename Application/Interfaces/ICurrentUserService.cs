namespace Application.Interfaces;

public interface ICurrentUserService
{
    public Guid UserId { get; }
    public Guid CompanyId { get; }
    bool HasPermission(string permissionName);
}
