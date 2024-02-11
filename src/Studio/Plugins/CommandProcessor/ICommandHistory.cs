namespace CommandProcessorPlugin;

public interface ICommandHistory
{
	void Commit(Command command);
	void Undo();
	void Redo();
}