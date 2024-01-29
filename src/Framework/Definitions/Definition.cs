namespace Mine.Framework;

[Serializable]
public abstract class Definition
{
	public abstract void LoadDefinitionsRecursive();
	public abstract void LoadAllRecursive();
}