using Veldrid;

namespace Mine.Framework;

public sealed class ShaderResourceSetViewProjectionMatrix : ShaderResourceSet
{
	public const string BufferName = "ViewProjectionMatrixConstants";
	
	public ShaderResourceSetViewProjectionMatrix()
		: base(new ShaderResourceUniformBuffer(BufferName, Matrix4x4Float.SizeInBytes))
	{
	}

	public void Set(Matrix4x4Float viewProjection)
	{
		ShaderResourceUniformBuffer uniformBuffer = (ShaderResourceUniformBuffer)Resources[0];
		Engine.Graphics.Device.UpdateBuffer(uniformBuffer.GetOrCreateBuffer(), 0, viewProjection);
	}

	public static ResourceLayoutDescription GetResourceLayoutDescription(ShaderStages stages)
	{
		return new ResourceLayoutDescription(
			new ResourceLayoutElementDescription(BufferName, ResourceKind.UniformBuffer, stages)
		);
	}
}