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
            HLSLPROGRAM

            #include "Packages/com.radish.render-pipeline/ShaderLibrary/Common.hlsl"

            #pragma vertex BlitVertex
            #pragma fragment BlitFragment
            #pragma target 5.0

            RADISH_DECLARE_TEX2D_NOSAMPLER(_MainTex);
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 texcoord : TEXCOORD0;
            };

            Varyings BlitVertex(uint vertexID : SV_VertexID)
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

            float4 BlitFragment(Varyings IN) : SV_TARGET
            {
                return SAMPLE_TEXTURE2D_LOD(_MainTex, sampler_PointClamp, IN.texcoord, 0);
            }

            ENDHLSL
        }
    }
}