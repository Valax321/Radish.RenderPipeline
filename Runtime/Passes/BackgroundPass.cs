using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Rendering.RenderGraphModule;

namespace Radish.Rendering.Passes
{
    public sealed class BackgroundPassData
    {
        public RendererListHandle Renderers;
    }
    
    [PublicAPI]
    public sealed class BackgroundPass : RenderPass<BackgroundPassData>
    {
        private ResourceIdentifier m_ColorAttachment;
        private ResourceIdentifier m_DepthAttachment;
        
        public BackgroundPass(string name, ResourceIdentifier colorAttachment, ResourceIdentifier depthAttachment) : base(name)
        {
            m_ColorAttachment = colorAttachment;
            m_DepthAttachment = depthAttachment;
        }

        protected override void SetupPass(BackgroundPassData data, in RenderPassContext passContext, in CameraContext cameraContext,
            ref RenderGraphBuilder builder)
        {
            var sceneColor = passContext.passManager.Get<TextureHandle>(m_ColorAttachment);
            var sceneDepth = passContext.passManager.Get<TextureHandle>(m_DepthAttachment);

            sceneColor.RegisterUse(builder.UseColorBuffer(sceneColor.value, 0));
            sceneDepth.RegisterUse(builder.UseDepthBuffer(sceneDepth.value, DepthAccess.Read));

            var renderers = passContext.graph.CreateSkyboxRendererList(in cameraContext.Camera);
            data.Renderers = builder.UseRendererList(in renderers);
            
            builder.AllowRendererListCulling(false);

            builder.SetRenderFunc<BackgroundPassData>(static (data, ctx) =>
            {
                ctx.cmd.DrawRendererList(data.Renderers);
            });
        }

        protected override bool ShouldCullPass(in RenderPassContext passContext, in CameraContext cameraContext)
        {
            return cameraContext.Camera.clearFlags != CameraClearFlags.Skybox;
        }
    }
}