using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

namespace Radish.Rendering.Passes
{
    public enum DitherMode
    {
        None,
        Bayer4x4,
        BlueNoise
    }
    
    [Serializable]
    [SupportedOnRenderPipeline(typeof(RadishRenderPipelineAsset))]
    [UnityEngine.Categorization.CategoryInfo(Name = "Radish Dither Pass Resources")]
    internal class DitherPassResources : IRenderPipelineResources
    {
        public int version => 0;

        [ResourcePath("Hidden/Radish/TargetDither", SearchType.ShaderName)]
        [SerializeField] private Shader m_DitherShader;
        
        public bool isAvailableInPlayerBuild => true;
        public Shader ditherShader => m_DitherShader;
    }
    
    public sealed class DitherPassData
    {
        public TextureHandle SceneColor;
        public TextureHandle Temp;
        public int ShaderPass;
        public float DitherIntensity;
    }
    
    [PublicAPI]
    public sealed class DitherPass : RenderPass<DitherPassData>
    {
        public bool SkipForSceneView { get; set; } = true;
        
        private ResourceIdentifier m_ColorTexture;

        private static Material s_DitherMaterial;
        private static readonly int MainTexProp = Shader.PropertyToID("_MainTex");
        private static readonly int DitherIntensityProp = Shader.PropertyToID("_DitherIntensity");

        public DitherPass(string name, ResourceIdentifier colorTexture) : base(name)
        {
            m_ColorTexture = colorTexture;
        }

        protected override void SetupPass(DitherPassData data, in RenderPassContext passContext, in CameraContext cameraContext,
            ref RenderGraphBuilder builder)
        {
            var resources = GraphicsSettings.GetRenderPipelineSettings<DitherPassResources>();
            var settings = cameraContext.VolumeStack.GetComponent<DitherComponent>();
            
            InitMaterial(ref s_DitherMaterial, resources.ditherShader);
            
            var color = passContext.passManager.Get<TextureHandle>(m_ColorTexture);

            data.SceneColor = color.RegisterUse(builder.ReadWriteTexture(color.value));
            data.Temp = CreatePingPongTextureFrom(data.SceneColor, in passContext, ref builder);
            data.ShaderPass = settings.mode.value switch
            {
                DitherMode.Bayer4x4 => 0,
                DitherMode.BlueNoise => 1,
                _ => throw new ArgumentOutOfRangeException()
            };
            data.DitherIntensity = settings.intensity.value;
            
            builder.SetRenderFunc<DitherPassData>(static (data, ctx) =>
            {
                s_DitherMaterial.SetTexture(MainTexProp, data.Temp);
                s_DitherMaterial.SetFloat(DitherIntensityProp, data.DitherIntensity);
                
                ctx.cmd.CopyTexture(data.SceneColor, data.Temp);
                ctx.cmd.SetRenderTarget(data.SceneColor);
                BlitProcedural(ctx.cmd, data.Temp, s_DitherMaterial, data.ShaderPass);
            });
        }

        protected override bool ShouldCullPass(in RenderPassContext passContext, in CameraContext cameraContext)
        {
            if (SkipForSceneView && cameraContext.Camera.cameraType is CameraType.SceneView or CameraType.Preview)
                return true;
            
            var resources = GraphicsSettings.GetRenderPipelineSettings<DitherPassResources>();
            var settings = cameraContext.VolumeStack.GetComponent<DitherComponent>();

            return !resources.ditherShader ||
                   (settings.intensity.value <= 0 || settings.mode.value == DitherMode.None);
        }
    }
}