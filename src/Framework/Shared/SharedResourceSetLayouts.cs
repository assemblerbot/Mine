namespace Mine.Framework;

public sealed class SharedResourceSetLayouts : IDisposable
{
	public MultiStageResourceLayout WorldMatrix          = new(ShaderResourceSetWorldMatrix.GetResourceLayoutDescription);
	public MultiStageResourceLayout ViewProjectionMatrix = new(ShaderResourceSetViewProjectionMatrix.GetResourceLayoutDescription);

	public void Dispose()
	{
		WorldMatrix.Dispose();
		ViewProjectionMatrix.Dispose();
	}
}