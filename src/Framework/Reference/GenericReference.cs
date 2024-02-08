/*
using Migration;
using OdinSerializer;

namespace Mine.Framework;

[Serializable, SerializedClassId("a9b015f2-c0fa-46b5-8cdf-d9553a993621")]
public sealed class GenericReference<T> where T : class
{
	public string? Guid = null;
	[NonSerialized] public T?      Data = null;

	public bool Load()
	{
		if (Guid == null)
		{
			return false;
		}

		byte[]? bytes = Engine.Resources.ReadResourceByGuid(Guid);
		if (bytes == null)
		{
			return false;
		}

		Data = SerializationUtility.DeserializeValue<T>(bytes, DataFormat.Binary);
		return Data is not null;
	}
}

#region Migration
[MigratableInterface(typeof(GenericReference<>))]
public interface IGenericReferenceMigratable;
    
[Serializable, LatestVersion(typeof(GenericReference<>))]
public class GenericReference_000 : IGenericReferenceMigratable
{
	public string Guid;
}
#endregion
*/