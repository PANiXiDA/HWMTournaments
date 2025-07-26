namespace Entities.Core;

public abstract class BaseEntity<TId>
{
    public TId Id { get; set; }

    public BaseEntity(TId id)
    {
        Id = id;
    }
}
