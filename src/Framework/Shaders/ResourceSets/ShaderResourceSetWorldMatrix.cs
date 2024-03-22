using Veldrid;

namespace Mine.Framework;

public sealed class ShaderResourceSetWorldMatrix : ShaderResourceSet
{
	public ShaderResourceSetWorldMatrix()
		: base(new ShaderResourceUniformBuffer("WorldMatrixConstants", ShaderStages.Vertex, Matrix4x4Float.SizeInBytes)) // TODO
	{
	}

	public void Set(Matrix4x4Float localToWorld)
	{
		ShaderResourceUniformBuffer uniformBuffer = (ShaderResourceUniformBuffer)Resources[0];
		Engine.Graphics.Device.UpdateBuffer(uniformBuffer.GetOrCreateBuffer(), 0, localToWorld);
	}

	public static ResourceLayoutDescription GetResourceLayoutDescription()
	{
		return new ResourceLayoutDescription(
			new ResourceLayoutElementDescription("WorldMatrixConstants", ResourceKind.UniformBuffer, ShaderStages.Vertex)
		);
	}
}