namespace Mine.Framework;

[Serializable]
public sealed class DefinitionReference<T> : Reference where T:Definition
{
	public override ReferenceKind Kind => ReferenceKind.AssetDefinition;
}