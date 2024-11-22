#ifndef RADISH_UTILS_H
#define RADISH_UTILS_H

#include "Packages/com.radish.render-pipeline/ShaderLibrary/Common.hlsl"

inline float ThreeWayLerp(float a, float b, float c, float t)
{
    float bc = lerp(b, c, saturate(t));
    float r = lerp(a, bc, saturate(-t));
    return r;
}

inline float2 ThreeWayLerp(float2 a, float2 b, float2 c, float t)
{
    float2 bc = lerp(b, c, saturate(t));
    float2 r = lerp(a, bc, saturate(-t));
    return r;
}

inline float3 ThreeWayLerp(float3 a, float3 b, float3 c, float t)
{
    float3 bc = lerp(b, c, saturate(t));
    float3 r = lerp(a, bc, saturate(-t));
    return r;
}

inline float4 ThreeWayLerp(float4 a, float4 b, float4 c, float t)
{
    float4 bc = lerp(b, c, saturate(t));
    float4 r = lerp(a, bc, saturate(-t));
    return r;
}

inline float3 GetCameraForwardDir()
{
    return mul(unity_ObjectToWorld, float3(0, 0, 1)).xyz;
}

#endif