using Migration;

namespace Mine.Studio;

[Serializable, SerializedClassId("1d1cea4c-4339-4d76-a548-602210666e09")]
public sealed class StudioDefinitionReference : StudioReference
{
	public override string Name => $"Definition<{GenericParameter}>";
	public          string GenericParameter;

	public StudioDefinitionReference(string genericParameter)
	{
		GenericParameter = genericParameter;
	}
}

#region Migration
[MigratableInterface(typeof(StudioDefinitionReference))]
public interface IStudioDefinitionReferenceMigratable : IStudioReferenceMigratable;
    
[Serializable, LatestVersion(typeof(StudioDefinitionReference))]
public class StudioDefinitionReference_000 : StudioReference_000, IStudioDefinitionReferenceMigratable
{
	public string GenericParameter;
}
#endregion