using Mine.ImGuiPlugin;
using RedHerring.Studio.Models.Project.FileSystem;

namespace RedHerring.Studio.Tools;

public static class ToolProjectViewNodeIcon
{
	public static string ToIcon(this ProjectNodeType type)
	{
		return type switch
		{
			ProjectNodeType.Uninitialized => FontAwesome6.CircleQuestion,
			
			ProjectNodeType.AssetBinary => FontAwesome6.File,
			ProjectNodeType.AssetFolder => FontAwesome6.Folder,
			ProjectNodeType.AssetImage => FontAwesome6.FileImage,
			ProjectNodeType.AssetMesh => FontAwesome6.Cube,
			ProjectNodeType.AssetDefinitionData => FontAwesome6.Table,
			
			ProjectNodeType.ScriptFile => FontAwesome6.FileCode,
			ProjectNodeType.ScriptFolder => FontAwesome6.Folder,
			ProjectNodeType.ScriptDefinitionTemplate => FontAwesome6.TableColumns,

			_ => FontAwesome6.Ban
		};
	}
}