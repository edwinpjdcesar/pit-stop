using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class PitStopContext : DbContext
{
    public PitStopContext(DbContextOptions<PitStopContext> options) : base(options) { }

    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<Maintenance> MaintenanceRecords { get; set; }
    public DbSet<Part> Parts { get; set; }
    public DbSet<MaintenancePart> MaintenanceParts { get; set; }
    public DbSet<Make> Makes { get; set; }
    public DbSet<Model> Models { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Make>().ToTable("Make");
        modelBuilder.Entity<Model>().ToTable("Model");
        modelBuilder.Entity<Vehicle>().ToTable("Vehicle");
        modelBuilder.Entity<Maintenance>().ToTable("Maintenance");
        modelBuilder.Entity<Part>().ToTable("Part");
        modelBuilder.Entity<MaintenancePart>().ToTable("MaintenancePart");

        modelBuilder.Entity<Make>()
            .Property(m => m.DateCreated).HasDefaultValueSql("GETUTCDATE()");
        modelBuilder.Entity<Make>()
            .Property(m => m.DateLastUpdated).HasDefaultValueSql("GETUTCDATE()");
        modelBuilder.Entity<Make>()
            .Property(m => m.CreatedBy).HasDefaultValue("SYSTEM");

        modelBuilder.Entity<Model>()
            .Property(m => m.DateCreated).HasDefaultValueSql("GETUTCDATE()");
        modelBuilder.Entity<Model>()
            .Property(m => m.DateLastUpdated).HasDefaultValueSql("GETUTCDATE()");
        modelBuilder.Entity<Model>()
            .Property(m => m.CreatedBy).HasDefaultValue("SYSTEM");

        modelBuilder.Entity<MaintenancePart>()
            .HasKey(mp => new { mp.MaintenanceId, mp.PartId });

        modelBuilder.Entity<MaintenancePart>()
            .HasOne(mp => mp.Maintenance)
            .WithMany(m => m.MaintenanceParts)
            .HasForeignKey(mp => mp.MaintenanceId);

        modelBuilder.Entity<MaintenancePart>()
            .HasOne(mp => mp.Part)
            .WithMany(p => p.MaintenanceParts)
            .HasForeignKey(mp => mp.PartId);

        modelBuilder.Entity<Maintenance>()
            .HasOne(m => m.Vehicle)
            .WithMany(v => v.MaintenanceRecords)
            .HasForeignKey(m => m.VehicleId);

        modelBuilder.Entity<Model>()
            .HasOne(m => m.Make)
            .WithMany(mk => mk.Models)
            .HasForeignKey(m => m.MakeId);

        modelBuilder.Entity<Vehicle>()
            .HasOne(v => v.Make)
            .WithMany()
            .HasForeignKey(v => v.MakeId);

        modelBuilder.Entity<Vehicle>()
            .HasOne(v => v.Model)
            .WithMany()
            .HasForeignKey(v => v.ModelId);

        modelBuilder.Entity<Vehicle>()
            .Property(v => v.PriceAtPurchase)
            .HasPrecision(18, 2);

        modelBuilder.Entity<MaintenancePart>()
            .Property(mp => mp.UnitPrice)
            .HasPrecision(18, 2);
    }
}
