using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain;

namespace Infrastructure;

public class DispatchEntityConfiguration : IEntityTypeConfiguration<Dispatch>
{
    public void Configure(EntityTypeBuilder<Dispatch> builder)
    {
        builder.ToTable("dispatchs");

        builder.HasKey(x => x.DispatchId);

        builder.Property(x => x.DispatchStatus).HasColumnName("dispatch_status").HasConversion<string>();
        builder.Property(x => x.ShipperId).HasColumnName("shipper_id");
        builder.Property(x => x.CarrierId).HasColumnName("carrier_id");
        builder.Property(x => x.PriceTotal).HasColumnName("price_total");
        builder.Property(x => x.Instructions).HasColumnName("instructions");

        builder.Property(x => x.RecordVersion).HasColumnName("record_version").IsRowVersion();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");

        builder.HasMany(x => x.Vehicles)
           .WithOne(v => v.Dispatch)
           .HasForeignKey(x => x.DispatchId);
    }
}
