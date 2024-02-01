namespace Mine.Framework;

public interface IRenderable
{
	Entity Entity { get; }

	int  GetRenderOrder();
	void Render();
	void WindowResized(Vector2Int size);
}