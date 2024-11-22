Shader "Hidden/Radish/UpscaleBlit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        ZWrite Off
        Cull Off
        ZTest Always
        
        HLSLINCLUDE

        #include "Packages/com.radish.render-pipeline/ShaderLibrary/BlitPass.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Filtering.hlsl"

        float4 FinalBlitNearest(in BlitPassVaryings IN) : SV_TARGET
        {
            return SAMPLE_TEXTURE2D_LOD(_MainTex, sampler_PointClamp, IN.texcoord, 0);
        }

        float4 FinalBlitBilinear(in BlitPassVaryings IN) : SV_TARGET
        {
            return SAMPLE_TEXTURE2D_LOD(_MainTex, sampler_LinearClamp, IN.texcoord, 0);
        }

        float4 FinalBlitBicubic(in BlitPassVaryings IN) : SV_TARGET
        {
            return SampleTexture2DBicubic(TEXTURE2D_ARGS(_MainTex, sampler_LinearClamp), IN.texcoord, _MainTex_TexelSize.zwxy, 1.0, 0);
        }
        
        ENDHLSL

        Pass
        {
            Name "Nearest"
            
            HLSLPROGRAM

            #pragma vertex BlitPassVertex
            #pragma fragment FinalBlitNearest
            #pragma target 5.0
            
            ENDHLSL
        }

        Pass
        {
            Name "Bilinear"
            
            HLSLPROGRAM

            #pragma vertex BlitPassVertex
            #pragma fragment FinalBlitBilinear
            #pragma target 5.0
            
            ENDHLSL
        }

        Pass
        {
            Name "Bicubic"
            
            HLSLPROGRAM

            #pragma vertex BlitPassVertex
            #pragma fragment FinalBlitBicubic
            #pragma target 5.0
            
            ENDHLSL
        }
    }
}