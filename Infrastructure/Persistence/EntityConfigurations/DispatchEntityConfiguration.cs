using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain;

namespace Infrastructure;

public class DispatchEntityConfiguration : IEntityTypeConfiguration<Dispatch>
{
    public void Configure(EntityTypeBuilder<Dispatch> builder)
    {
        builder.ToTable("dispatches", t =>
        {
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
        builder.Property(x => x.PickupStopId).HasColumnName("pickup_stop_id");
        builder.Property(x => x.DropoffStopId).HasColumnName("dropoff_stop_id");
        builder.Property(x => x.IsSigned).HasColumnName("is_signed");

        builder.Property(x => x.RecordVersion).HasColumnName("record_version").IsRowVersion();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");

        builder.HasMany(x => x.Vehicles)
           .WithOne(v => v.Dispatch)
           .HasForeignKey(x => x.DispatchId);

        builder.HasOne(x => x.PickupStop)
            .WithMany()
            .HasForeignKey(x => x.PickupStopId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.DropoffStop)
            .WithMany()
            .HasForeignKey(x => x.DropoffStopId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
