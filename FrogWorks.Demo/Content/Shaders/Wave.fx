sampler s0;
float timer;
float offset;
float wavelength;
float frequency;

float4 PixelShaderFunction(float2 coords: TEXCOORD0) : COLOR0
{
	coords.x += sin(timer + (offset + coords.y) * wavelength) * frequency;
	float4 color = tex2D(s0, coords);
	return color;
}

technique Technique1
{
	pass Pass1
	{
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}	
}
