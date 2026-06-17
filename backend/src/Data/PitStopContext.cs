using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class PitStopContext : DbContext
{
    public PitStopContext(DbContextOptions<PitStopContext> options) : base(options) { }

    public DbSet<Make> Makes { get; set; }
    public DbSet<Model> Models { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<Maintenance> MaintenanceRecords { get; set; }
    public DbSet<Part> Parts { get; set; }
    public DbSet<MaintenancePart> MaintenanceParts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PitStopContext).Assembly);
    }
}
