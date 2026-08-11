namespace Infrastructure;

internal static class RbacSeedIds
{
    public static readonly DateTime SeedTimestamp = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static readonly Guid OwnerRoleId = new("10000000-0000-0000-0000-000000000001");
    public static readonly Guid AdminRoleId = new("10000000-0000-0000-0000-000000000002");
    public static readonly Guid DriverRoleId = new("10000000-0000-0000-0000-000000000003");

    public static readonly Guid DispatchesCreatePermissionId = new("20000000-0000-0000-0000-000000000001");
    public static readonly Guid DispatchesReadPermissionId = new("20000000-0000-0000-0000-000000000002");
    public static readonly Guid DispatchesUpdatePermissionId = new("20000000-0000-0000-0000-000000000003");
    public static readonly Guid DispatchesDeletePermissionId = new("20000000-0000-0000-0000-000000000004");
    public static readonly Guid UsersCreatePermissionId = new("20000000-0000-0000-0000-000000000005");
    public static readonly Guid UsersReadPermissionId = new("20000000-0000-0000-0000-000000000006");
    public static readonly Guid UsersUpdatePermissionId = new("20000000-0000-0000-0000-000000000007");
    public static readonly Guid UsersDeletePermissionId = new("20000000-0000-0000-0000-000000000008");
    public static readonly Guid CompaniesReadPermissionId = new("20000000-0000-0000-0000-000000000009");
    public static readonly Guid CompaniesUpdatePermissionId = new("20000000-0000-0000-0000-00000000000a");
}
