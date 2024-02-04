using Migration;
using Mine.Framework;

namespace Mine.Studio;

[Serializable, SerializedClassId("74764396-ae2a-4023-bdba-55ede73161ab")]
public class MaterialAsset : Material
{
	// TODO - shader reference
	// material parameters are stored in base class? - what about references to textures?
}

#region Migration

[MigratableInterface(typeof(MaterialAsset))]
public interface IMaterialAssetMigratable;
    
[Serializable, LatestVersion(typeof(MaterialAsset))]
public class MaterialAsset_000 : IMaterialAssetMigratable
{
}
#endregion