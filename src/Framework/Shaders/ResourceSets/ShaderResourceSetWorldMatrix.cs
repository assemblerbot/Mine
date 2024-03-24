using Veldrid;

namespace Mine.Framework;

public sealed class ShaderResourceSetWorldMatrix : ShaderResourceSet
{
	private const string       _bufferName   = "WorldMatrixConstants";
	private const ShaderStages _shaderStages = ShaderStages.Vertex;
	
	public ShaderResourceSetWorldMatrix()
		: base(new ShaderResourceUniformBuffer(_bufferName, _shaderStages, Matrix4x4Float.SizeInBytes))
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
			new ResourceLayoutElementDescription(_bufferName, ResourceKind.UniformBuffer, _shaderStages)
		);
	}
}