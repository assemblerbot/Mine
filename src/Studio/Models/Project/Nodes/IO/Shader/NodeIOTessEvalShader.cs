namespace Mine.Studio;

[NodeIO(ProjectNodeType.AssetTessEvalShader)]
public sealed class NodeIOTessEvalShader : NodeIOShader
{
	public NodeIOTessEvalShader(ProjectNode owner) : base(owner)
	{
	}

	public override NodeIOSettings CreateImportSettings()
	{
		return new NodeIOShaderSettings{ShaderStage = NodeIOShaderStage.tesseval};
	}
}