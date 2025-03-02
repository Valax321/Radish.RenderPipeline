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

BlitPassVaryings BlitPassVertex(uint vertexID : SV_VertexID)
{
    BlitPassVaryings OUT;

    OUT.positionCS = float4(
        vertexID <= 1 ? -1.0 : 3.0,
        vertexID == 1 ? 3.0 : -1.0,
        0.0, 1.0
    );

    OUT.positionCS.y *= _ProjectionParams.x;

    OUT.texcoord = float2(
        vertexID <= 1 ? 0.0 : 2.0,
        vertexID == 1 ? 2.0 : 0.0
    );

    return OUT;
}

RADISH_DECLARE_TEX2D_NOSAMPLER(_MainTex);
float4 _MainTex_TexelSize;

#endif