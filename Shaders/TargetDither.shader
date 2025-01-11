Shader "Hidden/Radish/TargetDither"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DitherIntensity ("Dither Intensity", Range(0, 1)) = 0.05
        _FrameNumber ("Frame Number", Float) = 0
    }
    SubShader
    {
        ZWrite Off
        ZTest Always
        Cull Off
        
        HLSLINCLUDE

        #include "Packages/com.radish.render-pipeline/ShaderLibrary/Common.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Filtering.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

        RADISH_DECLARE_TEX2D_NOSAMPLER(_MainTex);
        TEXTURE2D_ARRAY(_BlueNoise);
        
        float4 _MainTex_TexelSize;
        float4 _BlueNoise_TexelSize;
        float _DitherIntensity;
        int _FrameNumber;

        #define LUMA_RESPONSE 0.8

        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float2 texcoord : TEXCOORD0;
        };

        inline float Dither4x4Bayer(int x, int y)
        {
            const float dither[16] = {
                1, 9, 3, 11,
                13, 5, 15, 7,
                4, 12, 2, 10,
                16, 8, 14, 6
            };
            int r = y * 4 + x;
            return dither[r] / 16; // same # of instructions as pre-dividing due to compiler magic
        }

        Varyings DitherVertex(uint vertexID : SV_VertexID)
        {
            Varyings OUT;

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

        inline float3 ApplyDithering(float3 color, float3 ditherPattern, float lumaResponse, float intensity)
        {
            float lum = 1.0 - sqrt(Luminance(saturate(color)));
            lum = lerp(1.0, lum, lumaResponse);
            return color + (color * ditherPattern * intensity * lum);
        }

        float4 DitherFrag_Bayer(in Varyings IN) : SV_TARGET
        {
            float2 pixelPos = IN.texcoord * _MainTex_TexelSize.zw;
            float dither = Dither4x4Bayer(fmod(pixelPos.x, 4), fmod(pixelPos.y, 4));

            float4 col = SAMPLE_TEXTURE2D_LOD(_MainTex, sampler_PointClamp, IN.texcoord, 0);
            
            col.rgb = ApplyDithering(col.rgb, float3(dither, dither, dither), LUMA_RESPONSE, _DitherIntensity);
            
            return col;
        }

        float4 DitherFrag_BlueNoise(in Varyings IN) : SV_TARGET
        {
            float2 uv = IN.texcoord;
            uv *= _MainTex_TexelSize.zw * _BlueNoise_TexelSize.xy;
            
             float3 noise = SAMPLE_TEXTURE2D_ARRAY_LOD(_BlueNoise, sampler_PointRepeat, uv, _FrameNumber, 0).rgb;
            float4 col = SAMPLE_TEXTURE2D_LOD(_MainTex, sampler_PointClamp, IN.texcoord, 0);

            col.rgb = ApplyDithering(col.rgb, noise, LUMA_RESPONSE, _DitherIntensity);

            return col;
        }
        
        ENDHLSL

        Pass
        {
            Name "Dither 4x4 Bayer"
            
            HLSLPROGRAM

            #pragma vertex DitherVertex
            #pragma fragment DitherFrag_Bayer
            #pragma target 5.0
            
            ENDHLSL
        }

        Pass
        {
            Name "Dither Blue Noise"
            
            HLSLPROGRAM

            #pragma vertex DitherVertex
            #pragma fragment DitherFrag_BlueNoise
            #pragma target 5.0
            
            ENDHLSL
        }
    }
}