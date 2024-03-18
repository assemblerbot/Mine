using Veldrid;

namespace Mine.Framework;

public sealed class SharedResourceSetLayouts : IDisposable
{
	// layout per kind
	private ResourceLayout? _worldMatrix;
	private ResourceLayout? _viewProjectionMatrix; 
	
	public void Dispose()
	{
		_worldMatrix?.Dispose();
		_viewProjectionMatrix?.Dispose();
	}
}