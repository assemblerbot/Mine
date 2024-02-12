namespace Mine.Framework;

public class CameraComponent : Component, IRenderable
{
	public int  GetRenderOrder()
	{
		return 0;
	}

	public void Render()
	{
		
	}

	public void WindowResized(Vector2Int size)
	{
		
	}
}