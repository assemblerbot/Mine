using Migration;

namespace Mine.Studio;

[Serializable, SerializedClassId("definition-asset-row")]
public sealed class DefinitionAssetRow
{
	public List<DefinitionAssetValue> _values = new();
}