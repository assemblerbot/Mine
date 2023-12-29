using Migration;

namespace Mine.Studio;

[Serializable, SerializedClassId("7583bbd9-04c7-4c0f-b791-4c52836093d6")]
public sealed class DefinitionTemplateFieldString : DefinitionTemplateField
{
	public override string TypeName => "string";

	public DefinitionTemplateFieldString() : base("new")
	{
	}
	
	public DefinitionTemplateFieldString(string name) : base(name)
	{
	}
}

#region Migration

[MigratableInterface(typeof(DefinitionTemplateFieldString))]
public interface IDefinitionTemplateFieldStringMigratable : IDefinitionTemplateFieldMigratable;
    
[Serializable, LatestVersion(typeof(DefinitionTemplateFieldString))]
public class DefinitionTemplateFieldString_000 : DefinitionTemplateField_000, IDefinitionTemplateFieldStringMigratable
{
}
#endregion