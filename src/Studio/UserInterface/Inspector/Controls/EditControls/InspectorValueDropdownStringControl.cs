﻿using Gui = ImGuiNET.ImGui;

namespace Mine.Studio;

public class InspectorValueDropdownStringControl : InspectorValueDropdownControl<string>
{
	public InspectorValueDropdownStringControl(IInspector inspector, string id) : base(inspector, id)
	{
	}

	protected override bool InputControl(bool makeItemActive)
	{
		int  localValue = Array.FindIndex(_items, x => x == Value);
		bool submit     = Gui.Combo(LabelId, ref localValue, _items, _items.Length);

		if (localValue != -1)
		{
			Value = _items[localValue];
		}

		return submit || makeItemActive;
	}
}