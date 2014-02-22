float4x4 World;
float4x4 View;
float4x4 Projection;

float4 AmbientColor;
float AmbientIntensity;

struct VertexShaderInput
{
float4 Position : POSITION0;
};

struct VertexShaderOutput
{
float4 Position : POSITION0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;

	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);
	
	return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	return AmbientColor * AmbientIntensity;
}

technique Techique1
{
	pass Pass1
	{
		VertexShader = compile vs_2_0 VertexShaderFunction();
		PixelShader = compile ps_2_0 PixelShaderFunction();
		
	}
	
}