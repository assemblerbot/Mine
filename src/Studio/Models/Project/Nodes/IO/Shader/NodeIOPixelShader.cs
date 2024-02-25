namespace Mine.Studio;

[NodeIO(ProjectNodeType.AssetPixelShader)]
public sealed class NodeIOPixelShader : NodeIOShader
{
	public NodeIOPixelShader(ProjectNode owner) : base(owner)
	{
	}

	public override NodeIOSettings CreateImportSettings()
	{
		return new NodeIOShaderSettings{ShaderStage = NodeIOShaderStage.fragment};
	}
}