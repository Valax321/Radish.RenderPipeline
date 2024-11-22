using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

namespace Radish.Rendering.Passes
{
    public sealed class PostSharpenPassData
    {
        public TextureHandle Target;
        public TextureHandle TempColor;
        public Vector2 SSize;
    }

    [Serializable]
    [SupportedOnRenderPipeline(typeof(RadishRenderPipelineAsset))]
    [UnityEngine.Categorization.CategoryInfo(Name = "Radish Sharpen Pass Resources")]
    internal sealed class PostSharpenPassResources : IRenderPipelineResources
    {
        public int version => 0;
        
        [ResourcePath("Hidden/Radish/Sharpen", SearchType.ShaderName)]
        [SerializeField] private Shader m_SharpenShader;
        
        public bool isAvailableInPlayerBuild => true;
        public Shader sharpenShader => m_SharpenShader;
    }
    
    [PublicAPI]
    public sealed class PostSharpenPass : RenderPass<PostSharpenPassData>
    {
        private static Material s_Material;
        private static readonly int s_SSize = Shader.PropertyToID("_SSize");

        private ResourceIdentifier m_ColorTexture;

        public PostSharpenPass(string name, ResourceIdentifier colorTexture) : base(name)
        {
            m_ColorTexture = colorTexture;
        }

        protected override void SetupPass(PostSharpenPassData data, in RenderPassContext passContext, in CameraContext cameraContext,
            ref RenderGraphBuilder builder)
        {
            var backbuffer = passContext.passManager.Get<TextureHandle>(m_ColorTexture);
            var settings = GraphicsSettings.GetRenderPipelineSettings<PostSharpenPassResources>();
            
            InitMaterial(ref s_Material, settings.sharpenShader);

            data.Target = backbuffer.RegisterUse(builder.ReadWriteTexture(builder.ReadWriteTexture(backbuffer.value)));
            data.TempColor = CreatePingPongTextureFrom(data.Target, in passContext, ref builder);
            data.SSize = new Vector2(1.0f / cameraContext.Camera.scaledPixelWidth, 1.0f / cameraContext.Camera.scaledPixelHeight);
            
            builder.SetRenderFunc<PostSharpenPassData>(static (data, ctx) =>
            {
                ctx.cmd.SetGlobalVector(s_SSize, data.SSize);
                
                ctx.cmd.Blit(data.Target, data.TempColor, colorSpaceConvertMaterial, LinearToSRGBPassIndex);
                ctx.cmd.Blit(data.TempColor, data.Target, s_Material, 0);
                ctx.cmd.Blit(data.Target, data.TempColor, s_Material, 1);
                ctx.cmd.Blit(data.TempColor, data.Target, colorSpaceConvertMaterial, SRGBToLinearPassIndex);
            });
        }

        protected override bool ShouldCullPass(in RenderPassContext passContext, in CameraContext cameraContext)
        {
            var settings = cameraContext.VolumeStack.GetComponent<UpscaleComponent>();
            return !settings.sharpenImage.value || !settings.active;
        }
    }
}