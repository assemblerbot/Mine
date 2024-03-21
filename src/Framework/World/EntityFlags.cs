namespace Mine.Framework;

[Flags]
public enum EntityFlags
{
	IsInWorld                   = 1 << 0, // connected to hierarchy under world's root entity
	ResourceSetWorldMatrixDirty = 1 << 2,
	ActiveSelf                  = 1 << 3,
	IsDisposed                  = 1 << 4,

	LocalToWorldMatrixDirty     = 1 << 5,
	WorldToLocalMatrixDirty     = 1 << 6,
	ShaderResourceWorldMatrixDirty = 1 << 7,
	MatricesDirty               = LocalToWorldMatrixDirty | WorldToLocalMatrixDirty | ShaderResourceWorldMatrixDirty,
	
	Default = ActiveSelf | MatricesDirty | ResourceSetWorldMatrixDirty
}