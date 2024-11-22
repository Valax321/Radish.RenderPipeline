using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

namespace Radish.Rendering.Passes
{
    public sealed class FXAAPassData
    {
        public TextureHandle SceneColor;
        public TextureHandle TempColor;
        public FXAAQuality Quality;
    }
    
    [Serializable]
    [SupportedOnRenderPipeline(typeof(RadishRenderPipelineAsset))]
    [UnityEngine.Categorization.CategoryInfo(Name = "Radish FXAA Pass Resources")]
    internal class FXAAResources : IRenderPipelineResources
    {
        public int version => 0;

        [ResourcePath("Hidden/Radish/FXAA", SearchType.ShaderName)]
        [SerializeField] private Shader m_FXAAShader;
        
        public bool isAvailableInPlayerBuild => true;
        public Shader fxaaShader => m_FXAAShader;
    }

    public enum FXAAQuality
    {
        High,
        Medium,
        Low,
        Off
    }
    
    [PublicAPI]
    public sealed class FXAAPass : RenderPass<FXAAPassData>
    {
        private static Material s_Material;
        private static LocalKeyword[] s_Keywords = new LocalKeyword[3];

        private ResourceIdentifier m_ColorTexture;

        public FXAAPass(string name, ResourceIdentifier colorTexture) : base(name)
        {
            m_ColorTexture = colorTexture;
        }

        protected override void SetupPass(FXAAPassData data, in RenderPassContext passContext, in CameraContext cameraContext,
            ref RenderGraphBuilder builder)
        {
            var resources = GraphicsSettings.GetRenderPipelineSettings<FXAAResources>();
            var settings = cameraContext.VolumeStack.GetComponent<FXAAComponent>();
            
            InitMaterial(ref s_Material, resources.fxaaShader);
            s_Keywords[0] = resources.fxaaShader.keywordSpace.FindKeyword("FXAA_QUALITY_LOW");
            s_Keywords[1] = resources.fxaaShader.keywordSpace.FindKeyword("FXAA_QUALITY_MEDIUM");
            s_Keywords[2] = resources.fxaaShader.keywordSpace.FindKeyword("FXAA_QUALITY_HIGH");

            var sceneColor = passContext.passManager.Get<TextureHandle>(m_ColorTexture);
            
            data.SceneColor = sceneColor.RegisterUse(builder.ReadWriteTexture(sceneColor.value));
            data.TempColor = CreatePingPongTextureFrom(data.SceneColor, in passContext, ref builder);
            data.Quality = settings.quality.value;
            
            builder.AllowPassCulling(false);
            
            builder.SetRenderFunc<FXAAPassData>(static (data, ctx) =>
            {
                s_Material.SetKeyword(s_Keywords[2], data.Quality == FXAAQuality.High);
                s_Material.SetKeyword(s_Keywords[1], data.Quality == FXAAQuality.Medium);
                s_Material.SetKeyword(s_Keywords[0], data.Quality == FXAAQuality.Low);
                
                ctx.cmd.Blit(data.SceneColor, data.TempColor, s_Material, 0);
                ctx.cmd.Blit(data.TempColor, data.SceneColor, s_Material, 1);
            });
        }

        protected override bool ShouldCullPass(in RenderPassContext passContext, in CameraContext cameraContext)
        {
            var settings = cameraContext.VolumeStack.GetComponent<FXAAComponent>();
            return !settings.active || settings.quality.value == FXAAQuality.Off;
        }
    }
}