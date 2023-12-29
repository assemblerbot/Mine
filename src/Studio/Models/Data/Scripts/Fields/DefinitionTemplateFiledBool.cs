using Migration;

namespace Mine.Studio;

[Serializable, SerializedClassId("6380ff58-3a2f-4d13-8b46-fb053c5f6e7e")]
public sealed class DefinitionTemplateFiledBool : DefinitionTemplateField
{
	public override string TypeName => "bool";

	public DefinitionTemplateFiledBool() : base("new")
	{
	}
	
	public DefinitionTemplateFiledBool(string name) : base(name)
	{
	}
}

#region Migration

[MigratableInterface(typeof(DefinitionTemplateFiledBool))]
public interface IDefinitionTemplateFiledBoolMigratable : IDefinitionTemplateFieldMigratable;
    
[Serializable, LatestVersion(typeof(DefinitionTemplateFiledBool))]
public class DefinitionTemplateFiledBool_000 : DefinitionTemplateField_000, IDefinitionTemplateFiledBoolMigratable
{
}
#endregion