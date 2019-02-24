/// <summary>
/// Base class for the nodes for the entity
/// </summary>
public abstract class EntityNode
{
    protected Entity _entity;

    private void Init(Entity entity)
    {
        _entity = entity;
    }
}