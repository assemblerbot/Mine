namespace Gfx;

/// <summary>
/// Universal exception thrown from Gfx library.
/// </summary>
public sealed class GfxException : SystemException
{
	public readonly string Message;
	
	public GfxException(string message = "")
	{
		Message = message;
	}
}