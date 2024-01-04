using System.Numerics;
using ImGuiNET;
using Mine.ImGuiPlugin;
using RedHerring.Studio.Commands;
using RedHerring.Studio.Models.Project.FileSystem;
using RedHerring.Studio.UserInterface;

namespace Mine.Studio;

public sealed class ToolDefinitionAssetEditor : IInspectorCommandTarget
{
	#region Row
	private sealed class Row
	{
		public readonly List<InspectorControl> Controls = new();
		public readonly string                 DeleteButtonNameId;

		public Row(int uniqueId)
		{
			DeleteButtonNameId = $"{FontAwesome6.Xmark}##delete_button.{uniqueId}";
		}

		public Row AddControls(IInspectorCommandTarget commandTarget, DefinitionAssetRow row, Func<int> uniqueIdGenerator)
		{
			foreach (DefinitionAssetValue value in row.Values)
			{
				InspectorControl control = (InspectorControl) Activator.CreateInstance(value.InspectorControlType, commandTarget, $"tool_asset_editor_control.{uniqueIdGenerator()}")!;
				Controls.Add(control);

				control.InitFromSource(row, value, value.GetType().GetFields()[0]);
				control.SetCustomLabel("");
			}

			return this;
		}
	}
	#endregion

	#region Commands
	private sealed class AddRowCommand : Command
	{
		private readonly IInspectorCommandTarget _commandTarget;
		private readonly DefinitionAsset         _asset;
		private readonly List<Row>               _rows;
		private readonly Func<int>               _uniqueIdGenerator;

		private int                 _rowIndex;
		private DefinitionAssetRow? _assetRow;
		private Row?                _controlsRow;

		public AddRowCommand(IInspectorCommandTarget commandTarget, DefinitionAsset asset, List<Row> rows, Func<int> uniqueIdGenerator)
		{
			_commandTarget     = commandTarget;
			_asset             = asset;
			_rows              = rows;
			_uniqueIdGenerator = uniqueIdGenerator;
		}

		public override void Do()
		{
			if (_assetRow == null || _controlsRow == null)
			{
				_assetRow = new();
				foreach (DefinitionTemplateField? field in _asset.Template.Fields)
				{
					if (field == null)
					{
						return;
					}

					_assetRow.Values.Add(field.Type.ToDefinitionAssetValue());
				}

				_controlsRow = new Row(_uniqueIdGenerator()).AddControls(_commandTarget, _assetRow, _uniqueIdGenerator);
				_rowIndex    = _asset.Rows.Count;
			}

			_asset.Rows.Add(_assetRow);
			_rows.Add(_controlsRow);
		}

		public override void Undo()
		{
			_asset.Rows.RemoveAt(_rowIndex);
			_rows.RemoveAt(_rowIndex);
		}
	}

	private sealed class DeleteRowCommand : Command
	{
		private DefinitionAsset _asset;
		private List<Row>       _rows;

		private int                 _rowIndex;
		private DefinitionAssetRow? _assetRow;
		private Row?                _controlsRow;

		public DeleteRowCommand(DefinitionAsset asset, List<Row> rows, int rowIndex)
		{
			_asset    = asset;
			_rows     = rows;
			_rowIndex = rowIndex;
		}

		public override void Do()
		{
			_assetRow    = _asset.Rows[_rowIndex];
			_controlsRow = _rows[_rowIndex];

			_asset.Rows.RemoveAt(_rowIndex);
			_rows.RemoveAt(_rowIndex);
		}

		public override void Undo()
		{
			_asset.Rows.Insert(_rowIndex, _assetRow!);
			_rows.Insert(_rowIndex, _controlsRow!);
		}
	}
	#endregion

	private readonly ProjectAssetFileNode     _definitionAssetNode;
	private readonly DefinitionAsset          _definitionAsset;
	private readonly string                   _editorUniqueId;

	private readonly CommandHistoryWithChange _commandHistory = new();
	private readonly List<Row>                _controlRows    = new();
	
	private int _uniqueIdGenerator = 0;

	
	public ToolDefinitionAssetEditor(ProjectAssetFileNode assetNode, DefinitionAsset asset, string editorUniqueId)
	{
		_definitionAssetNode = assetNode;
		_definitionAsset         = asset;
		_editorUniqueId          = editorUniqueId;
		RebuildControls();
	}

	public void Commit(Command command)
	{
		_commandHistory.Commit(command);
	}
	
	public void Update()
	{
		ImGui.PushID(_editorUniqueId);
		
		ImGui.Text(_definitionAssetNode.RelativePath);
		
		//--- buttons ---
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

		if (ImGui.Button("Apply changes"))
		{
			if (WriteToFile())
			{
				_commandHistory.ResetChange();
			}
		}

		if (wasChange)
		{
			ImGui.PopStyleColor(3);
		}
		else
		{
			ImGui.EndDisabled();
		}

		ImGui.SameLine();
		if (_commandHistory.CanUndo)
		{
			if (ImGui.Button(FontAwesome6.RotateLeft))
			{
				_commandHistory.Undo();
			}
		}
		else
		{
			ImGui.BeginDisabled();
			ImGui.Button(FontAwesome6.RotateLeft);
			ImGui.EndDisabled();
		}

		ImGui.SameLine();
		if (_commandHistory.CanRedo)
		{
			if (ImGui.Button(FontAwesome6.RotateRight))
			{
				_commandHistory.Redo();
			}
		}
		else
		{
			ImGui.BeginDisabled();
			ImGui.Button(FontAwesome6.RotateRight);
			ImGui.EndDisabled();
		}


		//--- table ---
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
			bool newRowRequested    = false;
			int  deleteRowRequested = -1;

			ImGui.TableSetupColumn("", ImGuiTableColumnFlags.WidthFixed | ImGuiTableColumnFlags.NoResize | ImGuiTableColumnFlags.NoReorder, 20f);
			
			for (int col = 0; col < fields.Count; ++col)
			{
				ImGui.TableSetupColumn(fields[col]?.Name ?? "null", ImGuiTableColumnFlags.WidthStretch);
			}
			ImGui.TableSetupScrollFreeze(0, 1);

			// setup headers
			{
				//ImGui.TableHeadersRow(); <-- default was replaced by:

				ImGui.TableNextRow(ImGuiTableRowFlags.Headers);
				{
					ImGui.TableSetColumnIndex(0);
					ImGui.PushID(0);
					newRowRequested = ImGui.SmallButton(FontAwesome6.Plus);
					ImGui.PopID();
				}

				for (int col = 0; col < fields.Count; ++col)
				{
					ImGui.TableSetColumnIndex(col+1);
					ImGui.PushID(col+1);
					string columnName = ImGui.TableGetColumnName(col+1);
					ImGui.TableHeader(columnName);
					ImGui.PopID();
				}
			}

			for (int rowIndex = 0; rowIndex < _definitionAsset.Rows.Count; ++rowIndex)
			{
				DefinitionAssetRow row = _definitionAsset.Rows[rowIndex];
				
				ImGui.TableNextRow();

				ImGui.TableSetColumnIndex(0);
				if (ImGui.SmallButton(_controlRows[rowIndex].DeleteButtonNameId))
				{
					deleteRowRequested = rowIndex;
				}

				for (int columnIndex = 0; columnIndex < fields.Count; ++columnIndex)
				{
					ImGui.TableSetColumnIndex(columnIndex + 1);
					_controlRows[rowIndex].Controls[columnIndex].Update();
				}
			}
			ImGui.EndTable();

			if (newRowRequested)
			{
				Commit(new AddRowCommand(this, _definitionAsset, _controlRows, CreateUniqueId));
			}

			if (deleteRowRequested != -1)
			{
				Commit(new DeleteRowCommand(_definitionAsset, _controlRows, deleteRowRequested));
			}
		}

		ImGui.PopID();
	}

	private void RebuildControls()
	{
		_controlRows.Clear();

		foreach (DefinitionAssetRow row in _definitionAsset.Rows)
		{
			_controlRows.Add(new Row(CreateUniqueId()).AddControls(this, row, CreateUniqueId));
		}
	}

	private int CreateUniqueId()
	{
		return ++_uniqueIdGenerator;
	}

	private bool WriteToFile()
	{
		_definitionAsset.WriteToFile(_definitionAssetNode.AbsolutePath);
		return true;
	}
}