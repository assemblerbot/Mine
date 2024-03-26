namespace Mine.Framework;

public class DirectionalLightComponent : LightComponent<ShaderResourceSetDirectionalLight>
{
	public Color4FloatRGBA Color = Color4FloatRGBA.White;
	
	public DirectionalLightComponent(ulong renderMask) : base(renderMask)
	{
	}
}