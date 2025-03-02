using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

namespace Radish.Rendering.Passes
{
    public sealed class TonemapperPassData
    {
        public TextureHandle SceneColor;
        public Material TonemapperMaterial;
        public TextureHandle TempColor;
        public TonemapperPass.TonemapType Type;
    }
    
    [Serializable]
    [SupportedOnRenderPipeline(typeof(RadishRenderPipelineAsset))]
    [UnityEngine.Categorization.CategoryInfo(Name = "Radish Tonemapping Pass Resources")]
    internal sealed class TonemapResources : IRenderPipelineResources
    {
        public int version => 0;

        [ResourcePath("Hidden/Radish/Tonemapping", SearchType.ShaderName)]
        [SerializeField] private Shader m_TonemapShader;
        
        public bool isAvailableInPlayerBuild => true;
        public Shader tonemapShader => m_TonemapShader;
    }
    
    [PublicAPI]
    public sealed class TonemapperPass : RenderPass<TonemapperPassData>
    {
        public enum TonemapType
        {
            None = -1,
            Neutral = 0,
            ACES = 1,
        }

        private static Material s_Material;

        private ResourceIdentifier m_ColorTexture;
        
        public TonemapperPass(string name, ResourceIdentifier colorTexture) : base(name)
        {
            m_ColorTexture = colorTexture;
        }

        protected override void SetupPass(TonemapperPassData data, in RenderPassContext passContext, in CameraContext cameraContext,
            ref RenderGraphBuilder builder)
        {
            var settings = GraphicsSettings.GetRenderPipelineSettings<TonemapResources>();
            var sceneColor = passContext.passManager.Get<TextureHandle>(m_ColorTexture);
            var component = cameraContext.VolumeStack.GetComponent<TonemapperComponent>();
            
            InitMaterial(ref s_Material, settings.tonemapShader);
            
            data.SceneColor = sceneColor.RegisterUse(builder.ReadWriteTexture(sceneColor.value));
            data.TempColor = CreatePingPongTextureFrom(data.SceneColor, in passContext, ref builder);
            data.TonemapperMaterial = s_Material;
            data.Type = component.type.value;
            
            builder.AllowPassCulling(false);
            
            builder.SetRenderFunc<TonemapperPassData>(static (data, ctx) =>
            {
                ctx.cmd.SetRenderTarget(data.TempColor);
                BlitProcedural(ctx.cmd, data.SceneColor, data.TonemapperMaterial, (int)data.Type);
                
                ctx.cmd.SetRenderTarget(data.SceneColor);
                BlitProcedural(ctx.cmd, data.TempColor, blitMaterial, 0);
            });
        }

        protected override bool ShouldCullPass(in RenderPassContext passContext, in CameraContext cameraContext)
        {
            var c = cameraContext.VolumeStack.GetComponent<TonemapperComponent>();
            return !c.active || c.type.value == TonemapType.None;
        }
    }
}