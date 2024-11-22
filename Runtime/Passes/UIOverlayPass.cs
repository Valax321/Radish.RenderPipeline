using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Rendering.RenderGraphModule;

namespace Radish.Rendering.Passes
{
    public sealed class UIOverlayPassData
    {
        public RendererListHandle Renderers;
    }

    [PublicAPI]
    public sealed class UIOverlayPass : RenderPass<UIOverlayPassData>
    {
        private ResourceIdentifier m_ColorTexture;
        
        public UIOverlayPass(string name, ResourceIdentifier colorAttachment) : base(name)
        {
            m_ColorTexture = colorAttachment;
        }

        protected override void SetupPass(UIOverlayPassData data, in RenderPassContext passContext, in CameraContext cameraContext,
            ref RenderGraphBuilder builder)
        {
            var cameraTarget = passContext.passManager.Get<TextureHandle>(m_ColorTexture);
            
            cameraTarget.RegisterUse(builder.UseColorBuffer(cameraTarget.value, 0));
            data.Renderers =
                builder.UseRendererList(passContext.graph.CreateUIOverlayRendererList(cameraContext.Camera));
            
            builder.AllowPassCulling(false);
            
            builder.SetRenderFunc<UIOverlayPassData>(static (data, ctx) =>
            {
                ctx.cmd.DrawRendererList(data.Renderers);
            });
        }

        protected override bool ShouldCullPass(in RenderPassContext passContext, in CameraContext cameraContext)
        {
            return cameraContext.Camera.cameraType is not CameraType.Game;
        }
    }
}