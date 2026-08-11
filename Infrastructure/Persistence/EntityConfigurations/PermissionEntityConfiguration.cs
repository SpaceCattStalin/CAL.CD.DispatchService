using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain;

namespace Infrastructure;

public class PermissionEntityConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("permissions");

        builder.HasKey(x => x.PermissionId);

        builder.Property(x => x.PermissionId).HasColumnName("permission_id");
        builder.Property(x => x.Name).HasColumnName("name");
        builder.HasIndex(x => x.Name).IsUnique();

        builder.Property(x => x.RecordVersion).IsRowVersion();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");

        builder.HasData(
            new { PermissionId = RbacSeedIds.DispatchesCreatePermissionId, Name = "dispatches:create", CreatedAt = RbacSeedIds.SeedTimestamp, UpdatedAt = RbacSeedIds.SeedTimestamp },
            new { PermissionId = RbacSeedIds.DispatchesReadPermissionId, Name = "dispatches:read", CreatedAt = RbacSeedIds.SeedTimestamp, UpdatedAt = RbacSeedIds.SeedTimestamp },
            new { PermissionId = RbacSeedIds.DispatchesUpdatePermissionId, Name = "dispatches:update", CreatedAt = RbacSeedIds.SeedTimestamp, UpdatedAt = RbacSeedIds.SeedTimestamp },
            new { PermissionId = RbacSeedIds.DispatchesDeletePermissionId, Name = "dispatches:delete", CreatedAt = RbacSeedIds.SeedTimestamp, UpdatedAt = RbacSeedIds.SeedTimestamp },
            new { PermissionId = RbacSeedIds.UsersCreatePermissionId, Name = "users:create", CreatedAt = RbacSeedIds.SeedTimestamp, UpdatedAt = RbacSeedIds.SeedTimestamp },
            new { PermissionId = RbacSeedIds.UsersReadPermissionId, Name = "users:read", CreatedAt = RbacSeedIds.SeedTimestamp, UpdatedAt = RbacSeedIds.SeedTimestamp },
            new { PermissionId = RbacSeedIds.UsersUpdatePermissionId, Name = "users:update", CreatedAt = RbacSeedIds.SeedTimestamp, UpdatedAt = RbacSeedIds.SeedTimestamp },
            new { PermissionId = RbacSeedIds.UsersDeletePermissionId, Name = "users:delete", CreatedAt = RbacSeedIds.SeedTimestamp, UpdatedAt = RbacSeedIds.SeedTimestamp },
            new { PermissionId = RbacSeedIds.CompaniesReadPermissionId, Name = "companies:read", CreatedAt = RbacSeedIds.SeedTimestamp, UpdatedAt = RbacSeedIds.SeedTimestamp },
            new { PermissionId = RbacSeedIds.CompaniesUpdatePermissionId, Name = "companies:update", CreatedAt = RbacSeedIds.SeedTimestamp, UpdatedAt = RbacSeedIds.SeedTimestamp }
        );
    }
}
