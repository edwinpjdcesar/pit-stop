namespace Data.Models;

public class Make : BaseEntity<int>
{
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public ICollection<Model> Models { get; set; } = [];
}
