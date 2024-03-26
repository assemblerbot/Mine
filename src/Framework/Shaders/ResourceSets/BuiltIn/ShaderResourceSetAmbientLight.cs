using Veldrid;

namespace Mine.Framework;

public sealed class ShaderResourceSetAmbientLight : ShaderResourceSet
{
	public const string BufferName = "AmbientLightConstants";
	
	public ShaderResourceSetAmbientLight()
		: base(new ShaderResourceUniformBuffer(BufferName, Color4FloatRGBA.SizeInBytes))
	{
	}

	public void Set(Color4FloatRGBA color)
	{
		ShaderResourceUniformBuffer uniformBuffer = (ShaderResourceUniformBuffer)Resources[0];
		Engine.Graphics.Device.UpdateBuffer(uniformBuffer.GetOrCreateBuffer(), 0, color);
	}

	public void SetEmpty()
	{
		Set(Color4FloatRGBA.Black);
	}
	
	public static ResourceLayoutDescription GetResourceLayoutDescription(ShaderStages stages)
	{
		return new ResourceLayoutDescription(
			new ResourceLayoutElementDescription(BufferName, ResourceKind.UniformBuffer, stages)
		);
	}
}