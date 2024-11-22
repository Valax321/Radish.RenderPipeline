using JetBrains.Annotations;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

namespace Radish.Rendering.Passes
{
    public sealed class GizmoPassData
    {
        public RendererListHandle Renderers;
    }
    
    [PublicAPI]
    public sealed class GizmoPass : RenderPass<GizmoPassData>
    {
        private GizmoSubset m_Subset;

        private ResourceIdentifier m_ColorAttachment;
        private ResourceIdentifier m_DepthAttachment;

        public GizmoPass(string name, GizmoSubset subset, ResourceIdentifier colorAttachment, ResourceIdentifier depthAttachment) : base(name)
        {
            m_Subset = subset;
            m_ColorAttachment = colorAttachment;
            m_DepthAttachment = depthAttachment;
        }

        protected override void SetupPass(GizmoPassData data, in RenderPassContext passContext, in CameraContext cameraContext,
            ref RenderGraphBuilder builder)
        {
            var sceneColor = passContext.passManager.Get<TextureHandle>(m_ColorAttachment);
            var sceneDepth = passContext.passManager.Get<TextureHandle>(m_DepthAttachment);
            
            sceneColor.RegisterUse(builder.UseColorBuffer(sceneColor.value, 0));
            sceneDepth.RegisterUse(builder.UseDepthBuffer(sceneDepth.value, DepthAccess.Read));

            var renderers = passContext.graph.CreateGizmoRendererList(in cameraContext.Camera, in m_Subset);
            data.Renderers = builder.UseRendererList(in renderers);
            
            builder.AllowRendererListCulling(false);

            builder.SetRenderFunc<GizmoPassData>(static (data, ctx) =>
            {
                ctx.cmd.DrawRendererList(data.Renderers);
            });
        }

        protected override bool ShouldCullPass(in RenderPassContext passContext, in CameraContext cameraContext)
        {
#if UNITY_EDITOR
            return !UnityEditor.Handles.ShouldRenderGizmos();
#else
            return true;
#endif
        }
    }
}