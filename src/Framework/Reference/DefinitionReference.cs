namespace Mine.Framework;

[Serializable]
public sealed class DefinitionReference<T> : Reference where T : Definition
{
	private List<T>? _definitions = null;
	public  List<T>? Definitions => _definitions;

	public override bool Load()
	{
		_definitions = Definition.Create<T>(Path);
		return _definitions is not null;
	}

	public void LoadDefinitionsRecursive()
	{
		if (Load() && _definitions is not null)
		{
			foreach (T definition in _definitions)
			{
				definition.LoadDefinitionsRecursive();
			}
		}
	}

	public void LoadAllRecursive()
	{
		if (Load() && _definitions is not null)
		{
			foreach (T definition in _definitions)
			{
				definition.LoadAllRecursive();
			}
		}
	}
}