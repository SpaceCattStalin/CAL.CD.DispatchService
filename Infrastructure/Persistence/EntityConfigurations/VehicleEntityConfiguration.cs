using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain;

namespace Infrastructure;

public class VehicleEntityConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.ToTable("vehicles", t =>
        {
            t.HasCheckConstraint("CK_vehicles_vin_min_length", "LENGTH(vin) >= 10");
        });

        builder.HasKey(x => x.VehicleId);

        builder.Property(x => x.VehicleId).HasColumnName("vehicle_id");
        builder.Property(x => x.DispatchId).HasColumnName("dispatch_id");
        builder.Property(x => x.VehicleStatus).HasColumnName("vehicle_status").HasConversion<string>();
        builder.Property(x => x.Vin).HasColumnName("vin").HasMaxLength(15);
        builder.Property(x => x.Year).HasColumnName("year");
        builder.Property(x => x.Make).HasColumnName("make");
        builder.Property(x => x.Model).HasColumnName("model");
        builder.Property(x => x.Color).HasColumnName("color");
        builder.Property(x => x.PickupStopId).HasColumnName("pickup_stop_id");
        builder.Property(x => x.DropoffStopId).HasColumnName("dropoff_stop_id");

        builder.Property(x => x.RecordVersion).IsRowVersion();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");

        builder.HasOne(x => x.Dispatch)
            .WithMany(d => d.Vehicles)
            .HasForeignKey(x => x.DispatchId);

        builder.HasOne(x => x.PickupStop)
            .WithMany()
            .HasForeignKey(x => x.PickupStopId);

        builder.HasOne(x => x.DropoffStop)
            .WithMany()
            .HasForeignKey(x => x.DropoffStopId);
    }
}
