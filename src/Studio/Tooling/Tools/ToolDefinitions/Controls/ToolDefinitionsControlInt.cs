using ImGuiNET;
using Mine.Studio.Commands;
using RedHerring.Studio.Commands;

namespace Mine.Studio.Controls;

public sealed class ToolDefinitionsControlInt : ToolDefinitionsControl
{
	private int _value;
	
	public void UpdateUI(string labelId, DefinitionAssetValueInt assetValue, ICommandHistory commandHistory)
	{
		// TODO - need state
		int value = assetValue.Value;
		ImGui.InputInt(labelId, ref value);
		
		if (ImGui.IsItemDeactivatedAfterEdit())
		{
			commandHistory.Commit(new DefinitionCommandSetInt(assetValue, value));
		}
	}
}