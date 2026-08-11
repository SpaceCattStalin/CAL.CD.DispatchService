namespace Domain;

public class RolePermission : BaseEntity
{
    public Guid RoleId { get; init; }
    public Guid PermissionId { get; init; }
    public Role Role { get; init; }
    public Permission Permission { get; init; }
}
