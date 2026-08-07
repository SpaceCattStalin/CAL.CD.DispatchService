using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain;

namespace Infrastructure;

public class CompanyEntityConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("companies", t =>
        {
            t.HasCheckConstraint("CK_companies_company_name_min_length", "LENGTH(company_name) >= 5");
            t.HasCheckConstraint("CK_companies_company_phone_min_length", "LENGTH(company_phone) >= 10");
            t.HasCheckConstraint("CK_companies_company_email_min_length", "LENGTH(company_email) >= 15");
        });

        builder.HasKey(x => x.CompanyId);

        builder.Property(x => x.CompanyName).HasColumnName("company_name").HasMaxLength(50);
        builder.Property(x => x.CompanyPhone).HasColumnName("company_phone").HasMaxLength(12);
        builder.Property(x => x.CompanyEmail).HasColumnName("company_email").HasMaxLength(30);
        builder.Property(x => x.CompanyType).HasColumnName("type").HasConversion<string>();
        builder.Property(x => x.RecordVersion).HasColumnName("record_version").IsRowVersion();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");

        builder.HasMany(x => x.Users)
            .WithOne(u => u.Company)
            .HasForeignKey(u => u.CompanyId);
    }
}
