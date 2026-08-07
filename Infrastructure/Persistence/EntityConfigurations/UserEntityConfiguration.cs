using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain;

namespace Infrastructure;

public class UserEntityConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(x => x.UserId);

        builder.Property(x => x.FirstName).HasColumnName("first_name");
        builder.Property(x => x.LastName).HasColumnName("last_name");
        builder.Property(x => x.Phone).HasColumnName("phone");
        builder.Property(x => x.Email).HasColumnName("email");
        builder.Property(x => x.UserName).HasColumnName("user_name");
        builder.Property(x => x.PasswordHash).HasColumnName("password_hash");
        builder.Property(x => x.UserRole).HasColumnName("role").HasConversion<string>();
        builder.Property(x => x.IsActive).HasColumnName("is_active");
        builder.Property(x => x.CompanyId).HasColumnName("company_id");
        builder.Property(x => x.RecordVersion).HasColumnName("record_version").IsRowVersion();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");

        builder.HasOne(x => x.Company)
            .WithMany(c => c.Users)
            .HasForeignKey(x => x.CompanyId);
    }
}
