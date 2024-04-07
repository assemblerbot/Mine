using Silk.NET.Windowing;

namespace Gfx;

public enum GraphicsBackend
{
	Vulkan,
	Direct3D12,
}

public static class GraphicsBackendExtensions
{
	public static GraphicsAPI ToGraphicsAPI(this GraphicsBackend backend)
	{
		return backend switch
		{
			GraphicsBackend.Direct3D12 => GraphicsAPI.None,
			GraphicsBackend.Vulkan => GraphicsAPI.DefaultVulkan,
			// GraphicsBackend.OpenGL => new GraphicsAPI(ContextAPI.OpenGL, new APIVersion(4, 5)),
			// GraphicsBackend.Metal => GraphicsAPI.None,
			// GraphicsBackend.OpenGLES => new GraphicsAPI(ContextAPI.OpenGLES, new APIVersion(3, 1)),
			_ => throw new ArgumentOutOfRangeException()
		};
	}
}
