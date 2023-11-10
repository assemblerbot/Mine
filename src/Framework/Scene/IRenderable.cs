namespace GameToolkit.Framework;

public interface IRenderable
{
	int  GetRenderOrder();
	void Render();
	void WindowResized(Vector2Int size);
}