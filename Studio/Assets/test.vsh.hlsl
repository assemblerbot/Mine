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

struct VSInput
{
	layout(location=0) float3 Position : POSITION0;
	layout(location=1) float3 Normal : NORMAL0;
	layout(location=2) float2 UV : TEXCOORD0;
};

struct VStoPS
{
	layout(location=0) float4 Position : SV_POSITION;
	layout(location=1) float3 Normal : NORMAL0;
	layout(location=2) float2 UV : TEXCOORD0;
	layout(location=3) float4 VertexColor : TEXCOORD1;
};

VStoPS main(VSInput input)
{
	float4 worldPosition = float4(input.Position, 1) * WorldMatrix;

	VStoPS output;
	output.Position = worldPosition * ViewProjectionMatrix;
	output.Normal = float4(input.Normal, 0) * WorldMatrix;
	output.UV = input.UV;
	output.VertexColor = saturate(dot(output.Normal, -LightDirection));
	return output;
}
