using System.Text.Json;
using Migration;
using RedHerring.Studio.Models.ViewModels.Console;

namespace RedHerring.Studio.Models.Project.FileSystem;

public class ProjectScriptFileNode : ProjectNode
{
	private const string _scriptHeader = "//Meta"; 
	
	[Serializable]
	private class FileId
	{
		public string Guid { get; set; }
		public string Type { get; set; } // TODO - constants or something
	}

	public ProjectScriptFileNode(string name, string path, string relativePath) : base(name, path, relativePath, false)
	{
		SetNodeType(ProjectNodeType.ScriptFile);
	}

	public override void InitMeta(MigrationManager migrationManager, CancellationToken cancellationToken)
	{
		string guid = RelativePath;
		
		// try to parse file header
		using (FileStream file = new(Path, FileMode.Open))
		{
			if (file.Length > _scriptHeader.Length + 1)
			{
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

				if(equals)
				{
					byte[] content = new byte[file.Length - _scriptHeader.Length];
					if (file.ReadAtLeast(content, 1, false) != content.Length)
					{
						ConsoleViewModel.LogWarning($"File {Path} cannot be read!");
					}

					int endOfLine = Array.FindIndex(content, x => x == '\n' || x == '\r');
					if (endOfLine != -1)
					{
						Array.Resize(ref content, endOfLine);
					}

					FileId? fileId = JsonSerializer.Deserialize<FileId>(content);
					if (fileId != null)
					{
						guid = fileId.Guid;
						SetNodeType(ProjectNodeType.ScriptDefinitionTemplate);
					}
				}
			}
		}

		Meta = new Metadata
		       {
			       Guid = guid,
			       Hash = "",
		       };
	}

	public override void TraverseRecursive(Action<ProjectNode> process, TraverseFlags flags, CancellationToken cancellationToken)
	{
		if ((flags & TraverseFlags.Files) != 0)
		{
			process(this);
		}
	}
}