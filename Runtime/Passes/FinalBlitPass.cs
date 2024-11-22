using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Rendering.RenderGraphModule;

namespace Radish.Rendering.Passes
{
    public sealed class FinalBlitPassData
    {
        public TextureHandle Dest;
        public TextureHandle Src;
        public Rect Viewport;
    }
    
    [PublicAPI]
    public sealed class FinalBlitPass : RenderPass<FinalBlitPassData>
    {
        private static readonly int BlitParamsProp = Shader.PropertyToID("_BlitParams");
        private static readonly int MainTexProp = Shader.PropertyToID("_MainTex");
        private ResourceIdentifier m_Source;
        private ResourceIdentifier m_Destination;
        
        public FinalBlitPass(string name, ResourceIdentifier source, ResourceIdentifier destination) : base(name)
        {
            m_Source = source;
            m_Destination = destination;
        }
        
        protected override void SetupPass(FinalBlitPassData data, in RenderPassContext passContext, in CameraContext cameraContext,
            ref RenderGraphBuilder builder)
        {
            var backbuffer = passContext.passManager.Get<TextureHandle>(m_Destination);
            var src = passContext.passManager.Get<TextureHandle>(m_Source);

            data.Dest = backbuffer.RegisterUse(builder.WriteTexture(backbuffer.value));
            data.Src = src.RegisterUse(builder.ReadTexture(src.value));
            data.Viewport = cameraContext.Camera.pixelRect;
            
            builder.SetRenderFunc<FinalBlitPassData>(static (data, ctx) =>
            {
                ctx.cmd.SetRenderTarget(data.Dest);
                ctx.cmd.SetViewport(data.Viewport);
                BlitProcedural(ctx.cmd, data.Src, blitMaterial, 0);
            });
        }
    }
}