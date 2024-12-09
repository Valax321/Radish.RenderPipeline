using JetBrains.Annotations;
using Radish.Rendering.Settings;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

namespace Radish.Rendering
{
    [PublicAPI]
    public abstract class RenderPass<T> : RenderPassBase
        where T : class, new()
    {
        public string name { get; }
        
        private ProfilingSampler m_Sampler;
        
        private static Material s_ColorSpaceConvertShader;
        protected static Material colorSpaceConvertMaterial
        {
            get
            {
                if (!s_ColorSpaceConvertShader)
                {
                    var settings = GraphicsSettings.GetRenderPipelineSettings<RadishBuiltinResources>();
                    InitMaterial(ref s_ColorSpaceConvertShader, settings.colorSpaceConvertShader);
                }

                return s_ColorSpaceConvertShader;
            }
        }
        
        protected const int LinearToSRGBPassIndex = 0;
        protected const int SRGBToLinearPassIndex = 1;

        private static Material s_BlitMaterial;
        private static readonly int s_MainTexProp = Shader.PropertyToID("_MainTex");

        protected static Material blitMaterial
        {
            get
            {
                if (!s_BlitMaterial)
                {
                    var settings = GraphicsSettings.GetRenderPipelineSettings<RadishBuiltinResources>();
                    InitMaterial(ref s_BlitMaterial, settings.blitShader);
                }

                return s_BlitMaterial;
            }
        }
        
        public RenderPass(string name)
        {
            this.name = name;
        }

        public override void AddToGraph(RadishRenderPipeline pipeline, in CameraContext cameraContext)
        {
            m_Sampler ??= new ProfilingSampler(name);
            var context = new RenderPassContext(pipeline);

            if (ShouldCullPass(context, cameraContext))
                return;
            
            //TODO: Copy attachments here (if requested)
            
            var builder = context.graph.AddRenderPass<T>(name, out var passData, m_Sampler);
            try
            {
                //TODO: Register color/depth use if requested
                
                SetupPass(passData, in context, in cameraContext, ref builder);
            }
            finally
            {
                builder.Dispose();
            }
        }
        
        protected abstract void SetupPass(T data, in RenderPassContext passContext,
            in CameraContext cameraContext, ref RenderGraphBuilder builder);

        protected virtual bool ShouldCullPass(in RenderPassContext passContext,
            in CameraContext cameraContext)
        {
            return false;
        }
        
        protected static void InitMaterial(ref Material material, Shader shader)
        {
            if (!material || (material.shader != shader))
            {
                if (material)
                    CoreUtils.Destroy(material);
                
                material = CoreUtils.CreateEngineMaterial(shader);
            }
        }
        
        protected TextureHandle CreatePingPongTextureFrom(TextureHandle tex, in RenderPassContext passContext,
            ref RenderGraphBuilder builder)
        {
            var sceneColorDesc = passContext.graph.GetTextureDesc(tex);
            sceneColorDesc.name = $"{name} Temp Texture";
            return builder.CreateTransientTexture(in sceneColorDesc);
        }

        protected static void BlitProcedural(CommandBuffer cmd, RTHandle source, Material material, int pass)
        {
            var propBlock = MaterialPropertyBlockPool.Get();
            propBlock.SetTexture(s_MainTexProp, source);
            cmd.DrawProcedural(Matrix4x4.identity, material, pass, MeshTopology.Triangles, 3, 1, propBlock);
            MaterialPropertyBlockPool.Release(propBlock);
        }
    }
}