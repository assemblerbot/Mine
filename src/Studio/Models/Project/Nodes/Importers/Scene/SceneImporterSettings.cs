﻿using Migration;

namespace Mine.Studio;

[Serializable, SerializedClassId("ea019557-3078-4cf2-8bb1-f3ffe8d84e73")]
public sealed class SceneImporterSettings : ImporterSettings
{
	public float NormalSmoothingAngle = 15f;
	public bool  CompensateFBXScale   = false;
	
	[ReadOnlyInInspector] public List<SceneImporterMeshSettings> Meshes = new();

	[ReadOnlyInInspector] public List<SceneImporterMaterialSettings> Materials = new();

	[ReadOnlyInInspector] public SceneImporterHierarchyNodeSettings Root = new("Root");
}

#region Migration
[MigratableInterface(typeof(SceneImporterSettings))]
public interface ISceneImporterSettingsMigratable : IImporterSettingsMigratable;

[Serializable, LatestVersion(typeof(SceneImporterSettings))]
public class SceneImporterSettings_000 : ImporterSettings_000, ISceneImporterSettingsMigratable
{
	public float SmoothingAngle;
	public bool  CompensateFBXScale;
	
	[MigrateField] public List<ISceneImporterMeshSettingsMigratable> Meshes;

	[MigrateField] public List<ISceneImporterMaterialSettingsMigratable> Materials;

	public ISceneImporterHierarchyNodeSettingsMigratable Root;
}
#endregion