namespace Mine.Framework;

public class AmbientLightComponent : LightComponent<ShaderResourceSetAmbientLight>
{
	public Color4FloatRGBA Color = Color4FloatRGBA.White;
	
	public AmbientLightComponent(ulong renderMask) : base(renderMask)
	{
	}
}