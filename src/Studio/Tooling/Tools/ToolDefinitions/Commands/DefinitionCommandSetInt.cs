using RedHerring.Studio.Commands;

namespace Mine.Studio.Commands;

public sealed class DefinitionCommandSetInt : Command
{
	private DefinitionAssetValueInt _assetValue;
	private int                  _value;
	private int                  _prevValue;
	
	public DefinitionCommandSetInt(DefinitionAssetValueInt assetValue, int value)
	{
		_assetValue = assetValue;
		_value      = value;
	}

	public override void Do()
	{
		_prevValue        = _assetValue.Value;
		_assetValue.Value = _value;
	}

	public override void Undo()
	{
		_assetValue.Value = _prevValue;
	}
}