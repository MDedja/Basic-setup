namespace Entities.Base;

public abstract class Entity : IEntity
{
    public Entity(Guid id) => Id = id;

    public Guid Id { get; private set; }
    
}
