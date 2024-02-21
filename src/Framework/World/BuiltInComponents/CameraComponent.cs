namespace Mine.Framework;

public class CameraComponent : Component, IRenderable
{
	public int RenderOrder => 0;

	public override void AfterAddedToWorld()
	{
		Engine.World.RegisterRenderable(this);
	}

	public override void BeforeRemovedFromWorld()
	{
		Engine.World.UnregisterRenderable(this);
	}

	public void Render()
	{
		if (!Entity.ActiveInHierarchy)
		{
			return;
		}
		
		
	}

	public void WindowResized(Vector2Int size)
	{
		
	}
}