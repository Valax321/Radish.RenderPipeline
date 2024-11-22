using UnityEngine.Rendering.RenderGraphModule;

namespace Radish.Rendering
{
    public readonly struct RenderPassContext
    {
        public readonly RadishRenderPipeline pipeline;
        public RenderPassManager passManager => pipeline.renderPassManager;
        public RenderGraph graph => pipeline.graph;

        public RenderPassContext(RadishRenderPipeline pipeline)
        {
            this.pipeline = pipeline;
        }
    }
}