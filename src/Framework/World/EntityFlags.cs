namespace Mine.Framework;

[Flags]
public enum EntityFlags
{
	None = 0,
	TransformDirty = 0x02,
	MatricesDirty = 0x03,
}