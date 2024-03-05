namespace Mine.Framework;

public sealed class Shader
{
	public readonly AssetReference Reference;
	public readonly ShaderConstant[] Constants;

	public Shader(AssetReference reference, ShaderConstant[] constants)
	{
		Reference = reference;
		Constants = constants;
	}
}