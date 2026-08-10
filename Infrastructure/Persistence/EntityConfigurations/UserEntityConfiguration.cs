using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain;

namespace Infrastructure;

public class UserEntityConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users", t =>
        {
            t.HasCheckConstraint("CK_users_first_name_min_length", "LENGTH(first_name) >= 5");
            t.HasCheckConstraint("CK_users_last_name_min_length", "LENGTH(last_name) >= 5");
            t.HasCheckConstraint("CK_users_phone_min_length", "LENGTH(phone) >= 10");
            t.HasCheckConstraint("CK_users_email_min_length", "LENGTH(email) >= 15");
            t.HasCheckConstraint("CK_users_user_name_min_length", "LENGTH(user_name) >= 6");
        });

        builder.HasKey(x => x.UserId);

        builder.Property(x => x.FirstName).HasColumnName("first_name").HasMaxLength(50);
        builder.Property(x => x.LastName).HasColumnName("last_name").HasMaxLength(50);
        builder.Property(x => x.Phone).HasColumnName("phone").HasMaxLength(12);
        builder.Property(x => x.Email).HasColumnName("email").HasMaxLength(30);
        builder.Property(x => x.UserName).HasColumnName("user_name").HasMaxLength(20);
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
