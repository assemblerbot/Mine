namespace Mine.Framework;

[Serializable]
public abstract class Reference
{
	public string Path;

	public abstract bool Load();
}