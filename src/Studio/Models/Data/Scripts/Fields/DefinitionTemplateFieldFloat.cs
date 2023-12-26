namespace Mine.Studio;

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