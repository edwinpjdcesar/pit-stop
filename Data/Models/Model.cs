namespace Data.Models;

public class Model : BaseEntity<int>
{
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public int MakeId { get; set; }
    public Make Make { get; set; } = null!;
}
