namespace Mine.Studio;

public sealed class StudioAssetDatabase
{
	private readonly Dictionary<string, StudioAssetDatabaseItem> _items = new();
	private          bool                                        _dirty = false;
	public           bool                                        IsDirty => _dirty;

	public StudioAssetDatabaseItem? this[string key]
	{
		get => _items.TryGetValue(key, out StudioAssetDatabaseItem? path) ? path : null;
		set
		{
			if (value is null)
			{
				_items.Remove(key);
			}
			else
			{
				_items[key] = value;
			}

			_dirty = true;
		}
	}

	#region IO Manipulation
	public void Save(ProjectSettings projectSettings)
	{
		string path = Path.Join(projectSettings.AbsoluteScriptsPath, projectSettings.AssetDatabaseSourcePath);

		try
		{
			using FileStream   stream = new(path, FileMode.Create);
			using StreamWriter writer = new(stream);

			writer.WriteLine("// this file is generated in Mine Studio");
			writer.WriteLine("using Mine.Framework;");
			writer.WriteLine($"namespace {projectSettings.AssetDatabaseNamespace};");
			writer.WriteLine($"public static class {projectSettings.AssetDatabaseClass}");
			writer.WriteLine("{");

			foreach (StudioAssetDatabaseItem item in _items.Values)
			{
				if (item.Field == null)
				{
					continue;
				}

				writer.WriteLine($"	public static {item.ReferenceType} {item.Field} = new(@\"{item.Path}\");");
			}

			writer.WriteLine();

			writer.WriteLine("	public static Dictionary<string, Reference> Assets = new() {");
			foreach (StudioAssetDatabaseItem item in _items.Values)
			{
				writer.WriteLine($"		{{\"{item.Guid}\",new {item.ReferenceType}(@\"{item.Path}\")}},");
			}

			writer.WriteLine("	};");

			writer.WriteLine("}");

			writer.Flush();
			stream.Flush();
		}
		catch (Exception e)
		{
			ConsoleViewModel.LogException("Exception occured while writing AssetDatabase: " + e);
		}

		_dirty = false;
	}
	#endregion
}