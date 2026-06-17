using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations;

public class MakeConfiguration : IEntityTypeConfiguration<Make>
{
    public void Configure(EntityTypeBuilder<Make> builder)
    {
        builder.ToTable("Make");

        builder.HasKey(m => m.MakeId);

        builder.Property(m => m.Code).IsRequired();
        builder.Property(m => m.Name).IsRequired();

        builder.Property(m => m.DateCreated).HasDefaultValueSql("GETUTCDATE()");
        builder.Property(m => m.DateLastUpdated).HasDefaultValueSql("GETUTCDATE()");
        builder.Property(m => m.CreatedBy).HasDefaultValue("SYSTEM");
    }
}
