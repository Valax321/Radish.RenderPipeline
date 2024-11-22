Shader "Hidden/Radish/ColorSpaceConvert"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Cull Off
        ZWrite Off
        ZTest Always
        
        HLSLINCLUDE

        #include "Packages/com.radish.render-pipeline/ShaderLibrary/BlitPass.hlsl"
        
        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float2 texcoord : TEXCOORD0;
            float2 texSizeRcp : TEXCOORD1;
        };

        Varyings FXAA_Vert(in BlitPassAttributes IN)
        {
            Varyings OUT;

            OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
            OUT.texcoord = IN.texcoord;
            OUT.texSizeRcp = rcp(_MainTex_TexelSize.zw);
            
            return OUT;
        }
        
        ENDHLSL
        
        Pass
        {
            Name "Linear To SRGB"
            
            HLSLPROGRAM

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

            #pragma vertex BlitPassVertex
            #pragma fragment FXAA_Frag
            #pragma target 5.0

            float4 FXAA_Frag(in BlitPassVaryings IN) : SV_TARGET
            {
                float4 c = SAMPLE_TEXTURE2D_LOD(_MainTex, sampler_PointClamp, IN.texcoord, 0);
                return float4(LinearToSRGB(c.rgb), c.a);
            }

            ENDHLSL
        }

        Pass
        {
            Name "SRGB To Linear"
            
            HLSLPROGRAM

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

            #pragma vertex BlitPassVertex
            #pragma fragment FXAA_Frag
            #pragma target 5.0

            float4 FXAA_Frag(in BlitPassVaryings IN) : SV_TARGET
            {
                float4 c = SAMPLE_TEXTURE2D_LOD(_MainTex, sampler_PointClamp, IN.texcoord, 0);
                return float4(SRGBToLinear(c.rgb), c.a);
            }

            ENDHLSL
        }
    }
}