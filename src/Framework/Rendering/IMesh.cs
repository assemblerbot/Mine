using Veldrid;

namespace Mine.Framework;

public interface IMesh : IRenderable
{
	Material Material { get; }
	void     Draw(CommandList commandList);
}