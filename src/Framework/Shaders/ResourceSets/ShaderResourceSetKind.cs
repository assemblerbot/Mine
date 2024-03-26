namespace Mine.Framework;

public enum ShaderResourceSetKind
{
	Uninitialized,
	
	// matrices
	WorldMatrix,
	ViewProjectionMatrix,
	
	// material
	MaterialProperties0,
	MaterialProperties1,
	MaterialProperties2,
	MaterialProperties3,
	MaterialProperties4,
	MaterialProperties5,
	MaterialProperties6,
	MaterialProperties7,
	MaterialPropertiesMin = MaterialProperties0,
	MaterialPropertiesMax = MaterialProperties7,
	
	// lights
	AmbientLight,
	DirectionalLight,
	PointLights,
	SpotLights,
}