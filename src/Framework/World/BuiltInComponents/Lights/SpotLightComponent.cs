namespace Mine.Framework;

public class SpotLightComponent : LightComponent<ShaderResourceSetSpotLight>
{
	public SpotLightComponent(ulong renderMask) : base(renderMask)
	{
	}
}