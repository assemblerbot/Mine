using RedHerring.Studio.Commands;

namespace RedHerring.Studio.UserInterface;

public interface IInspector
{
	void Commit(Command command);
}