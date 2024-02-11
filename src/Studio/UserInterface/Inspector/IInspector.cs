using CommandProcessorPlugin;

namespace Mine.Studio;

public interface IInspector
{
	InspectorControlMap ControlMap { get; }
	
	void Commit(Command command);
}