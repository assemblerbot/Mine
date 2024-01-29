using System.Text.Json;

namespace Mine.Framework;

[Serializable]
public abstract class Definition
{
	public abstract void LoadDefinitionsRecursive();
	public abstract void LoadAllRecursive();

	public static List<T>? Create<T>(string path) where T:Definition
	{
		try
		{
			byte[]? json = Engine.Resources.ReadResource(path);
			
			JsonSerializerOptions options = new()
			                                {
				                                IncludeFields = true
			                                };

			return JsonSerializer.Deserialize<List<T>>(json, options);
		}
		catch (Exception e)
		{
			Console.WriteLine("Exception: " + e);
			return null;
		}
	}
}