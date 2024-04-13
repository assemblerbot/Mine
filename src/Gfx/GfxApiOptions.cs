using Silk.NET.Windowing;

namespace Gfx;

public struct GfxApiOptions
{
	public GraphicsBackend                                               Backend;
	public IWindow                                                       Window;
	public Action<DebugMessageSeverity, DebugMessageKind, string>? DebugMessageLog; // if null, no validation layers will be enabled

	public GfxApiOptions(
		GraphicsBackend                                               backend,
		IWindow                                                       window,
		Action<DebugMessageSeverity, DebugMessageKind, string>? debugMessageLog = null
	)
	{
		Backend         = backend;
		Window          = window;
		DebugMessageLog = debugMessageLog;
	}
}