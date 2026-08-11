using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain;

namespace Infrastructure;

public class RoleEntityConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");

        builder.HasKey(x => x.RoleId);

        builder.Property(x => x.RoleId).HasColumnName("role_id");
        builder.Property(x => x.Name).HasColumnName("name");
        builder.HasIndex(x => x.Name).IsUnique();

        builder.Property(x => x.RecordVersion).IsRowVersion();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");

        builder.HasData(
            new { RoleId = RbacSeedIds.OwnerRoleId, Name = "Owner", CreatedAt = RbacSeedIds.SeedTimestamp, UpdatedAt = RbacSeedIds.SeedTimestamp },
            new { RoleId = RbacSeedIds.AdminRoleId, Name = "Admin", CreatedAt = RbacSeedIds.SeedTimestamp, UpdatedAt = RbacSeedIds.SeedTimestamp },
            new { RoleId = RbacSeedIds.DriverRoleId, Name = "Driver", CreatedAt = RbacSeedIds.SeedTimestamp, UpdatedAt = RbacSeedIds.SeedTimestamp }
        );
    }
}
