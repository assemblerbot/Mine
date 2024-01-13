using System.Reflection;
using Mine.ImGuiPlugin;
using Mine.Studio;
using Gui = ImGuiNET.ImGui;

namespace RedHerring.Studio.UserInterface;

public sealed class InspectorReferenceControl : InspectorEditControl<StudioReference>
{
	private string                      _buttonLabelId;
	private string                      _popupLabelId;
	//private readonly Func<StudioReference, bool> _popupUpdateUI;

	// called from reflection
	public InspectorReferenceControl(IInspectorCommandTarget commandTarget, string id) : base(commandTarget, id)
	{
		_popupLabelId  = id + ".popup";
	}

	public override void InitFromSource(object? sourceOwner, object source, FieldInfo? sourceField = null, int sourceIndex = -1)
	{
		base.InitFromSource(sourceOwner, source, sourceField, sourceIndex);
		_buttonLabelId = $"{FontAwesome6.CircleDot} None ({Value?.Name ?? "null"})##{Id}.button";
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
		if (Gui.Button(_buttonLabelId))
		{
			if (Value != null)
			{
				Gui.OpenPopup(_popupLabelId);
			}
		}

		if (!string.IsNullOrEmpty(Label))
		{
			Gui.SameLine();
			Gui.Text(Label);
		}

		bool wasSubmit = false;
		if (Gui.BeginPopup(_popupLabelId))
		{
			//wasSubmit = _popupUpdateUI(Value!);
			Gui.EndPopup();
		}

		return wasSubmit; // submit after dropped something or after picked reference object
	}
}