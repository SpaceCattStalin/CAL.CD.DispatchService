using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain;

namespace Infrastructure;

public class StopEntityConfiguration : IEntityTypeConfiguration<Stop>
{
    public void Configure(EntityTypeBuilder<Stop> builder)
    {
        builder.ToTable("stops", t =>
        {
            t.HasCheckConstraint("CK_stops_address_min_length", "LENGTH(address) >= 20");
            t.HasCheckConstraint("CK_stops_location_name_min_length", "LENGTH(location_name) >= 10");
            t.HasCheckConstraint("CK_stops_contact_name_min_length", "LENGTH(contact_name) >= 5");
            t.HasCheckConstraint("CK_stops_contact_phone_min_length", "LENGTH(contact_phone) >= 10");
            t.HasCheckConstraint("CK_stops_contact_email_min_length", "LENGTH(contact_email) >= 15");
        });

        builder.HasKey(x => x.StopId);

        builder.Property(x => x.StopNumber).HasColumnName("stop_number").HasConversion<string>();
        builder.Property(x => x.Address).HasColumnName("address").HasMaxLength(100);
        builder.Property(x => x.LocationName).HasColumnName("location_name").HasMaxLength(30);
        builder.Property(x => x.ContactEmail).HasColumnName("contact_email").HasMaxLength(30);
        builder.Property(x => x.ContactPhone).HasColumnName("contact_phone").HasMaxLength(12);
        builder.Property(x => x.ContactName).HasColumnName("contact_name").HasMaxLength(50);
        builder.Property(x => x.RecordVersion).HasColumnName("record_version").IsRowVersion();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
    }
}
