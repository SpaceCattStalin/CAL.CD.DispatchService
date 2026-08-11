namespace Domain;

public class Permission : BaseEntity
{
    public Guid PermissionId { get; init; }
    public string Name { get; private set; }
    public ICollection<RolePermission> RolePermissions { get; private set; } = new List<RolePermission>();

    /// <summary>
    /// Factory method to create a Permission.
    /// </summary>
    /// <param name="name">Name of the permission in "resource:action" form, e.g. "dispatches:create"</param>
    /// <returns>A new Permission instance</returns>
    /// <exception cref="ArgumentException">name is empty</exception>
    public static Permission Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required", nameof(name));

        Permission permission = new()
        {
            PermissionId = Guid.NewGuid(),
            Name = name,
            CreatedAt = DateTime.UtcNow
        };

        return permission;
    }
}
