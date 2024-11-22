Shader "Hidden/Radish/FXAA"
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
            Name "Luma Compute"
            
            HLSLPROGRAM

            #pragma vertex BlitPassVertex
            #pragma fragment FXAA_Frag
            #pragma target 5.0

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

            float4 FXAA_Frag(in BlitPassVaryings IN) : SV_TARGET
            {
                float4 c = LinearToSRGB(SAMPLE_TEXTURE2D_LOD(_MainTex, sampler_PointClamp, IN.texcoord, 0));
                c.a = Luminance(saturate(c.rgb));

                return c;
            }

            ENDHLSL
        }

        Pass
        {
            Name "FXAA Apply"
            
            HLSLPROGRAM
            
            #define FXAA_HLSL_5 1
            #define FXAA_GREEN_AS_LUMA 0

            #if FXAA_QUALITY_HIGH
            #define FXAA_QUALITY__PRESET 39
            #define FXAA_PC 1
            #define FXAA_EDGE_THRESHOLD      0.063
            #define FXAA_EDGE_THRESHOLD_MIN  0.0312
            #define FXAA_SUBPIX 0.5
            #elif FXAA_QUALITY_MEDIUM
            #define FXAA_QUALITY__PRESET 12
            #define FXAA_PC 1
            #define FXAA_EDGE_THRESHOLD      (1.0/8.0)
            #define FXAA_EDGE_THRESHOLD_MIN  (1.0/24.0)
            #define FXAA_SUBPIX 1.0
            #else
            #define FXAA_PC 1
            #define FXAA_QUALITY__PRESET 10
            #define FXAA_EDGE_THRESHOLD      0.333
            #define FXAA_EDGE_THRESHOLD_MIN  0.0625
            #define FXAA_SUBPIX 1.0
            #endif
            
            #include "Assets/Game/Shaders/PostProcessing/FXAA.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

            #pragma vertex FXAA_Vert
            #pragma fragment FXAA_Frag
            #pragma multi_compile_local_fragment FXAA_QUALITY_LOW FXAA_QUALITY_MEDIUM FXAA_QUALITY_HIGH
            #pragma target 5.0

            float4 FXAA_Frag(in Varyings IN) : SV_TARGET
            {
                FxaaTex tex;
                tex.tex = _MainTex;
                tex.smpl = sampler_LinearClamp;
                
                float4 c = FxaaPixelShader(IN.texcoord, float4(0, 0, 0, 0), tex, tex, tex,
                    IN.texSizeRcp,
                    0, 0, 0,
                    FXAA_SUBPIX,
                    FXAA_EDGE_THRESHOLD,
                    FXAA_EDGE_THRESHOLD_MIN,
                    0, 0, 0, float4(0, 0, 0, 0)
                );

                return SRGBToLinear(c);
            }

            ENDHLSL
        }
    }
}