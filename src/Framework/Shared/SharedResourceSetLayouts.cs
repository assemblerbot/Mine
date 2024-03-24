using Veldrid;

namespace Mine.Framework;

public sealed class SharedResourceSetLayouts : IDisposable
{
	private class ResourceLayoutCombined
	{
	}

	// layout per kind
	public ResourceLayout WorldMatrix = null!;
	public ResourceLayout ViewProjectionMatrix = null!;

	public void Init()
	{
		WorldMatrix          = Engine.Graphics.Factory.CreateResourceLayout(ShaderResourceSetWorldMatrix.GetResourceLayoutDescription());
		ViewProjectionMatrix = Engine.Graphics.Factory.CreateResourceLayout(ShaderResourceSetViewProjectionMatrix.GetResourceLayoutDescription());
	}

	public void Dispose()
	{
		WorldMatrix?.Dispose();
		ViewProjectionMatrix?.Dispose();
	}
}