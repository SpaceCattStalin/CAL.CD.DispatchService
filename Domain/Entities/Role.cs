namespace Domain;

public class Role : BaseEntity
{
    public Guid RoleId { get; init; }
    public string Name { get; private set; }
    public ICollection<RolePermission> RolePermissions { get; private set; } = new List<RolePermission>();

    /// <summary>
    /// Factory method to create a Role.
    /// </summary>
    /// <param name="name">Name of the role, e.g. "Owner", "Admin", "Driver"</param>
    /// <returns>A new Role instance</returns>
    /// <exception cref="ArgumentException">name is empty</exception>
    public static Role Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required", nameof(name));

        Role role = new()
        {
            RoleId = Guid.NewGuid(),
            Name = name,
            CreatedAt = DateTime.UtcNow
        };

        return role;
    }

    /// <summary>
    /// Grants a permission to this role.
    /// </summary>
    /// <param name="permission">The permission to grant</param>
    /// <exception cref="ArgumentNullException">permission is null</exception>
    public void AddPermission(Permission permission)
    {
        ArgumentNullException.ThrowIfNull(permission);

        if (RolePermissions.Any(rp => rp.PermissionId == permission.PermissionId))
            return;

        RolePermissions.Add(new RolePermission
        {
            RoleId = RoleId,
            Role = this,
            PermissionId = permission.PermissionId,
            Permission = permission,
            CreatedAt = DateTime.UtcNow
        });
    }
}
