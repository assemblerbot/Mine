namespace Mine.Framework;

public class PointLightComponent : LightComponent<ShaderResourceSetPointLight>
{
	public PointLightComponent(ulong renderMask) : base(renderMask)
	{
	}
}