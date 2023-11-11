namespace GameToolkit.Framework;

public interface IRenderable
{
	GameObject GameObject { get; }

	int  GetRenderOrder();
	void Render();
	void WindowResized(Vector2Int size);
}