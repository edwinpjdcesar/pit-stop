using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations;

public class MaintenancePartConfiguration : IEntityTypeConfiguration<MaintenancePart>
{
    public void Configure(EntityTypeBuilder<MaintenancePart> builder)
    {
        builder.ToTable("MaintenancePart");

        builder.HasKey(mp => new { mp.MaintenanceId, mp.PartId });

        builder.Property(mp => mp.Quantity).IsRequired();
        builder.Property(mp => mp.UnitPrice).HasPrecision(18, 2).IsRequired();
    }
}
