namespace Domain.Entities;

public class Maintenance : BaseAuditEntity
{
    public Guid MaintenanceId { get; set; }

    public Guid VehicleId { get; set; }
    public Vehicle Vehicle { get; set; } = null!;

    public string Description { get; set; } = string.Empty;
    public int Mileage { get; set; }
    public DateTime ServiceDate { get; set; }

    public ICollection<MaintenancePart> MaintenanceParts { get; set; } = [];
}
