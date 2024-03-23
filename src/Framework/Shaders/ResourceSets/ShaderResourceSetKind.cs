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
	AmbientLight0,
	
	DirectionalLight0,
	DirectionalLight1,
	
	PointLight0,
	PointLight1,
	PointLight2,
	PointLight3,

	SpotLight0,
	SpotLight1,
	SpotLight2,
	SpotLight3,
}