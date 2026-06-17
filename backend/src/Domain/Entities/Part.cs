namespace Domain.Entities;

public class Part : BaseAuditEntity
{
    public Guid PartId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string? ModelNumber { get; set; }
    public string Description { get; set; } = string.Empty;

    public ICollection<MaintenancePart> MaintenanceParts { get; set; } = [];
}
