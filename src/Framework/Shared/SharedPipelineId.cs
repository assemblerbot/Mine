using Gfx;

namespace Mine.Framework;

public readonly record struct SharedPipelineId
{
	public readonly ulong             MaterialPassId;
	public readonly ulong             VertexLayoutId;
	public readonly PrimitiveTopology Topology;
	public readonly ulong             OutputDescriptionId;

	public SharedPipelineId(ulong materialPassId, ulong vertexLayoutId, PrimitiveTopology topology, ulong outputDescriptionId)
	{
		MaterialPassId          = materialPassId;
		VertexLayoutId      = vertexLayoutId;
		Topology            = topology;
		OutputDescriptionId = outputDescriptionId;
	}
	
	public override int GetHashCode()
	{
		return HashCode.Combine(MaterialPassId, VertexLayoutId, Topology, OutputDescriptionId);
	}
}