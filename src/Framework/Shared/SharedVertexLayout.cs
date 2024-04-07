using Gfx;

namespace Mine.Framework;

public readonly struct SharedVertexLayout
{
	public readonly ulong                   Id;
	public readonly VertexLayoutDescription VertexLayoutDescription;

	public SharedVertexLayout(ulong id, VertexLayoutDescription vertexLayoutDescription)
	{
		Id                      = id;
		VertexLayoutDescription = vertexLayoutDescription;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Id, VertexLayoutDescription);
	}
}