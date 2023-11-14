using ImGuiNET;
using RedHerring.Studio.Models;

namespace RedHerring.Studio.Tools;

[Tool(ToolName)]
public class ToolPlugins : ATool
{
	public const       string    ToolName = "Plugins";
	protected override string    Name => ToolName;

	public ToolPlugins(StudioModel studioModel) : base(studioModel)
	{
	}

	public ToolPlugins(StudioModel studioModel, int uniqueId) : base(studioModel, uniqueId)
	{
	}
	
	public override void Update(out bool finished)
	{
		finished = UpdateUI();
	}

	private bool UpdateUI()
	{
		bool isOpen = true;
		if (ImGui.Begin(NameWithSalt, ref isOpen))
		{
			//_inspector.Update();
			ImGui.End();
		}

		return !isOpen;
	}
}