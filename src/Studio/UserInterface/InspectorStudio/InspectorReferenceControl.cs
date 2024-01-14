using System.Reflection;
using Mine.ImGuiPlugin;
using Mine.Studio;
using Gui = ImGuiNET.ImGui;

namespace RedHerring.Studio.UserInterface;

public sealed class InspectorReferenceControl : InspectorEditControl<StudioReference>
{
	private string  _buttonLabelId;
	private string? _currentGuid = null;

	private StudioReference? _newValue  = null;
	private bool             _wasCommit = false;

	public InspectorReferenceControl(IInspector inspector, string id) : base(inspector, id)
	{
	}

	public override void InitFromSource(object? sourceOwner, object source, FieldInfo? sourceField = null, int sourceIndex = -1)
	{
		base.InitFromSource(sourceOwner, source, sourceField, sourceIndex);
		_newValue    = null;
		_wasCommit   = false;
		_currentGuid = Value?.Guid;
		RefreshButtonLabelByCurrentGuid();
	}

	public override void Update() // TODO - if not changed, just derive from SingleValue control
	{
		bool isItemActive = false;
		if(_multipleValues)
		{
			if (!GuiMultiEditButton())
			{
				return;
			}

			_multipleValues = false;
			isItemActive    = true;
		}

		BeginReadOnlyStyle();
		bool submit = InputControl(isItemActive);
		EndReadOnlyStyle();

		SubmitOrUpdateValue(submit, isItemActive || Gui.IsItemActive());
	}

	protected bool InputControl(bool makeItemActive)
	{
		if (_currentGuid != Value?.Guid)
		{
			_currentGuid = Value?.Guid;
			RefreshButtonLabelByCurrentGuid();
		}

		if (Gui.Button(_buttonLabelId))
		{
			if (Value != null)
			{
				(_inspector as IInspectorStudio)?.OpenReferencePopup(
					Value!,
					node =>
					{
						_newValue      = Value.CreateCopy();
						_newValue.Guid = node?.Meta?.Guid;
						_wasCommit  = true;
					}
				);
			}
		}
		
		// const int referencePickButtonWidth = 40;
		// Gui.SameLine();
		// Gui.SetCursorPosX(Gui.GetCursorPosX() - referencePickButtonWidth);
		// Gui.SetNextItemAllowOverlap();
		// Gui.SetNextItemWidth(referencePickButtonWidth);
		// Gui.Button(FontAwesome6.Anchor);
		

		if (!string.IsNullOrEmpty(Label))
		{
			Gui.SameLine();
			Gui.Text(Label);
		}

		if (_wasCommit)
		{
			Value      = _newValue;
			_wasCommit = false;
			return true;
		}

		return false;
	}

	private void RefreshButtonLabelByCurrentGuid()
	{
		string? path = _currentGuid == null ? "null" : (_inspector as IInspectorStudio)?.ProjectNodeGuidToName(_currentGuid) ?? _currentGuid; 
		_buttonLabelId = $"{FontAwesome6.CircleDot} {path} ({Value?.Name ?? "null"})##{Id}.button";
	}
}