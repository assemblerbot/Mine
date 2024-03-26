namespace Mine.Framework;

public class DirectionalLightComponent : LightComponent<ShaderResourceSetDirectionalLight>
{
	public DirectionalLightComponent(ulong renderMask) : base(renderMask)
	{
	}
}