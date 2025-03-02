Shader "Hidden/Radish/CameraTargetBlit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        ZWrite Off
        ZTest Always
        Cull Off

        Pass
        {
            Name "Standard Blit"
            HLSLPROGRAM

            #include "Packages/com.radish.render-pipeline/ShaderLibrary/BlitPass.hlsl"

            #pragma vertex BlitPassVertex
            #pragma fragment BlitFragment
            #pragma target 5.0
            
            float4 BlitFragment(BlitPassVaryings IN) : SV_TARGET
            {
                return SAMPLE_TEXTURE2D_LOD(_MainTex, sampler_PointClamp, IN.texcoord, 0);
            }

            ENDHLSL
        }

        Pass
        {
            Name "AlphaBlend Blit"
            Blend SrcAlpha OneMinusSrcAlpha
            
            HLSLPROGRAM

            #include "Packages/com.radish.render-pipeline/ShaderLibrary/BlitPass.hlsl"

            #pragma vertex BlitPassVertex
            #pragma fragment BlitFragment
            #pragma target 5.0
            
            float4 BlitFragment(BlitPassVaryings IN) : SV_TARGET
            {
                return SAMPLE_TEXTURE2D_LOD(_MainTex, sampler_PointClamp, IN.texcoord, 0);
            }

            ENDHLSL
        }
    }
}