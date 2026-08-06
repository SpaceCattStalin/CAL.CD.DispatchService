using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain;

namespace Infrastructure;

public class CompanyEntityConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("companies");

        builder.HasKey(x => x.CompanyId);

        builder.Property(x => x.CompanyName).HasColumnName("company_name");
        builder.Property(x => x.CompanyPhone).HasColumnName("company_phone");
        builder.Property(x => x.CompanyEmail).HasColumnName("company_email");
        builder.Property(x => x.CompanyType).HasColumnName("type").HasConversion<string>();
        builder.Property(x => x.RecordVersion).HasColumnName("record_version").IsRowVersion();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");

        builder.HasMany(x => x.Users)
            .WithOne(u => u.Company)
            .HasForeignKey(u => u.CompanyId);
    }
}
