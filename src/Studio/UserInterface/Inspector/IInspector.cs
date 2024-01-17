using RedHerring.Studio.Commands;

namespace RedHerring.Studio.UserInterface;

public interface IInspector
{
	InspectorControlMap ControlMap { get; }
	
	void Commit(Command command);
}