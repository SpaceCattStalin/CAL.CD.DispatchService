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

        builder.Property(x => x.UserId).HasColumnName("user_id");
        builder.Property(x => x.FirstName).HasColumnName("first_name").HasMaxLength(50);
        builder.Property(x => x.LastName).HasColumnName("last_name").HasMaxLength(50);
        builder.Property(x => x.Phone).HasColumnName("phone").HasMaxLength(12);
        builder.Property(x => x.Email).HasColumnName("email").HasMaxLength(30);
        builder.Property(x => x.UserName).HasColumnName("user_name").HasMaxLength(20);
        builder.Property(x => x.PasswordHash).HasColumnName("password_hash");
        builder.Property(x => x.UserRole).HasColumnName("role").HasConversion<string>();
        builder.Property(x => x.IsActive).HasColumnName("is_active");
        builder.Property(x => x.CompanyId).HasColumnName("company_id");
        builder.Property(x => x.RecordVersion).IsRowVersion();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");

        builder.HasOne(x => x.Company)
            .WithMany(c => c.Users)
            .HasForeignKey(x => x.CompanyId);

        builder.HasData(new
        {
            UserId = TestUserSeedIds.UserId,
            FirstName = "Tester",
            LastName = "Owner",
            Phone = "1234567890",
            Email = "owner@testlogistics.com",
            UserName = "testowner",
            PasswordHash = TestUserSeedIds.PasswordHash,
            UserRole = UserRole.Owner,
            IsActive = true,
            CompanyId = TestUserSeedIds.CompanyId,
            CreatedAt = TestUserSeedIds.SeedTimestamp,
            UpdatedAt = TestUserSeedIds.SeedTimestamp
        });

        builder.HasData(new
        {
            UserId = TestUserSeedIds.CarrierOwnerId,
            FirstName = "Carrier",
            LastName = "Ownerrrrrr",
            Phone = "1234567890",
            Email = "owner@testcarriers.com",
            UserName = "testcarrier",
            PasswordHash = TestUserSeedIds.PasswordHash,
            UserRole = UserRole.Owner,
            IsActive = true,
            CompanyId = new Guid("dc63068f-dbfc-422c-8464-f47698fd8905"),
            CreatedAt = TestUserSeedIds.SeedTimestamp,
            UpdatedAt = TestUserSeedIds.SeedTimestamp
        });

        builder.HasData(new
        {
            UserId = TestUserSeedIds.CarrierDriverId,
            FirstName = "Driver",
            LastName = "Driverrrrrr",
            Phone = "1234567890",
            Email = "owner@testdriverr.com",
            UserName = "testdriver",
            PasswordHash = TestUserSeedIds.PasswordHash,
            UserRole = UserRole.Driver,
            IsActive = true,
            CompanyId = new Guid("dc63068f-dbfc-422c-8464-f47698fd8905"),
            CreatedAt = TestUserSeedIds.SeedTimestamp,
            UpdatedAt = TestUserSeedIds.SeedTimestamp
        });

        builder.HasData(new
        {
            UserId = RbacSeedIds.SyncRoleId,
            FirstName = "SyncJobbbb",
            LastName = "SyncJobbb",
            Phone = "12345678910",
            Email = "syncjob@placeholder.com",
            UserName = "SyncJobbbb",
            PasswordHash = TestUserSeedIds.PasswordHash,
            UserRole = UserRole.SyncJob,
            IsActive = true,
            CompanyId = TestUserSeedIds.CompanyId,
            CreatedAt = TestUserSeedIds.SeedTimestamp,
            UpdatedAt = TestUserSeedIds.SeedTimestamp
        });

        builder.HasData(
            // Apex Carriers LLC
            new
            {
                UserId = DemoDataSeedIds.ApexCarriersOwnerId,
                FirstName = "Marcus",
                LastName = "Alden",
                Phone = "5550002001",
                Email = "m.alden@apexcarriers.com",
                UserName = "malden01",
                PasswordHash = TestUserSeedIds.PasswordHash,
                UserRole = UserRole.Owner,
                IsActive = true,
                CompanyId = DemoDataSeedIds.ApexCarriersId,
                CreatedAt = DemoDataSeedIds.SeedTimestamp,
                UpdatedAt = DemoDataSeedIds.SeedTimestamp
            },
            new
            {
                UserId = DemoDataSeedIds.ApexCarriersDriver1Id,
                FirstName = "Derek",
                LastName = "Simmons",
                Phone = "5550002002",
                Email = "d.simmons@apexcarriers.com",
                UserName = "dsimmons01",
                PasswordHash = TestUserSeedIds.PasswordHash,
                UserRole = UserRole.Driver,
                IsActive = true,
                CompanyId = DemoDataSeedIds.ApexCarriersId,
                CreatedAt = DemoDataSeedIds.SeedTimestamp,
                UpdatedAt = DemoDataSeedIds.SeedTimestamp
            },
            new
            {
                UserId = DemoDataSeedIds.ApexCarriersDriver2Id,
                FirstName = "Nathan",
                LastName = "Coleman",
                Phone = "5550002003",
                Email = "n.coleman@apexcarriers.com",
                UserName = "ncoleman01",
                PasswordHash = TestUserSeedIds.PasswordHash,
                UserRole = UserRole.Driver,
                IsActive = true,
                CompanyId = DemoDataSeedIds.ApexCarriersId,
                CreatedAt = DemoDataSeedIds.SeedTimestamp,
                UpdatedAt = DemoDataSeedIds.SeedTimestamp
            },
            new
            {
                UserId = DemoDataSeedIds.ApexCarriersDriver3Id,
                FirstName = "Bradley",
                LastName = "Sutton",
                Phone = "5550002016",
                Email = "b.sutton@apexcarriers.com",
                UserName = "bsutton01",
                PasswordHash = TestUserSeedIds.PasswordHash,
                UserRole = UserRole.Driver,
                IsActive = true,
                CompanyId = DemoDataSeedIds.ApexCarriersId,
                CreatedAt = DemoDataSeedIds.SeedTimestamp,
                UpdatedAt = DemoDataSeedIds.SeedTimestamp
            },
            new
            {
                UserId = DemoDataSeedIds.ApexCarriersDriver4Id,
                FirstName = "Melissa",
                LastName = "Grover",
                Phone = "5550002017",
                Email = "m.grover@apexcarriers.com",
                UserName = "mgrover01",
                PasswordHash = TestUserSeedIds.PasswordHash,
                UserRole = UserRole.Driver,
                IsActive = true,
                CompanyId = DemoDataSeedIds.ApexCarriersId,
                CreatedAt = DemoDataSeedIds.SeedTimestamp,
                UpdatedAt = DemoDataSeedIds.SeedTimestamp
            },
            new
            {
                UserId = DemoDataSeedIds.ApexCarriersDriver5Id,
                FirstName = "Wesley",
                LastName = "Barton",
                Phone = "5550002018",
                Email = "w.barton@apexcarriers.com",
                UserName = "wbarton01",
                PasswordHash = TestUserSeedIds.PasswordHash,
                UserRole = UserRole.Driver,
                IsActive = true,
                CompanyId = DemoDataSeedIds.ApexCarriersId,
                CreatedAt = DemoDataSeedIds.SeedTimestamp,
                UpdatedAt = DemoDataSeedIds.SeedTimestamp
            },

            // Blue Horizon Transport
            new
            {
                UserId = DemoDataSeedIds.BlueHorizonOwnerId,
                FirstName = "Renee",
                LastName = "Whitfield",
                Phone = "5550002004",
                Email = "r.whitfield@bluehorizon.com",
                UserName = "rwhitfield01",
                PasswordHash = TestUserSeedIds.PasswordHash,
                UserRole = UserRole.Owner,
                IsActive = true,
                CompanyId = DemoDataSeedIds.BlueHorizonId,
                CreatedAt = DemoDataSeedIds.SeedTimestamp,
                UpdatedAt = DemoDataSeedIds.SeedTimestamp
            },
            new
            {
                UserId = DemoDataSeedIds.BlueHorizonDriver1Id,
                FirstName = "Oscar",
                LastName = "Bennett",
                Phone = "5550002005",
                Email = "o.bennett@bluehorizon.com",
                UserName = "obennett01",
                PasswordHash = TestUserSeedIds.PasswordHash,
                UserRole = UserRole.Driver,
                IsActive = true,
                CompanyId = DemoDataSeedIds.BlueHorizonId,
                CreatedAt = DemoDataSeedIds.SeedTimestamp,
                UpdatedAt = DemoDataSeedIds.SeedTimestamp
            },
            new
            {
                UserId = DemoDataSeedIds.BlueHorizonDriver2Id,
                FirstName = "Miguel",
                LastName = "Torres",
                Phone = "5550002006",
                Email = "m.torres@bluehorizon.com",
                UserName = "mtorres01",
                PasswordHash = TestUserSeedIds.PasswordHash,
                UserRole = UserRole.Driver,
                IsActive = true,
                CompanyId = DemoDataSeedIds.BlueHorizonId,
                CreatedAt = DemoDataSeedIds.SeedTimestamp,
                UpdatedAt = DemoDataSeedIds.SeedTimestamp
            },
            new
            {
                UserId = DemoDataSeedIds.BlueHorizonDriver3Id,
                FirstName = "Rachel",
                LastName = "Doyle",
                Phone = "5550002019",
                Email = "r.doyle@bluehorizon.com",
                UserName = "rdoyle01",
                PasswordHash = TestUserSeedIds.PasswordHash,
                UserRole = UserRole.Driver,
                IsActive = true,
                CompanyId = DemoDataSeedIds.BlueHorizonId,
                CreatedAt = DemoDataSeedIds.SeedTimestamp,
                UpdatedAt = DemoDataSeedIds.SeedTimestamp
            },
            new
            {
                UserId = DemoDataSeedIds.BlueHorizonDriver4Id,
                FirstName = "Nathaniel",
                LastName = "Vance",
                Phone = "5550002020",
                Email = "n.vance@bluehorizon.com",
                UserName = "nvance01",
                PasswordHash = TestUserSeedIds.PasswordHash,
                UserRole = UserRole.Driver,
                IsActive = true,
                CompanyId = DemoDataSeedIds.BlueHorizonId,
                CreatedAt = DemoDataSeedIds.SeedTimestamp,
                UpdatedAt = DemoDataSeedIds.SeedTimestamp
            },
            new
            {
                UserId = DemoDataSeedIds.BlueHorizonDriver5Id,
                FirstName = "Priscilla",
                LastName = "Hayes",
                Phone = "5550002021",
                Email = "p.hayes@bluehorizon.com",
                UserName = "phayes01",
                PasswordHash = TestUserSeedIds.PasswordHash,
                UserRole = UserRole.Driver,
                IsActive = true,
                CompanyId = DemoDataSeedIds.BlueHorizonId,
                CreatedAt = DemoDataSeedIds.SeedTimestamp,
                UpdatedAt = DemoDataSeedIds.SeedTimestamp
            },

            // Midwest Freight Solutions
            new
            {
                UserId = DemoDataSeedIds.MidwestFreightOwnerId,
                FirstName = "Diana",
                LastName = "Foster",
                Phone = "5550002007",
                Email = "d.foster@midwestfreight.com",
                UserName = "dfoster01",
                PasswordHash = TestUserSeedIds.PasswordHash,
                UserRole = UserRole.Owner,
                IsActive = true,
                CompanyId = DemoDataSeedIds.MidwestFreightId,
                CreatedAt = DemoDataSeedIds.SeedTimestamp,
                UpdatedAt = DemoDataSeedIds.SeedTimestamp
            },
            new
            {
                UserId = DemoDataSeedIds.MidwestFreightDriver1Id,
                FirstName = "Trevor",
                LastName = "Banks",
                Phone = "5550002008",
                Email = "t.banks@midwestfreight.com",
                UserName = "tbanks01",
                PasswordHash = TestUserSeedIds.PasswordHash,
                UserRole = UserRole.Driver,
                IsActive = true,
                CompanyId = DemoDataSeedIds.MidwestFreightId,
                CreatedAt = DemoDataSeedIds.SeedTimestamp,
                UpdatedAt = DemoDataSeedIds.SeedTimestamp
            },
            new
            {
                UserId = DemoDataSeedIds.MidwestFreightDriver2Id,
                FirstName = "Isaac",
                LastName = "Meyer",
                Phone = "5550002009",
                Email = "i.meyer@midwestfreight.com",
                UserName = "imeyer01",
                PasswordHash = TestUserSeedIds.PasswordHash,
                UserRole = UserRole.Driver,
                IsActive = true,
                CompanyId = DemoDataSeedIds.MidwestFreightId,
                CreatedAt = DemoDataSeedIds.SeedTimestamp,
                UpdatedAt = DemoDataSeedIds.SeedTimestamp
            },
            new
            {
                UserId = DemoDataSeedIds.MidwestFreightDriver3Id,
                FirstName = "Gregory",
                LastName = "Lambert",
                Phone = "5550002022",
                Email = "g.lambert@midwestfreight.com",
                UserName = "glambert01",
                PasswordHash = TestUserSeedIds.PasswordHash,
                UserRole = UserRole.Driver,
                IsActive = true,
                CompanyId = DemoDataSeedIds.MidwestFreightId,
                CreatedAt = DemoDataSeedIds.SeedTimestamp,
                UpdatedAt = DemoDataSeedIds.SeedTimestamp
            },
            new
            {
                UserId = DemoDataSeedIds.MidwestFreightDriver4Id,
                FirstName = "Vanessa",
                LastName = "Pruitt",
                Phone = "5550002023",
                Email = "v.pruitt@midwestfreight.com",
                UserName = "vpruitt01",
                PasswordHash = TestUserSeedIds.PasswordHash,
                UserRole = UserRole.Driver,
                IsActive = true,
                CompanyId = DemoDataSeedIds.MidwestFreightId,
                CreatedAt = DemoDataSeedIds.SeedTimestamp,
                UpdatedAt = DemoDataSeedIds.SeedTimestamp
            },
            new
            {
                UserId = DemoDataSeedIds.MidwestFreightDriver5Id,
                FirstName = "Harold",
                LastName = "Chambers",
                Phone = "5550002024",
                Email = "h.chambers@midwestfreight.com",
                UserName = "hchambers01",
                PasswordHash = TestUserSeedIds.PasswordHash,
                UserRole = UserRole.Driver,
                IsActive = true,
                CompanyId = DemoDataSeedIds.MidwestFreightId,
                CreatedAt = DemoDataSeedIds.SeedTimestamp,
                UpdatedAt = DemoDataSeedIds.SeedTimestamp
            },

            // Golden State Manufacturing
            new
            {
                UserId = DemoDataSeedIds.GoldenStateOwnerId,
                FirstName = "Sophia",
                LastName = "Grant",
                Phone = "5550002010",
                Email = "s.grant@goldenstate.com",
                UserName = "sgrant01",
                PasswordHash = TestUserSeedIds.PasswordHash,
                UserRole = UserRole.Owner,
                IsActive = true,
                CompanyId = DemoDataSeedIds.GoldenStateId,
                CreatedAt = DemoDataSeedIds.SeedTimestamp,
                UpdatedAt = DemoDataSeedIds.SeedTimestamp
            },
            new
            {
                UserId = DemoDataSeedIds.GoldenStateDriver1Id,
                FirstName = "Connor",
                LastName = "Reyes",
                Phone = "5550002011",
                Email = "c.reyes@goldenstate.com",
                UserName = "creyes01",
                PasswordHash = TestUserSeedIds.PasswordHash,
                UserRole = UserRole.Driver,
                IsActive = true,
                CompanyId = DemoDataSeedIds.GoldenStateId,
                CreatedAt = DemoDataSeedIds.SeedTimestamp,
                UpdatedAt = DemoDataSeedIds.SeedTimestamp
            },
            new
            {
                UserId = DemoDataSeedIds.GoldenStateDriver2Id,
                FirstName = "Julia",
                LastName = "Stanton",
                Phone = "5550002012",
                Email = "j.stanton@goldenstate.com",
                UserName = "jstanton01",
                PasswordHash = TestUserSeedIds.PasswordHash,
                UserRole = UserRole.Driver,
                IsActive = true,
                CompanyId = DemoDataSeedIds.GoldenStateId,
                CreatedAt = DemoDataSeedIds.SeedTimestamp,
                UpdatedAt = DemoDataSeedIds.SeedTimestamp
            },

            // Summit Retail Group
            new
            {
                UserId = DemoDataSeedIds.SummitRetailOwnerId,
                FirstName = "Victor",
                LastName = "Holloway",
                Phone = "5550002013",
                Email = "v.holloway@summitretail.com",
                UserName = "vholloway01",
                PasswordHash = TestUserSeedIds.PasswordHash,
                UserRole = UserRole.Owner,
                IsActive = true,
                CompanyId = DemoDataSeedIds.SummitRetailId,
                CreatedAt = DemoDataSeedIds.SeedTimestamp,
                UpdatedAt = DemoDataSeedIds.SeedTimestamp
            },
            new
            {
                UserId = DemoDataSeedIds.SummitRetailDriver1Id,
                FirstName = "Grace",
                LastName = "Pemberton",
                Phone = "5550002014",
                Email = "g.pemberton@summitretail.com",
                UserName = "gpemberton01",
                PasswordHash = TestUserSeedIds.PasswordHash,
                UserRole = UserRole.Driver,
                IsActive = true,
                CompanyId = DemoDataSeedIds.SummitRetailId,
                CreatedAt = DemoDataSeedIds.SeedTimestamp,
                UpdatedAt = DemoDataSeedIds.SeedTimestamp
            },
            new
            {
                UserId = DemoDataSeedIds.SummitRetailDriver2Id,
                FirstName = "Ethan",
                LastName = "Ramirez",
                Phone = "5550002015",
                Email = "e.ramirez@summitretail.com",
                UserName = "eramirez01",
                PasswordHash = TestUserSeedIds.PasswordHash,
                UserRole = UserRole.Driver,
                IsActive = true,
                CompanyId = DemoDataSeedIds.SummitRetailId,
                CreatedAt = DemoDataSeedIds.SeedTimestamp,
                UpdatedAt = DemoDataSeedIds.SeedTimestamp
            }
        );
    }
}
