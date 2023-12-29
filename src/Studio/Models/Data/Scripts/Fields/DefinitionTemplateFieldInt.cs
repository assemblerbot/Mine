using Migration;

namespace Mine.Studio;

[Serializable, SerializedClassId("2d21adef-dbfc-49be-b8cd-a0306dcea5b6")]
public sealed class DefinitionTemplateFieldInt : DefinitionTemplateField
{
	public override string TypeName => "int";

	public DefinitionTemplateFieldInt() : base("new")
	{
	}
	
	public DefinitionTemplateFieldInt(string name) : base(name)
	{
	}
}
#region Migration

[MigratableInterface(typeof(DefinitionTemplateFieldInt))]
public interface IDefinitionTemplateFieldIntMigratable : IDefinitionTemplateFieldMigratable;
    
[Serializable, LatestVersion(typeof(DefinitionTemplateFieldInt))]
public class DefinitionTemplateFieldInt_000 : DefinitionTemplateField_000, IDefinitionTemplateFieldIntMigratable
{
}
#endregion