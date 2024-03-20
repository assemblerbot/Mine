using Veldrid;

namespace Mine.Framework;

public sealed class ShaderResourceSetViewProjectionMatrix : ShaderResourceSet
{
	public ShaderResourceSetViewProjectionMatrix()
		: base(new ShaderResourceUniformBuffer("ViewProjectionTransform", ShaderStages.Vertex, Matrix4x4Float.SizeInBytes)) // TODO
	{
	}

	public override void Update()
	{
	}

	public static ResourceLayoutDescription GetResourceLayoutDescription()
	{
		return new ResourceLayoutDescription(
			new ResourceLayoutElementDescription("ViewProjectionTransform", ResourceKind.UniformBuffer, ShaderStages.Vertex)
		);
	}
}