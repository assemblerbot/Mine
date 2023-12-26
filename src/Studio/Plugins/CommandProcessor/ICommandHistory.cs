namespace RedHerring.Studio.Commands;

public interface ICommandHistory
{
	void Commit(Command command);
	void Undo();
	void Redo();
}