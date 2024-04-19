
layout(set = 0, binding = 0)  cbuffer WorldMatrixConstants
{
	float4x4 WorldMatrix;
};

layout(set = 1, binding = 0)  cbuffer ViewProjectionMatrixConstants
{
	float4x4 ViewProjectionMatrix;
};

layout(set = 2, binding = 0) cbuffer MaterialVertex
{
	float4 LightDirection;
};

layout(set = 3, binding = 0) cbuffer MaterialPixel
{
	float4 SurfaceColor;
};

layout(set = 4, binding = 0) cbuffer AmbientLightConstants
{
	float4 AmbientLightColor;
};

layout(set = 5, binding = 0) cbuffer DirectionalLightConstants
{
	float4 DirectionalLightColor;
	float4 DirectionalLightDirection;
};

struct VStoPS
{
	layout(location=0) float4 Position : SV_POSITION;
	layout(location=1) float3 Normal : NORMAL0;
	layout(location=2) float2 UV : TEXCOORD0;
	layout(location=3) float4 VertexColor : TEXCOORD1;
};

float4 main(VStoPS input) : SV_TARGET
{
	float4 tmp = 0.0001 * saturate(input.Position * WorldMatrix + input.Position * ViewProjectionMatrix + LightDirection);
	
	tmp += 0.0001 * SurfaceColor;
	tmp += 0.0001 * AmbientLightColor;
	tmp += 0.0001 * DirectionalLightColor;

	float4 ndotl = saturate(dot(input.Normal, -DirectionalLightDirection));
	return SurfaceColor * (ndotl * DirectionalLightColor + AmbientLightColor) + tmp;
}
