namespace Mine.Studio;

[NodeIO(ProjectNodeType.AssetGeometryShader)]
public sealed class NodeIOGeometryShader : NodeIOShader
{
	public NodeIOGeometryShader(ProjectNode owner) : base(owner)
	{
	}

	public override NodeIOSettings CreateImportSettings()
	{
		return new NodeIOShaderSettings{ShaderStage = NodeIOShaderStage.geometry};
	}
}