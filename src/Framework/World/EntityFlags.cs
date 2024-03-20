namespace Mine.Framework;

[Flags]
public enum EntityFlags
{
	IsInWorld                   = 1 << 0, // connected to hierarchy under world's root entity
	MatricesDirty               = 1 << 1,
	ResourceSetWorldMatrixDirty = 1 << 2,
	ActiveSelf = 1 << 3,
	IsDisposed = 1 << 4,
	
	Default = ActiveSelf | MatricesDirty | ResourceSetWorldMatrixDirty
}