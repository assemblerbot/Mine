namespace Mine.Framework;

public class Component
{
	private Entity _entity = null!;
	public Entity Entity => _entity;

	public void SetEntity(Entity entity)
	{
		_entity = entity;
	}

	// 'event' functions
	public virtual void AfterAddedToWorld()
	{
	}
	
	public virtual void BeforeRemovedFromWorld()
	{
	}

	public virtual void Dispose()
	{
	}

	public virtual void AfterEntityFlagsChanged(EntityFlags oldFlags, EntityFlags newFlags)
	{
	}
}