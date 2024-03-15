using Veldrid;

namespace Mine.Framework;

public interface IMesh : IRenderable
{
	ulong Id { get; }
	Material Material { get; }
	void     Draw(CommandList commandList);
}