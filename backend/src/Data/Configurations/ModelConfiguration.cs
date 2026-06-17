using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations;

public class ModelConfiguration : IEntityTypeConfiguration<Model>
{
    public void Configure(EntityTypeBuilder<Model> builder)
    {
        builder.ToTable("Model");

        builder.HasKey(m => m.ModelId);

        builder.Property(m => m.Code).IsRequired();
        builder.Property(m => m.Name).IsRequired();

        builder.Property(m => m.DateCreated).HasDefaultValueSql("GETUTCDATE()");
        builder.Property(m => m.DateLastUpdated).HasDefaultValueSql("GETUTCDATE()");
        builder.Property(m => m.CreatedBy).HasDefaultValue("SYSTEM");

        builder.HasOne(m => m.Make)
            .WithMany(mk => mk.Models)
            .HasForeignKey(m => m.MakeId);
    }
}
