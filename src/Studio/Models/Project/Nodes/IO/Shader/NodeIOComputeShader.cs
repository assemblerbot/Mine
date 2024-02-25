namespace Mine.Studio;

[NodeIO(ProjectNodeType.AssetComputeShader)]
public sealed class NodeIOComputeShader : NodeIOShader
{
	public NodeIOComputeShader(ProjectNode owner) : base(owner)
	{
	}

	public override NodeIOSettings CreateImportSettings()
	{
		return new NodeIOShaderSettings{ShaderStage = NodeIOShaderStage.compute};
	}
}