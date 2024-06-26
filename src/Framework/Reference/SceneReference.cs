using OdinSerializer;

namespace Mine.Framework;

[Serializable]
public sealed class SceneReference : Reference<Scene>
{
	public SceneReference(string path) : base(path)
	{
	}

	public override Scene? LoadValue()
	{
		try
		{
			byte[]? bytes = Engine.Resources.ReadResource(Path);
			if (bytes is null)
			{
				return null;
			}

			return SerializationUtility.DeserializeValue<Scene>(bytes, DataFormat.Binary);
		}
		catch (Exception e)
		{
			return null;
		}
	}
}