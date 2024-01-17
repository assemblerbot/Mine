using System.Reflection;
using Mine.ImGuiPlugin;
using Mine.Studio;
using Gui = ImGuiNET.ImGui;

namespace RedHerring.Studio.UserInterface;

public sealed class InspectorReferenceControl : InspectorEditControl<StudioReference>
{
	private string  _buttonLabelId;
	private string  _pickButtonLabelId;
	private string? _currentGuid = null;

	private StudioReference? _newValue  = null;
	private bool             _wasCommit = false;

	public InspectorReferenceControl(IInspector inspector, string id) : base(inspector, id)
	{
		_pickButtonLabelId = $"{FontAwesome6.CircleDot}##{Id}.pickbutton";
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

		// select in project button
		float showReferenceButtonPosition = Gui.GetCursorPosX();
		Gui.SetNextItemAllowOverlap();
		if (Gui.Button(_buttonLabelId))
		{
			// TODO - select in project
		}

		// pick button
		Gui.SameLine();
		float nextPosition = Gui.GetCursorPosX();
		Gui.SetCursorPosX(showReferenceButtonPosition);
		if (Gui.Button(_pickButtonLabelId))
		{
			if (Value != null)
			{
				(_inspector as IInspectorStudio)?.OpenReferencePopup(
					Value!,
					node =>
					{
						_newValue      = Value.CreateCopy();
						_newValue.Guid = node?.Meta?.Guid;
						_wasCommit     = true;
					}
				);
			}
		}

		
		if (!string.IsNullOrEmpty(Label))
		{
			Gui.SameLine();
			Gui.SetCursorPosX(nextPosition);
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
		_buttonLabelId = $"{FontAwesome6.CircleDot}  {path} ({Value?.Name ?? "null"})##{Id}.button";
	}
}