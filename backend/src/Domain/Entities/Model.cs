namespace Domain.Entities;

public class Model : BaseAuditEntity
{
    public int ModelId { get; set; }

    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public int MakeId { get; set; }
    public Make Make { get; set; } = null!;
}
