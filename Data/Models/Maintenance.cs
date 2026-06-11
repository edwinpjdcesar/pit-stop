namespace Data.Models;

public class Maintenance : BaseEntity<Guid>
{
    public Guid VehicleId { get; set; }
    public Vehicle Vehicle { get; set; } = null!;

    public string Description { get; set; } = string.Empty;
    public int Mileage { get; set; }
    public DateTime DateOfService { get; set; }

    public ICollection<MaintenancePart> MaintenanceParts { get; set; } = [];
}
