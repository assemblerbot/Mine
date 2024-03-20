namespace Mine.Framework;

public class LightComponent : Component, IRenderable
{
	private ulong _renderMask;
	public ulong      RenderMask => _renderMask;
}