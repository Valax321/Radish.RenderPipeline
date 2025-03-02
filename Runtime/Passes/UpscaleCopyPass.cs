using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

namespace Radish.Rendering.Passes
{
    public sealed class UpscaleCopyPassData
    {
        public TextureHandle Source;
        public TextureHandle Destination;
        public ResolutionScaleMethod Method;
    }

    [Serializable]
    [SupportedOnRenderPipeline(typeof(RadishRenderPipelineAsset))]
    [UnityEngine.Categorization.CategoryInfo(Name = "Radish Upscale Pass Resources")]
    internal sealed class CameraTargetBlitResources : IRenderPipelineResources
    {
        public int version => 0;
        public bool isAvailableInPlayerBuild => true;

        [ResourcePath("Hidden/Radish/UpscaleBlit", SearchType.ShaderName)]
        [SerializeField] private Shader m_BlitShader;

        public Shader BlitShader => m_BlitShader;
    }
    
    [Serializable]
    public sealed class UpscaleCopyPass : RenderPass<UpscaleCopyPassData>
    {
        private static Material s_BlitMaterial;
        private static Material s_ComposeMaterial;

        private ResourceIdentifier m_Source;
        private ResourceIdentifier m_Destination;

        public UpscaleCopyPass(string name, ResourceIdentifier source, ResourceIdentifier destination) : base(name)
        {
            m_Source = source;
            m_Destination = destination;
        }

        protected override void SetupPass(UpscaleCopyPassData data, in RenderPassContext passContext, in CameraContext cameraContext,
            ref RenderGraphBuilder builder)
        {
            var resources = GraphicsSettings.GetRenderPipelineSettings<CameraTargetBlitResources>();
            var src = passContext.passManager.Get<TextureHandle>(m_Source);
            var dest = passContext.passManager.Get<TextureHandle>(m_Destination);
            
            InitMaterial(ref s_BlitMaterial, resources.BlitShader);
            
            data.Source = src.RegisterUse(builder.ReadTexture(src.value));
            data.Destination = dest.RegisterUse(builder.WriteTexture(dest.value));
            
            builder.AllowPassCulling(false);

            var settings = cameraContext.VolumeStack.GetComponent<UpscaleComponent>();
            data.Method = settings.upscaleMethod.value;
            
            builder.SetRenderFunc<UpscaleCopyPassData>(static (data, context) =>
            {
                context.cmd.SetRenderTarget(data.Destination);
                BlitProcedural(context.cmd, data.Source, s_BlitMaterial, (int)data.Method);
            });
        }
    }
}