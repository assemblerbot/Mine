namespace Mine.Framework;

public class AmbientLightComponent : LightComponent<ShaderResourceSetAmbientLight>
{
	public AmbientLightComponent(ulong renderMask) : base(renderMask)
	{
	}
}