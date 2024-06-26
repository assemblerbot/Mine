﻿using ImGuiNET;
using Gui = ImGuiNET.ImGui;

namespace Mine.Studio;

public sealed class InspectorFloatControl : InspectorSingleInputControl<float>
{
	public InspectorFloatControl(IInspector inspector, string id) : base(inspector, id)
	{
	}

	protected override bool InputControl(bool makeItemActive)
	{
		if (makeItemActive)
		{
			Gui.SetKeyboardFocusHere();
		}

		Gui.InputFloat(LabelId, ref Value, 0.0f, 0.0f, "%.3f", _isReadOnly ? ImGuiInputTextFlags.ReadOnly : ImGuiInputTextFlags.None);
		return Gui.IsItemDeactivatedAfterEdit();
	}
}