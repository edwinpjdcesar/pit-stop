namespace Data.Models;

public class MaintenancePart : BaseAuditEntity
{
    public Guid MaintenanceId { get; set; }
    public Maintenance Maintenance { get; set; } = null!;

    public Guid PartId { get; set; }
    public Part Part { get; set; } = null!;

    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
