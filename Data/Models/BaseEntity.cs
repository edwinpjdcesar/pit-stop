namespace Data.Models;

public abstract class BaseEntity<T> : BaseAuditEntity, IEntity<T> where T : struct
{
    public T Id { get; set; }

    protected BaseEntity() { }

    protected BaseEntity(T id)
    {
        Id = id;
    }
}
