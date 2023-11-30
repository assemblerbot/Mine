using System.Numerics;
using ImGuiNET;
using RedHerring.Studio.Commands;
using RedHerring.Studio.UserInterface;
using Gui = ImGuiNET.ImGui;

namespace Mine.Studio;

public sealed class ObjectDialog
{
	private readonly string    _titleId;
	private readonly Inspector _inspector;
	private readonly Action?   _onClose;

	public ObjectDialog(string titleId, CommandHistory history, object sourceObject, Action? onClose = null)
	{
		_titleId   = titleId;
		_inspector = new Inspector(history);
		_inspector.Init(sourceObject);
		_onClose = onClose;
	}

	public void Open()
	{
		Gui.OpenPopup(_titleId);
	}

	public void Update()
	{
		Vector2 center = Gui.GetMainViewport().GetCenter();
		Gui.SetNextWindowPos(center, ImGuiCond.Appearing, new Vector2(0.5f, 0.5f));
		Gui.SetNextWindowSizeConstraints(new Vector2(400, 200), new Vector2(2000, 2000));

		bool unusedIsOpen = true;
		if (Gui.BeginPopupModal(_titleId, ref unusedIsOpen))
		{
			_inspector.Update();
			Gui.EndPopup();
		}
		
		if(!unusedIsOpen)
		{
			_onClose?.Invoke();
		}
	}
}