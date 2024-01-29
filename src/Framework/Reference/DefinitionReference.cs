using System.Text.Json;

namespace Mine.Framework;

[Serializable]
public sealed class DefinitionReference<T> : Reference where T : Definition
{
	private List<T>? _definitions = null;
	public  List<T>? Definitions => _definitions;

	public override bool Load()
	{
		try
		{
			byte[]? json = Engine.Resources.ReadResource(Path);
			
			JsonSerializerOptions options = new()
			                                {
				                                IncludeFields = true
			                                };

			_definitions = JsonSerializer.Deserialize<List<T>>(json, options);
		}
		catch (Exception e)
		{
			return false;
		}

		return true;
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