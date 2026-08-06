using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain;

namespace Infrastructure;

public class StopEntityConfiguration : IEntityTypeConfiguration<Stop>
{
    public void Configure(EntityTypeBuilder<Stop> builder)
    {
        builder.ToTable("stops");

        builder.HasKey(x => x.StopId);

        builder.Property(x => x.StopNumber).HasColumnName("stop_number").HasConversion<string>();
        builder.Property(x => x.Address).HasColumnName("address");
        builder.Property(x => x.LocationName).HasColumnName("location_name");
        builder.Property(x => x.ContactEmail).HasColumnName("contact_email");
        builder.Property(x => x.ContactPhone).HasColumnName("contact_phone");
        builder.Property(x => x.ContactName).HasColumnName("contact_name");
        builder.Property(x => x.RecordVersion).HasColumnName("record_version").IsRowVersion();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
    }
}
