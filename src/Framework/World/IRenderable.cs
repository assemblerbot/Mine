namespace GameToolkit.Framework;

public interface IRenderable
{
	int RenderOrder { get; }
	void Render();
}