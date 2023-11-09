namespace GameToolkit.Framework;

public interface IRenderable
{
	int  GetRenderOrder();
	void Render();
	void WindowResized(Int2 size);
}