using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain;

namespace Infrastructure;

public class RolePermissionEntityConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("role_permissions");

        builder.HasKey(x => new { x.RoleId, x.PermissionId });

        builder.Property(x => x.RoleId).HasColumnName("role_id");
        builder.Property(x => x.PermissionId).HasColumnName("permission_id");

        builder.Property(x => x.RecordVersion).IsRowVersion();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");

        builder.HasOne(x => x.Role)
            .WithMany(x => x.RolePermissions)
            .HasForeignKey(x => x.RoleId);

        builder.HasOne(x => x.Permission)
            .WithMany(x => x.RolePermissions)
            .HasForeignKey(x => x.PermissionId);

        builder.HasData(
            // Owner: all permissions
            Seed(RbacSeedIds.OwnerRoleId, RbacSeedIds.DispatchesCreatePermissionId),
            Seed(RbacSeedIds.OwnerRoleId, RbacSeedIds.DispatchesReadPermissionId),
            Seed(RbacSeedIds.OwnerRoleId, RbacSeedIds.DispatchesUpdatePermissionId),
            Seed(RbacSeedIds.OwnerRoleId, RbacSeedIds.DispatchesDeletePermissionId),
            Seed(RbacSeedIds.OwnerRoleId, RbacSeedIds.UsersCreatePermissionId),
            Seed(RbacSeedIds.OwnerRoleId, RbacSeedIds.UsersReadPermissionId),
            Seed(RbacSeedIds.OwnerRoleId, RbacSeedIds.UsersUpdatePermissionId),
            Seed(RbacSeedIds.OwnerRoleId, RbacSeedIds.UsersDeletePermissionId),
            Seed(RbacSeedIds.OwnerRoleId, RbacSeedIds.CompaniesReadPermissionId),
            Seed(RbacSeedIds.OwnerRoleId, RbacSeedIds.CompaniesUpdatePermissionId),

            // Admin: dispatches CRUD, users create/read/update, companies read
            Seed(RbacSeedIds.AdminRoleId, RbacSeedIds.DispatchesCreatePermissionId),
            Seed(RbacSeedIds.AdminRoleId, RbacSeedIds.DispatchesReadPermissionId),
            Seed(RbacSeedIds.AdminRoleId, RbacSeedIds.DispatchesUpdatePermissionId),
            Seed(RbacSeedIds.AdminRoleId, RbacSeedIds.DispatchesDeletePermissionId),
            Seed(RbacSeedIds.AdminRoleId, RbacSeedIds.UsersCreatePermissionId),
            Seed(RbacSeedIds.AdminRoleId, RbacSeedIds.UsersReadPermissionId),
            Seed(RbacSeedIds.AdminRoleId, RbacSeedIds.UsersUpdatePermissionId),
            Seed(RbacSeedIds.AdminRoleId, RbacSeedIds.CompaniesReadPermissionId),

            // Driver: dispatches read only
            Seed(RbacSeedIds.DriverRoleId, RbacSeedIds.DispatchesReadPermissionId)
        );
    }

    private static object Seed(Guid roleId, Guid permissionId) => new
    {
        RoleId = roleId,
        PermissionId = permissionId,
        CreatedAt = RbacSeedIds.SeedTimestamp,
        UpdatedAt = RbacSeedIds.SeedTimestamp
    };
}
