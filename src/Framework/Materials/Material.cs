namespace Mine.Framework;

public abstract class Material
{
	public readonly Pass[] Passes;

	public Material(params Pass[] passes)
	{
		Passes = passes;
	}

	public void SetShaderConstant(string name, Vector4Float value)
	{
		// TODO
	}
}