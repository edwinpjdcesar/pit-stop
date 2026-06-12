using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations;

public class MaintenanceConfiguration : IEntityTypeConfiguration<Maintenance>
{
    public void Configure(EntityTypeBuilder<Maintenance> builder)
    {
        builder.ToTable("Maintenance");

        builder.HasKey(m => m.MaintenanceId);

        builder.Property(m => m.Description).IsRequired();
        builder.Property(m => m.Mileage).IsRequired();
        builder.Property(m => m.ServiceDate).IsRequired();

        builder.HasOne(m => m.Vehicle)
            .WithMany(v => v.MaintenanceRecords)
            .HasForeignKey(m => m.VehicleId)
            .IsRequired();

        builder.HasMany(m => m.MaintenanceParts)
            .WithOne(mp => mp.Maintenance)
            .HasForeignKey(mp => mp.MaintenanceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
