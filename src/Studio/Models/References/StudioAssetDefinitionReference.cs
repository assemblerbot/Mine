using Migration;
using RedHerring.Studio.Models;
using RedHerring.Studio.Models.Project;
using RedHerring.Studio.Models.Project.FileSystem;

namespace Mine.Studio;

[Serializable, SerializedClassId("1d1cea4c-4339-4d76-a548-602210666e09")]
public sealed class StudioAssetDefinitionReference : StudioReference
{
	public override string Name => GetName();
	public          string GenericParameterGuid;

	public StudioAssetDefinitionReference(string genericParameterGuid)
	{
		GenericParameterGuid = genericParameterGuid;
	}

	public override bool CanAcceptNode(StudioModel studioModel, ProjectNode node)
	{
		if (node.Type != ProjectNodeType.AssetDefinition)
		{
			return false;
		}

		NodeIOAssetDefinition? io = node.GetNodeIO<NodeIOAssetDefinition>();
		if (io == null)
		{
			return false;
		}

		return io.Asset?.Template.Header.Guid == GenericParameterGuid;
	}

	public override StudioReference CreateCopy()
	{
		return new StudioAssetDefinitionReference(GenericParameterGuid) {Guid = Guid};
	}

	private string GetName()
	{
		ProjectNode? node = StudioModel.Instance.Project.FindNode(node => node.Type == ProjectNodeType.ScriptDefinition && node.Meta!.Guid == GenericParameterGuid, false, true);
		if (node is not null)
		{
			NodeIOScriptDefinition? io = node.GetNodeIO<NodeIOScriptDefinition>();
			if (io?.Template is not null)
			{
				return $"Definition<{io.Template.NamespaceName}.{io.Template.ClassName}>";
			}
		}

		return $"Definition<{GenericParameterGuid}>";
	}
}

#region Migration
[MigratableInterface(typeof(StudioAssetDefinitionReference))]
public interface IStudioAssetDefinitionReferenceMigratable : IStudioReferenceMigratable;
    
[Serializable, LatestVersion(typeof(StudioAssetDefinitionReference))]
public class StudioAssetDefinitionReference_000 : StudioReference_000, IStudioAssetDefinitionReferenceMigratable
{
	public string GenericParameterGuid;
}
#endregion