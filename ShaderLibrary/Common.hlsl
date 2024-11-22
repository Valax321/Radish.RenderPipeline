#ifndef RADISH_RP_COMMON_H
#define RADISH_RP_COMMON_H

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "Packages/com.radish.render-pipeline/ShaderLibrary/Input.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/GlobalSamplers.hlsl"

#define RADISH_DECLARE_TEX2D_SAMPLER(baseName) TEXTURE2D(baseName); SAMPLER(sampler##baseName);
#define RADISH_DECLARE_TEX2D_NOSAMPLER(baseName) TEXTURE2D(baseName);

TEXTURE2D(_CameraColorTexture);
TEXTURE2D(_CameraDepthTexture);

float4 ComputeScreenPos(float4 positionCS)
{
    float4 o = positionCS * 0.5f;
    o.xy = float2(o.x, o.y * _ProjectionParams.x) + o.w;
    o.zw = positionCS.zw;
    return o;
}

#endif