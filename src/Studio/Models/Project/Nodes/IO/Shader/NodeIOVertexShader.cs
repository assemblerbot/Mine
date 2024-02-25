namespace Mine.Studio;

[NodeIO(ProjectNodeType.AssetVertexShader)]
public sealed class NodeIOVertexShader : NodeIOShader
{
	public NodeIOVertexShader(ProjectNode owner) : base(owner)
	{
	}

	public override NodeIOSettings CreateImportSettings()
	{
		return new NodeIOShaderSettings{ShaderStage = NodeIOShaderStage.vertex};
	}
}