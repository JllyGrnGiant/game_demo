float4x4 World;
float4x4 View;
float4x4 Projection;
texture TextureMap;
sampler2D TextureMapSampler = sampler_state { texture = TextureMap; AddressU = WRAP; AddressV = WRAP; MinFilter = Linear; MipFilter = LINEAR; MagFilter = Linear; };

float UVScaling = 30.0f;

texture RedTexture;
sampler2D RedSampler = sampler_state { texture = RedTexture; AddressU = WRAP; AddressV = WRAP; MinFilter = Linear; MipFilter = LINEAR; MagFilter = Linear; };

texture GreenTexture;
sampler2D GreenSampler = sampler_state { texture = GreenTexture; AddressU = WRAP; AddressV = WRAP; MinFilter = Linear; MipFilter = LINEAR; MagFilter = Linear; };

texture BlueTexture;
sampler2D BlueSampler = sampler_state { texture = BlueTexture; AddressU = WRAP; AddressV = WRAP; MinFilter = Linear; MipFilter = LINEAR; MagFilter = Linear; };

texture AlphaTexture;
sampler2D AlphaSampler = sampler_state { texture = AlphaTexture; AddressU = WRAP; AddressV = WRAP; MinFilter = Linear; MipFilter = LINEAR; MagFilter = Linear; };

float3 LightDirection;
float3 Ambient;
float3 LightColor;

float FogStart;
float FogEnd;
float MinFog;
float MaxFog;
float3 FogColor;
float FogDensity;

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float2 UV : TEXCOORD0;
	float3 Normal : NORMAL0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float2 UV : TEXCOORD0;
	float3 Normal : TEXCOORD1;
	float Depth : TEXCOORD2;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
	output.Normal = mul(input.Normal, World);
	output.UV = input.UV;
	output.Depth = output.Position.z;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	//float4 red = tex2D(RedSampler, input.UV * UVScaling);
	//float4 green = tex2D(GreenSampler, input.UV * UVScaling);
	//float4 blue = tex2D(BlueSampler, input.UV * UVScaling);
	//float4 alpha = tex2D(AlphaSampler, input.UV * UVScaling);
	//float4 map = tex2D(TextureMapSampler, input.UV);
	//float a = map.a;
	//float4 final = (map.r * a * red) + (map.g * a * green) + (map.b * a * blue) + ((1.0f - a) * alpha);
	//float4 final = (red) + (green) + (blue);

	float3 normal = normalize(input.Normal);
	float3 lightDir = normalize(LightDirection);
	float3 lightMod = dot(normal, lightDir);

	lightMod.r = clamp(lightMod.r * LightColor.r, Ambient.r, 1);
	lightMod.g = clamp(lightMod.g * LightColor.g, Ambient.g, 1);
	lightMod.b = clamp(lightMod.b * LightColor.b, Ambient.b, 1);

	float4 tex = tex2D(TextureMapSampler, input.UV * UVScaling);
	tex *= float4(lightMod, 1);

	float depth = input.Depth;

	float fog = (depth - FogStart) / (FogEnd - FogStart);
	fog *= FogDensity;
	fog = clamp(fog, MinFog, MaxFog);

	tex = lerp(tex, float4(FogColor, 1), fog);

	return tex;
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
