using Migration;
using RedHerring.Studio.Models;
using RedHerring.Studio.Models.Project.FileSystem;
using RedHerring.Studio.UserInterface;

namespace Mine.Studio;

[Serializable, SerializedClassId("f82d86eb-7957-4c24-8b8d-3a1e04c86ec6")]
public abstract class StudioReference
{
	public virtual Type   InspectorControlType => typeof(InspectorReferenceControl);
	public abstract string Name { get; }

	public string? Guid = null;
	public bool    IsEmpty => Guid == null;

	public abstract bool            CanAcceptNode(StudioModel studioModel, ProjectNode node);
	public abstract StudioReference CreateCopy();
	
	public virtual void WriteJsonValue(StringWriter stringWriter)
	{
		//TODO
	}

	protected bool Equals(StudioReference other)
	{
		return Guid == other.Guid;
	}

	public override bool Equals(object? obj)
	{
		if (ReferenceEquals(null, obj))
		{
			return false;
		}

		if (ReferenceEquals(this, obj))
		{
			return true;
		}

		if (obj.GetType() != this.GetType())
		{
			return false;
		}

		return Equals((StudioReference) obj);
	}

	public static bool operator ==(StudioReference? left, StudioReference? right)
	{
		return Equals(left, right);
	}

	public static bool operator !=(StudioReference? left, StudioReference? right)
	{
		return !Equals(left, right);
	}
}

#region Migration

[MigratableInterface(typeof(StudioReference))]
public interface IStudioReferenceMigratable;
    
[Serializable, LatestVersion(typeof(StudioReference))]
public class StudioReference_000 : IStudioReferenceMigratable
{
	public string Guid;
}
#endregion