uniform extern texture ScreenTexture;	

sampler ScreenS = sampler_state
{
	Texture = <ScreenTexture>;	
};

float wave;				// pi/.75 is a good default
float distortion;		// 1 is a good default
float2 centerCoord;		// 0.5,0.5 is the screen center

float4 PixelShader(float2 texCoord: TEXCOORD0) : COLOR
{
float4 Color;
Color.a = 1.0f;
Color = tex2D(ScreenS, texCoord.xy);
Color.rgb = (Color.r+Color.g+Color.b)/3.0f;
 	
if (Color.r>0.5) Color.r = 1.0f;// else Color.r = 0.0f;
if (Color.g>0.5) Color.g = 1.0f;// else Color.g = 0.0f;
if (Color.b>0.5) Color.b = 1.0f;// else Color.b = 0.0f;

	
			
	return Color;
}
technique
{
	pass P0
	{
		PixelShader = compile ps_2_0 PixelShader();
	}
}
