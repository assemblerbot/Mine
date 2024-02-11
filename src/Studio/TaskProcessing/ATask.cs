namespace Mine.Studio;

public abstract class ATask
{
	public abstract void Process(CancellationToken cancellationToken);
}