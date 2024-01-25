using System.Text.Json;
using Migration;

namespace RedHerring.Studio.Models.Project.FileSystem;

[Serializable, SerializedClassId("c413809d-cfeb-4083-aefb-79c1850f20e0")]
public sealed class ProjectScriptFileHeader
{
	private const string _scriptHeader = "//Meta";

	public string Guid;
	public string Type;
	public int    Version;
	
	public ProjectScriptFileHeader(string guid, string type, int version)
	{
		Guid    = guid;
		Type    = type;
		Version = version;
	}

	public static ProjectScriptFileHeader? CreateFromFile(string path)
	{
		try
		{
			using FileStream file = new(path, FileMode.Open);

			if (file.Length <= _scriptHeader.Length + 1)
			{
				return null;
			}

			byte[] header = new byte[_scriptHeader.Length];
			file.ReadExactly(header, 0, _scriptHeader.Length);

			bool equals = true;
			for (int i = 0; i < _scriptHeader.Length; ++i)
			{
				if (header[i] != _scriptHeader[i])
				{
					equals = false;
					break;
				}
			}

			if (!equals)
			{
				return null;
			}

			byte[] content = new byte[file.Length - _scriptHeader.Length];
			if (file.ReadAtLeast(content, 1, false) != content.Length)
			{
				return null;
			}

			int endOfLine = Array.FindIndex(content, x => x == '\n' || x == '\r');
			if (endOfLine != -1)
			{
				Array.Resize(ref content, endOfLine);
			}

			return JsonSerializer.Deserialize<ProjectScriptFileHeader>(content, new JsonSerializerOptions {IncludeFields = true});
		}
		catch (Exception e)
		{
			return null;
		}
	}

	public string ToHeaderString()
	{
		return $"{_scriptHeader}{JsonSerializer.Serialize(this, new JsonSerializerOptions{IncludeFields = true})}";
	}

	public static ProjectScriptFileHeader? ReadFromStringLine(string line)
	{
		if (!line.StartsWith(_scriptHeader))
		{
			return null;
		}

		string json = line.Substring(_scriptHeader.Length);
		return JsonSerializer.Deserialize<ProjectScriptFileHeader>(json, new JsonSerializerOptions{IncludeFields = true});
	}
}

#region Migration
[MigratableInterface(typeof(ProjectScriptFileHeader))]
public interface IProjectScriptFileHeaderMigratable;
    
[Serializable, LatestVersion(typeof(ProjectScriptFileHeader))]
public class ProjectScriptFileHeader_000 : IProjectScriptFileHeaderMigratable
{
	public string Guid;
	public string Type;
	public int    Version;
}
#endregion