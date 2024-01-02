using Mine.Framework;
using Mine.ImGuiPlugin;
using RedHerring.Studio.Models;
using RedHerring.Studio.Models.ViewModels;
using RedHerring.Studio.UserInterface;
using RedHerring.Studio.UserInterface.Attributes;
using Gui = ImGuiNET.ImGui;

namespace RedHerring.Studio.Tools;

[Tool(ToolName)]
public sealed class ToolInspector : Tool
{
	public const       string    ToolName = FontAwesome6.CircleInfo + " Inspector";
	protected override string    Name => ToolName;

	private readonly StudioModel _studioModel;
	
	private readonly   Inspector _inspector;

	//private List<object> _tests = new(){new InspectorTest2(), new InspectorTest()}; // TODO debug
	private List<object> _tests = new(){new InspectorTest()}; // TODO debug

	private bool _subscribedToChange = false;

	public ToolInspector(StudioModel studioModel, int uniqueId) : base(studioModel, uniqueId)
	{
		_studioModel = studioModel;
		_inspector        = new Inspector(studioModel.CommandHistory);
		_inspector.Init(_tests);
	}
	
	public override void Update(out bool finished)
	{
		finished = UpdateUI();
	}

	private bool UpdateUI()
	{
		bool isOpen = true;
		if (Gui.Begin(NameId, ref isOpen))
		{
			if (!_subscribedToChange)
			{
				SubscribeToChange();
			}

			_inspector.Update();
			Gui.End();
		}
		else
		{
			if (_subscribedToChange)
			{
				UnsubscribeFromChange();
			}
		}

		return !isOpen;
	}
	
	#region Event handlers

	private void SubscribeToChange()
	{
		_studioModel.EventAggregator.Register<SelectionViewModel.SelectionChanged>(OnSelectionChanged);
		_subscribedToChange = true;
	}

	private void UnsubscribeFromChange()
	{
		_studioModel.EventAggregator.Unregister<SelectionViewModel.SelectionChanged>(OnSelectionChanged);
		_subscribedToChange = false;
	}

	private void OnSelectionChanged(SelectionViewModel.SelectionChanged e)
	{
		_inspector.Init(StudioModel.Selection.GetAllSelectedTargets());
	}

	#endregion
}




//=======================================================================================================
// tests
//=======================================================================================================
public enum TestEnum
{
	Abc,
	Def,
	Ghi,
	Jkl,
	Mno
}

public class InspectorTest
{
	[ReadOnlyInInspector] public int      SomeValue1 = 1;
	public                       int      SomeValue2 = 22;
	public                       int      SomeValue3 = 333;
	public                       int      SomeValue4 = 4444;
	public                       float    FloatValue = 1.0f;
	public                       bool     BoolValue  = true;
	public                       TestEnum EnumValue  = TestEnum.Def;
	
	public InspectorTestSubclass       Subclass = new();
	[ReadOnlyInInspector, TableList] public List<InspectorTestSubclass> TestList = new() {new InspectorTestSubclass(), new InspectorTestSubclass()};

	[ValueDropdown("DropdownSource")] public int      DropdownInt    = 1;
	[ValueDropdown("DropdownSource")] public string   DropdownString = "pear";
	[HideInInspector]                 public string[] DropdownSource = {"apple", "pear", "orange", "banana"};
}

public class InspectorTestSubclass
{
	public int    SubValue1   = 111111;
	public int    SubValue2   = 222222;
	public string StringValue = "abc";
}



public class InspectorTest2
{
	public                       int SomeValue1 = 5;
	[ReadOnlyInInspector] public int SomeValue2 = 22;

	public InspectorTestSubclass2      Subclass = new();

	[ShowInInspector] private int SomeValue3 = 333;
	[HideInInspector] public  int SomeValue4 = 4444;
	
	public int   SomeValue5 = 55555;
	public float FloatValue = 1.0f;
	public bool  BoolValue  = true;

	public TestEnum EnumValue = TestEnum.Abc;

	[ValueDropdown("DropdownSource")] public int      DropdownInt    = 1;
	[ValueDropdown("DropdownSource")] public string   DropdownString = "pear";
	[HideInInspector]                 public string[] DropdownSource = {"apple", "pear", "orange", "banana"};
}

public class InspectorTestSubclass2
{
	public int    SubValue2   = 666666;
	public int    SubValue3   = 555555;
	public string StringValue = "abc";
}