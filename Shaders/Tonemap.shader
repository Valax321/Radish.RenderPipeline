Shader "Hidden/Radish/Tonemapping"
{
    Properties
    {
        _MainTex("Scene Color", 2D) = "white" {}
    }
    SubShader
    {
        Cull Off
        ZWrite Off
        ZTest Always
        
        HLSLINCLUDE

        #include "Packages/com.radish.render-pipeline/ShaderLibrary/BlitPass.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        
        ENDHLSL

        Pass
        {
            Name "Neutral Tonemap"
            
            HLSLPROGRAM

            #pragma vertex BlitPassVertex
            #pragma fragment TonemapperFrag
            #pragma target 5.0

            float4 TonemapperFrag(in BlitPassVaryings IN) : SV_TARGET
            {
                float4 sceneColor = SAMPLE_TEXTURE2D_LOD(_MainTex, sampler_PointClamp, IN.texcoord, 0);
                sceneColor.rgb = NeutralTonemap(sceneColor.rgb);
                return sceneColor;
            }
            
            ENDHLSL
        }

        Pass
        {
            Name "ACES Tonemap"
            
            HLSLPROGRAM

            #pragma vertex BlitPassVertex
            #pragma fragment TonemapperFrag

            float4 TonemapperFrag(in BlitPassVaryings IN) : SV_TARGET
            {
                float4 sceneColor = SAMPLE_TEXTURE2D_LOD(_MainTex, sampler_PointClamp, IN.texcoord, 0);
                sceneColor.rgb = AcesTonemap(unity_to_ACES(sceneColor.rgb));
                return sceneColor;
            }
            
            ENDHLSL
        }
    }
}