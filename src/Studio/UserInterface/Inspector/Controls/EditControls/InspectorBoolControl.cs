﻿using ImGuiNET;
using Gui = ImGuiNET.ImGui;

namespace Mine.Studio;

public class InspectorBoolControl : InspectorSingleInputControl<bool>
{
	public InspectorBoolControl(IInspector inspector, string id) : base(inspector, id)
	{
	}

	protected override bool InputControl(bool makeItemActive)
	{
		Gui.BeginDisabled(_isReadOnly);
		bool submit = Gui.Checkbox(LabelId, ref Value);
		Gui.EndDisabled();

		return submit || makeItemActive;
	}
}