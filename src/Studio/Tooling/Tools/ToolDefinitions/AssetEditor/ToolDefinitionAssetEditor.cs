using System.Numerics;
using ImGuiNET;
using Mine.ImGuiPlugin;
using RedHerring.Studio.Commands;
using RedHerring.Studio.UserInterface;

namespace Mine.Studio;

public sealed class ToolDefinitionAssetEditor : IInspectorCommandTarget
{
	private readonly DefinitionAsset              _definitionAsset;
	private readonly CommandHistoryWithChange     _commandHistory = new();
	private readonly List<List<InspectorControl>> _controls       = new();
	
	private int _uniqueIdGenerator = 0;
	private int UniqueId => ++_uniqueIdGenerator;

	private readonly string _buttonApplyTemplateChangesNameId;
	
	public ToolDefinitionAssetEditor(DefinitionAsset asset)
	{
		_definitionAsset                  = asset;
		_buttonApplyTemplateChangesNameId = $"Apply changes##apply_asset";
		RebuildControls();
	}

	public void Commit(Command command)
	{
		_commandHistory.Commit(command);
	}

	public void Update()
	{
		bool wasChange = _commandHistory.WasChange;
		if (wasChange)
		{
			// TODO - move to style
			ImGui.PushStyleColor(ImGuiCol.Button,        new Vector4(0.2f, 0.5f, 0.2f, 1f));
			ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.3f, 0.5f, 0.3f, 1f));
			ImGui.PushStyleColor(ImGuiCol.ButtonActive,  new Vector4(0.4f, 0.5f, 0.4f, 1f));
		}
		else
		{
			ImGui.BeginDisabled();
		}

		if (ImGui.Button(_buttonApplyTemplateChangesNameId))
		{
			// if (UpdateTemplateFile())
			// {
			// 	_definitionTemplateHistory!.ResetChange();
			// }
		}

		if (wasChange)
		{
			ImGui.PopStyleColor(3);
		}
		else
		{
			ImGui.EndDisabled();
		}
		
		ImGuiTableFlags flags =
			ImGuiTableFlags.Borders     |
			ImGuiTableFlags.Resizable   |
			ImGuiTableFlags.Sortable    |
			ImGuiTableFlags.Reorderable |
			ImGuiTableFlags.RowBg       |
			ImGuiTableFlags.ScrollX     |
			ImGuiTableFlags.ScrollY;
		
		IReadOnlyList<DefinitionTemplateField?> fields = _definitionAsset.Template.Fields;
		if (ImGui.BeginTable("definition_asset_editor_table", 1 + fields.Count, flags))
		{
			ImGui.TableSetupColumn(" ", ImGuiTableColumnFlags.WidthFixed | ImGuiTableColumnFlags.NoResize | ImGuiTableColumnFlags.NoReorder, 20f);
			
			for (int col = 0; col < fields.Count; ++col)
			{
				ImGui.TableSetupColumn(fields[col]?.Name ?? "null", ImGuiTableColumnFlags.WidthStretch);
			}
			ImGui.TableSetupScrollFreeze(0, 1);
			ImGui.TableHeadersRow();

			int removeRowRequested = -1;
			for (int rowIndex = 0; rowIndex < _definitionAsset.Rows.Count; ++rowIndex)
			{
				DefinitionAssetRow row = _definitionAsset.Rows[rowIndex];
				
				ImGui.TableNextRow();
				
				ImGui.TableSetColumnIndex(0);
				if (ImGui.SmallButton(FontAwesome6.Xmark)) // TODO - unique id: refactor rows of controls from list to class and then create unique id per row
				{
					removeRowRequested = rowIndex;
				}

				for (int columnIndex = 0; columnIndex < fields.Count; ++columnIndex)
				{
					ImGui.TableSetColumnIndex(columnIndex + 1);
					_controls[rowIndex][columnIndex].Update();
				}
			}

			// last row is empty, with just a + button
			ImGui.TableNextRow();
			ImGui.TableSetColumnIndex(0);
			if (ImGui.SmallButton(FontAwesome6.Plus))
			{
				AddRow();
			}

			if (removeRowRequested != -1)
			{
				RemoveRow(removeRowRequested);
			}

			ImGui.EndTable();
		}
	}

	private void RebuildControls()
	{
		_controls.Clear();

		foreach (DefinitionAssetRow row in _definitionAsset.Rows)
		{
			AddRowControls(row);
		}
	}

	private void AddRowControls(DefinitionAssetRow row)
	{
		List<InspectorControl> rowControls = new();
		foreach (DefinitionAssetValue value in row.Values)
		{
			InspectorControl control = (InspectorControl) Activator.CreateInstance(value.InspectorControlType, this, $"tool_asset_editor_control.{UniqueId}")!;
			rowControls.Add(control);

			control.InitFromSource(row, value, value.GetType().GetFields()[0]);
			control.SetCustomLabel("");
		}

		_controls.Add(rowControls);
	}

	private void AddRow()
	{
		DefinitionAssetRow assetRow = new ();
		foreach (DefinitionTemplateField? field in _definitionAsset.Template.Fields)
		{
			if (field == null)
			{
				return;
			}

			assetRow.Values.Add(field.Type.ToDefinitionAssetValue());
		}

		// todo - command
		_definitionAsset.Rows.Add(assetRow);
		AddRowControls(assetRow);
	}

	private void RemoveRow(int rowIndex)
	{
		// todo - command
		_definitionAsset.Rows.RemoveAt(rowIndex);
		_controls.RemoveAt(rowIndex);
	}
}