namespace Mine.Framework;

public interface IRenderer
{
	Entity Entity      { get; }
	
	int    RenderOrder { get; }
	ulong  RenderMask  { get; }

	void Render();
	void WindowResized(Vector2Int size);
}