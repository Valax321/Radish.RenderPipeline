using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

namespace Radish.Rendering.Passes
{
    public sealed class UpscaleCopyPassData
    {
        public TextureHandle SceneColor;
        public TextureHandle CameraTarget;
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
            var sceneColor = passContext.passManager.Get<TextureHandle>(m_Source);
            var cameraTarget = passContext.passManager.Get<TextureHandle>(m_Destination);
            
            InitMaterial(ref s_BlitMaterial, resources.BlitShader);
            
            data.SceneColor = sceneColor.RegisterUse(builder.ReadTexture(sceneColor.value));
            data.CameraTarget = cameraTarget.RegisterUse(builder.WriteTexture(cameraTarget.value));
            
            builder.AllowPassCulling(false);

            if (ScalableBufferManager.widthScaleFactor >= 1 || !cameraContext.Camera.allowDynamicResolution)
            {
                data.Method = ResolutionScaleMethod.Nearest;
            }
            else
            {
                var settings = cameraContext.VolumeStack.GetComponent<UpscaleComponent>();
                data.Method = settings.upscaleMethod.value;
            }
            
            builder.SetRenderFunc<UpscaleCopyPassData>(static (data, context) =>
            {
                context.cmd.Blit(data.SceneColor, data.CameraTarget, s_BlitMaterial, (int)data.Method);
            });
        }
    }
}