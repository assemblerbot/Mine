namespace Mine.Framework;

public class Component
{
	private Entity _entity = null!;
	public Entity Entity => _entity;

	public void SetEntity(Entity entity)
	{
		_entity = entity;
	}

	//--------------------------------------------
	// 'event' functions:
	//--------------------------------------------

	// called after ENTITY that owns this component was added to world
	//  (if the component was added to the entity already in the world, this function is NOT called)
	public virtual void AfterAddedToWorld()
	{
	}
	
	// called before ENTITY that owns this component is removed from world (disconnected or destroyed)
	public virtual void BeforeRemovedFromWorld()
	{
	}

	// called before ENTITY is disposed
	//  (all entities connected to the world are disposed in the moment when world is disposed, disconnected entities needs to be disposed by user)
	public virtual void Dispose()
	{
	}

	// called when ENTITY flags were changed
	//  (it's guaranteed that oldFlags != newFlags)
	public virtual void AfterEntityFlagsChanged(EntityFlags oldFlags, EntityFlags newFlags)
	{
	}
}