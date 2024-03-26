namespace Mine.Framework;

public sealed class SharedResourceSetLayouts : IDisposable
{
	// matrices
	public readonly MultiStageResourceLayout WorldMatrix          = new(ShaderResourceSetWorldMatrix.GetResourceLayoutDescription);
	public readonly MultiStageResourceLayout ViewProjectionMatrix = new(ShaderResourceSetViewProjectionMatrix.GetResourceLayoutDescription);
	
	// lights
	public readonly MultiStageResourceLayout AmbientLight     = new(ShaderResourceSetAmbientLight.GetResourceLayoutDescription);
	public readonly MultiStageResourceLayout DirectionalLight = new(ShaderResourceSetDirectionalLight.GetResourceLayoutDescription);

	public void Dispose()
	{
		WorldMatrix.Dispose();
		ViewProjectionMatrix.Dispose();
		AmbientLight.Dispose();
		DirectionalLight.Dispose();
	}
}