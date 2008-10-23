uniform extern texture ScreenTexture;	

//this is a pixel sample, basically just grabs a pixel from the screen
sampler ScreenS = sampler_state
{
	Texture = <ScreenTexture>;	
};

//this function is run on each pixel in turn, it takes a texture coordinate as a parameter, and returns a colour value.
float4 PixelShader(float2 texCoord: TEXCOORD0) : COLOR
{
	//this will hold our return value
	float4 Color;

	Color.a = 1.0f;
	
	//grab the pixel attributes 
	Color = tex2D(ScreenS, texCoord.xy);
	
	//Color.rgb = (Color.r+Color.g+Color.b)/3.0f;
 	
	if (Color.r > 0.3) Color.r *= 2.0f; else Color.r = 0.0f;
	if (Color.g > 0.3) Color.b *= 2.0f; else Color.g = 0.0f;
	if (Color.b > 0.3) Color.g *= 2.0f; else Color.b = 0.0f;
	
	//this will be the final colour of the pixel.
	return Color;
}
technique
{
	pass P0
	{
		PixelShader = compile ps_2_0 PixelShader();
	}
}
