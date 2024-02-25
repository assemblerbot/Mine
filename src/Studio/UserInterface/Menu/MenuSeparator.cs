using Gui = ImGuiNET.ImGui;

namespace Mine.Studio;

public sealed class MenuSeparator : MenuNode
{
	public MenuSeparator() : base("")
	{
	}

	public override void Update()
	{
		Gui.Separator();
	}
}