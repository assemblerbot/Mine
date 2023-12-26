namespace Mine.Studio;

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