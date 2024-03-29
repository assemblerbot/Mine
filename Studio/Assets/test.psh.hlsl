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

half4 main(VStoPS input) : SV_TARGET
{
	half4 ndotl = saturate(dot(input.Normal, -DirectionalLightDirection));
	//return input.Position*0.0001 + float4(input.UV, 0,0)*0.0001 + input.Color*0.0001 + _texture.Sample(_sampler, input.UV);
	//return SurfaceColor * input.VertexColor * AmbientLightColor;
	return SurfaceColor * (ndotl * DirectionalLightColor + AmbientLightColor);
}
