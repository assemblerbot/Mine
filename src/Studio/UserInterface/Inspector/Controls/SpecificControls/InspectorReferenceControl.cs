using System.Reflection;
using ImGuiNET;
using Mine.Framework;
using Mine.ImGuiPlugin;
using Mine.Studio;
using Gui = ImGuiNET.ImGui;

namespace RedHerring.Studio.UserInterface;

public sealed class InspectorReferenceControl : InspectorEditControl<DefinitionAssetValueAssetReference>
{
	private string _buttonLabelId;
	
	public InspectorReferenceControl(IInspectorCommandTarget commandTarget, string id) : base(commandTarget, id)
	{
	}

	public override void InitFromSource(object? sourceOwner, object source, FieldInfo? sourceField = null, int sourceIndex = -1)
	{
		base.InitFromSource(sourceOwner, source, sourceField, sourceIndex);
		_buttonLabelId = $"{FontAwesome6.CircleDot} None ({Value?.Name ?? "null"})";
	}

	public sealed override void Update() // TODO - if not changed, just derive from SingleValue control
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
		Gui.Button(_buttonLabelId);
		Gui.SameLine();
		Gui.Text("Asset Reference");
		return false; // submit after dropped something or after picked reference object
	}
}