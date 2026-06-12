namespace Domain.Entities;

public class Make : BaseAuditEntity
{
    public int MakeId { get; set; }

    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public ICollection<Model> Models { get; set; } = [];
}
