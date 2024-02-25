namespace Mine.Studio;

[NodeIO(ProjectNodeType.AssetTessControlShader)]
public sealed class NodeIOTessControlShader : NodeIOShader
{
	public NodeIOTessControlShader(ProjectNode owner) : base(owner)
	{
	}

	public override NodeIOSettings CreateImportSettings()
	{
		return new NodeIOShaderSettings{ShaderStage = NodeIOShaderStage.tesscontrol};
	}
}