using Veldrid;

namespace Mine.Framework;

public sealed class ShaderResourceSetViewProjectionMatrix : ShaderResourceSet
{
	public ShaderResourceSetViewProjectionMatrix()
		: base(new ShaderResourceUniformBuffer("ViewProjectionMatrixConstants", ShaderStages.Vertex, Matrix4x4Float.SizeInBytes)) // TODO
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
			new ResourceLayoutElementDescription("ViewProjectionMatrixConstants", ResourceKind.UniformBuffer, ShaderStages.Vertex)
		);
	}
}