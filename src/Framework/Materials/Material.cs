namespace Mine.Framework;

public abstract class Material : IDisposable
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

	public Pass? FindPassByName(string name)
	{
		int index = Array.FindIndex(Passes, x => x.Name == name);
		return index == -1 ? null : Passes[index];
	}

	public void Dispose()
	{
		foreach (Pass pass in Passes)
		{
			pass.Dispose();
		}
	}
}