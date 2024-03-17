namespace Mine.Framework;

public enum ShaderConstantSemantic
{
	Custom,
	
	WorldMatrix,
	ViewMatrix,
	ProjectionMatrix,
	WorldViewProjectionMatrix,
	ViewProjectionMatrix,
	
	ObjectPosition,
	ObjectRotation,
	ObjectScale,
	
	Light0Position,
	Light0Direction,
	Light0Parameters,
	Light0Color,

	Light10Position,
	Light1Direction,
	Light1Parameters,
	Light1Color,

	Light2Position,
	Light2Direction,
	Light2Parameters,
	Light2Color,

	Light3Position,
	Light3Direction,
	Light3Parameters,
	Light3Color,
}