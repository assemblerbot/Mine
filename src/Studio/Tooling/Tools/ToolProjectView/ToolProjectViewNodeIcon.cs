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
			ProjectNodeType.AssetScene => FontAwesome6.Cube,
			//ProjectNodeType.AssetDefinition => FontAwesome6.Table,
			
			ProjectNodeType.ScriptFile => FontAwesome6.FileCode,
			ProjectNodeType.ScriptFolder => FontAwesome6.Folder,
			//ProjectNodeType.ScriptDefinition => FontAwesome6.TableColumns,

			_ => FontAwesome6.Ban
		};
	}
}