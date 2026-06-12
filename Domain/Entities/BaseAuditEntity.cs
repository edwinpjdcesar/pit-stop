namespace Domain.Entities;

public abstract class BaseAuditEntity
{
    public DateTime DateCreated { get; set; }
    public DateTime DateLastUpdated { get; set; }
    public string CreatedBy { get; set; } = string.Empty;

    protected BaseAuditEntity() { }
}
