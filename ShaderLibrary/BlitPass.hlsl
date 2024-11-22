#ifndef RADISH_BLIT_PASS_H
#define RADISH_BLIT_PASS_H

#include "Packages/com.radish.render-pipeline/ShaderLibrary/Common.hlsl"

struct BlitPassAttributes
{
    float4 positionOS : POSITION;
    float2 texcoord : TEXCOORD0;
};

struct BlitPassVaryings
{
    float4 positionCS : SV_POSITION;
    float2 texcoord : TEXCOORD0;
};

BlitPassVaryings BlitPassVertex(in BlitPassAttributes IN)
{
    BlitPassVaryings OUT;

    OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
    OUT.texcoord = IN.texcoord;

    return OUT;
}

RADISH_DECLARE_TEX2D_NOSAMPLER(_MainTex);
float4 _MainTex_TexelSize;

#endif