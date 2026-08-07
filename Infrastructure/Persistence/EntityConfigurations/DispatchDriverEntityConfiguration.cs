using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure;

public class DispatchDriverEntityConfiguration : IEntityTypeConfiguration<DispatchDriver>
{
    public void Configure(EntityTypeBuilder<DispatchDriver> builder)
    {
        builder.ToTable("dispatch_drivers");

        builder.HasKey(x => new { x.DispatchId, x.DriverId });

        builder.Property(x => x.DispatchId).HasColumnName("dispatch_id");
        builder.Property(x => x.DriverId).HasColumnName("driver_id");


        builder.HasOne(x => x.Dispatch)
            .WithMany(x => x.Drivers)
            .HasForeignKey(x => x.DispatchId);

        builder.HasOne(x => x.Driver)
            .WithMany()
            .HasForeignKey(x => x.DriverId);
    }
}
