namespace Mine.Framework;

[Serializable]
public sealed class AssetReference : Reference
{
	public byte[]? Bytes = null;
	
	public override bool Load()
	{
		try
		{
			Bytes = Engine.Resources.ReadResource(Path);
		}
		catch (Exception e)
		{
			return false;
		}

		return true;
	}
}