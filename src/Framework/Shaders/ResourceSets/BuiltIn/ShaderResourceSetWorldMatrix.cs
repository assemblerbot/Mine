using Veldrid;

namespace Mine.Framework;

public sealed class ShaderResourceSetWorldMatrix : ShaderResourceSet
{
	public const string BufferName = "WorldMatrixConstants";
	
	public ShaderResourceSetWorldMatrix()
		: base(new ShaderResourceUniformBuffer(BufferName, Matrix4x4Float.SizeInBytes))
	{
	}

	public void Set(Matrix4x4Float localToWorld)
	{
		ShaderResourceUniformBuffer uniformBuffer = (ShaderResourceUniformBuffer)Resources[0];
		Engine.Graphics.Device.UpdateBuffer(uniformBuffer.GetOrCreateBuffer(), 0, localToWorld);
	}

	public static ResourceLayoutDescription GetResourceLayoutDescription(ShaderStages stages)
	{
		return new ResourceLayoutDescription(
			new ResourceLayoutElementDescription(BufferName, ResourceKind.UniformBuffer, stages)
		);
	}
}