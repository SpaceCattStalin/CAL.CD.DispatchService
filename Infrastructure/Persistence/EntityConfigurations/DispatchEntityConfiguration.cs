using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain;

namespace Infrastructure;

public class DispatchEntityConfiguration : IEntityTypeConfiguration<Dispatch>
{
    public void Configure(EntityTypeBuilder<Dispatch> builder)
    {
        builder.ToTable("dispatchs", t =>
        {
            // PickupDate/DropoffDate vs "current date" and the 0<Vehicles<=12 rule depend on
            // wall-clock time / sibling rows, so they're enforced in the validator/domain layer
            // instead of as DB check constraints.
            t.HasCheckConstraint("CK_dispatchs_dropoff_after_pickup", "dropoff_date > pickup_date");
        });

        builder.HasKey(x => x.DispatchId);

        builder.Property(x => x.DispatchStatus).HasColumnName("dispatch_status").HasConversion<string>();
        builder.Property(x => x.ShipperId).HasColumnName("shipper_id");
        builder.Property(x => x.CarrierId).HasColumnName("carrier_id");
        builder.Property(x => x.Price).HasColumnName("price");
        builder.Property(x => x.PickupDate).HasColumnName("pickup_date");
        builder.Property(x => x.DropoffDate).HasColumnName("dropoff_date");
        builder.Property(x => x.Description).HasColumnName("description").HasMaxLength(500);

        builder.Property(x => x.RecordVersion).HasColumnName("record_version").IsRowVersion();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");

        builder.HasMany(x => x.Vehicles)
           .WithOne(v => v.Dispatch)
           .HasForeignKey(x => x.DispatchId);
    }
}
