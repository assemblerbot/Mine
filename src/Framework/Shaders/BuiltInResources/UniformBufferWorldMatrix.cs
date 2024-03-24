using Veldrid;

namespace Mine.Framework.Shaders.BuiltInResources;

public sealed class UniformBufferWorldMatrix : UniformBuffer
{
	public const string BufferName = "WorldMatrixConstants";

	public          uint              SizeInBytes       => Matrix4x4Float.SizeInBytes;
	public override BufferDescription BufferDescription => new (SizeInBytes, BufferUsage.UniformBuffer); 

	public void Set(Matrix4x4Float localToWorld)
	{
		Engine.Graphics.Device.UpdateBuffer(Buffer, 0, localToWorld);
	}

	public static ResourceLayoutDescription GetResourceLayoutDescription(ShaderStages stages)
	{
		return new ResourceLayoutDescription(
			new ResourceLayoutElementDescription(BufferName, ResourceKind.UniformBuffer, stages)
		);
	}
}