using Veldrid;

namespace Mine.Framework;

public sealed class ShaderResourceSetViewProjectionMatrix : ShaderResourceSet
{
	private const string       _bufferName   = "ViewProjectionMatrixConstants";
	private const ShaderStages _shaderStages = ShaderStages.Vertex;
	
	public ShaderResourceSetViewProjectionMatrix()
		: base(new ShaderResourceUniformBuffer(_bufferName, _shaderStages, Matrix4x4Float.SizeInBytes))
	{
	}

	public void Set(Matrix4x4Float viewProjection)
	{
		ShaderResourceUniformBuffer uniformBuffer = (ShaderResourceUniformBuffer)Resources[0];
		Engine.Graphics.Device.UpdateBuffer(uniformBuffer.GetOrCreateBuffer(), 0, viewProjection);
	}

	public static ResourceLayoutDescription GetResourceLayoutDescription()
	{
		return new ResourceLayoutDescription(
			new ResourceLayoutElementDescription(_bufferName, ResourceKind.UniformBuffer, _shaderStages)
		);
	}
}