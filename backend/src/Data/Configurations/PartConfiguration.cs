using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations;

public class PartConfiguration : IEntityTypeConfiguration<Part>
{
    public void Configure(EntityTypeBuilder<Part> builder)
    {
        builder.ToTable("Part");

        builder.HasKey(p => p.PartId);

        builder.Property(p => p.Name).IsRequired();
        builder.Property(p => p.Description).IsRequired();
        builder.Property(p => p.ModelNumber).IsRequired(false);

        builder.HasMany(p => p.MaintenanceParts)
            .WithOne(mp => mp.Part)
            .HasForeignKey(mp => mp.PartId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
