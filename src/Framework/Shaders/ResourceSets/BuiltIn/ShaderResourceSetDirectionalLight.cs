using Veldrid;

namespace Mine.Framework;

// float4 Color
// float3 Direction
public sealed class ShaderResourceSetDirectionalLight : ShaderResourceSet
{
	public const string BufferName = "DirectionalLightConstants";
	
	public ShaderResourceSetDirectionalLight()
		: base(new ShaderResourceUniformBuffer(BufferName, Color4FloatRGBA.SizeInBytes + Vector4Float.SizeInBytes))
	{
	}

	public void Set(Color4FloatRGBA color, Vector3Float direction)
	{
		ShaderResourceUniformBuffer uniformBuffer = (ShaderResourceUniformBuffer)Resources[0];
		Engine.Graphics.Device.UpdateBuffer(uniformBuffer.GetOrCreateBuffer(), 0, (color, new Vector4Float(direction, 0)));
	}

	public void SetEmpty()
	{
		Set(Color4FloatRGBA.Black, Vector3Float.Zero);
	}
	
	public static ResourceLayoutDescription GetResourceLayoutDescription(ShaderStages stages)
	{
		return new ResourceLayoutDescription(
			new ResourceLayoutElementDescription(BufferName, ResourceKind.UniformBuffer, stages)
		);
	}
}