namespace Mine.Framework;

public interface IRenderable
{
	Entity Entity { get; }
	int RenderOrder { get; }
	
	void Render();
	void WindowResized(Vector2Int size);
}