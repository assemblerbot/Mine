using Mine.ImGuiPlugin;
using Gui = ImGuiNET.ImGui;

namespace Mine.Studio;

[Tool(ToolName)]
public sealed class ToolConsole : Tool
{
	public const       string ToolName = FontAwesome6.Terminal + " Console";
	protected override string Name => ToolName;

	public ToolConsole(StudioModel studioModel, int uniqueId) : base(studioModel, uniqueId)
	{
	}

	public override void Update(out bool finished)
	{
		finished = UpdateUI();
	}
	
	private bool UpdateUI()
	{
		bool isOpen = true;
		if (Gui.Begin(NameId, ref isOpen))
		{
			for(int i=0; i<StudioModel.Console.Count; ++i)
			{
				ConsoleItem item = StudioModel.Console[i];
				Gui.TextColored(item.Type.ToColor(), item.Message);
			}
			
			Gui.End();
		}

		return !isOpen;
	}
}