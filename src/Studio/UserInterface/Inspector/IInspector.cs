using Mine.Studio;
using RedHerring.Studio.Commands;
using RedHerring.Studio.Models.Project.FileSystem;

namespace RedHerring.Studio.UserInterface;

public interface IInspector
{
	void   Commit(Command                     command);
}