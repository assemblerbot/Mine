using Migration;

namespace Mine.Studio;

[Serializable, SerializedClassId("a7573432-3f53-434a-bf3d-94e859f84f59")]
public sealed class DefinitionTemplateFieldFloat : DefinitionTemplateField
{
	public override string TypeName => "float";

	public DefinitionTemplateFieldFloat() : base("new")
	{
	}
	
	public DefinitionTemplateFieldFloat(string name) : base(name)
	{
	}
}

#region Migration

[MigratableInterface(typeof(DefinitionTemplateFieldFloat))]
public interface IDefinitionTemplateFieldFloatMigratable : IDefinitionTemplateFieldMigratable;
    
[Serializable, LatestVersion(typeof(DefinitionTemplateFieldFloat))]
public class DefinitionTemplateFieldFloat_000 : DefinitionTemplateField_000, IDefinitionTemplateFieldFloatMigratable
{
}
#endregion