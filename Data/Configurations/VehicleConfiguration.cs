using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations;

public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.ToTable("Vehicle");

        builder.HasKey(v => v.VehicleId);

        builder.Property(v => v.Year).IsRequired();
        builder.Property(v => v.VIN).HasMaxLength(17).IsRequired(false);
        builder.Property(v => v.LicensePlate).HasMaxLength(8).IsRequired(false);
        builder.Property(v => v.PurchaseDate).IsRequired(false);
        builder.Property(v => v.PurchasePrice).HasPrecision(18, 2).IsRequired(false);
        builder.Property(v => v.MileageAtPurchase).IsRequired(false);
        builder.Property(v => v.Mileage).IsRequired(false);

        builder.HasOne(v => v.Make)
            .WithMany()
            .HasForeignKey(v => v.MakeId)
            .IsRequired();

        builder.HasOne(v => v.Model)
            .WithMany()
            .HasForeignKey(v => v.ModelId)
            .IsRequired();

        builder.HasMany(v => v.MaintenanceRecords)
            .WithOne(m => m.Vehicle)
            .HasForeignKey(m => m.VehicleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
