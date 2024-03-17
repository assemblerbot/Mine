namespace Mine.Framework;

public enum UniformBufferKind
{
	Uninitialized,
	
	// world
	WorldStats,
	
	// entity
	WorldMatrix,
	
	// renderer/camera
	ViewProjectionMatrix,
	
	// light
	LightDirectional0,
	LightDirectional1,
	
	LightPoint0,
	LightPoint1,
	LightPoint2,
	LightPoint3,
	LightPoint4,
	LightPoint5,
	LightPoint6,
	LightPoint7,
	LightPoint8,
	LightPoint9,
	
	// material - custom from material definition
	Material,
}