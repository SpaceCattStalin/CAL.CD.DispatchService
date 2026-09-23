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

        builder.Property(x => x.CompanyId).HasColumnName("company_id");
        builder.Property(x => x.CompanyName).HasColumnName("company_name").HasMaxLength(50);
        builder.Property(x => x.CompanyPhone).HasColumnName("company_phone").HasMaxLength(12);
        builder.Property(x => x.CompanyEmail).HasColumnName("company_email").HasMaxLength(30);
        builder.Property(x => x.CompanyType).HasColumnName("type").HasConversion<string>();
        builder.Property(x => x.RecordVersion).IsRowVersion();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
    

        builder.HasMany(x => x.Users)
            .WithOne(u => u.Company)
            .HasForeignKey(u => u.CompanyId);

        builder.HasData(new
        {
            CompanyId = TestUserSeedIds.CompanyId,
            CompanyName = "Test Logistics Co",
            CompanyPhone = "1234567890",
            CompanyEmail = "contact@testlogistics.com",
            CompanyType = CompanyType.Shipper,
            CreatedAt = TestUserSeedIds.SeedTimestamp,
            UpdatedAt = TestUserSeedIds.SeedTimestamp
        });

        builder.HasData(
            new
            {
                CompanyId = DemoDataSeedIds.ApexCarriersId,
                CompanyName = "Apex Carriers LLC",
                CompanyPhone = "5550001001",
                CompanyEmail = "contact@apexcarriers.com",
                CompanyType = CompanyType.Carrier,
                CreatedAt = DemoDataSeedIds.SeedTimestamp,
                UpdatedAt = DemoDataSeedIds.SeedTimestamp
            },
            new
            {
                CompanyId = DemoDataSeedIds.BlueHorizonId,
                CompanyName = "Blue Horizon Transport",
                CompanyPhone = "5550001002",
                CompanyEmail = "ops@bluehorizon.com",
                CompanyType = CompanyType.Carrier,
                CreatedAt = DemoDataSeedIds.SeedTimestamp,
                UpdatedAt = DemoDataSeedIds.SeedTimestamp
            },
            new
            {
                CompanyId = DemoDataSeedIds.MidwestFreightId,
                CompanyName = "Midwest Freight Solutions",
                CompanyPhone = "5550001003",
                CompanyEmail = "dispatch@midwestfreight.com",
                CompanyType = CompanyType.Carrier,
                CreatedAt = DemoDataSeedIds.SeedTimestamp,
                UpdatedAt = DemoDataSeedIds.SeedTimestamp
            },
            new
            {
                CompanyId = DemoDataSeedIds.GoldenStateId,
                CompanyName = "Golden State Manufacturing",
                CompanyPhone = "5550001004",
                CompanyEmail = "shipping@goldenstate.com",
                CompanyType = CompanyType.Shipper,
                CreatedAt = DemoDataSeedIds.SeedTimestamp,
                UpdatedAt = DemoDataSeedIds.SeedTimestamp
            },
            new
            {
                CompanyId = DemoDataSeedIds.SummitRetailId,
                CompanyName = "Summit Retail Group",
                CompanyPhone = "5550001005",
                CompanyEmail = "logistics@summitretail.com",
                CompanyType = CompanyType.Shipper,
                CreatedAt = DemoDataSeedIds.SeedTimestamp,
                UpdatedAt = DemoDataSeedIds.SeedTimestamp
            }
        );
    }
}
