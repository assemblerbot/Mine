namespace Mine.Studio;

public readonly struct ConsoleItem
{
	public readonly ConsoleItemType Type;
	public readonly string          Message;

	public ConsoleItem(ConsoleItemType type, string? message)
	{
		Type    = type;
		Message = message ?? "null";
	}
}