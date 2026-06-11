namespace Data.Models;

public class Part : BaseEntity<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string? ModelNumber { get; set; }

    public ICollection<MaintenancePart> MaintenanceParts { get; set; } = [];
}
