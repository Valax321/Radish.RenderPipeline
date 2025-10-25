using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

namespace Radish.Rendering
{
    /// <summary>
    /// TODO: this is kinda redundant now, refactor this out at some point. Any further exposed properties should be a part of IRenderPassManagerProvider instead.
    /// </summary>
    public readonly struct RenderPassContext
    {
        public RenderPipeline pipeline => m_Provider.pipeline;
        public RenderPassManager passManager => m_Provider.renderPassManager;
        public RenderGraph graph => m_Provider.graph;

        private readonly IRenderPassManagerProvider m_Provider;

        public RenderPassContext(IRenderPassManagerProvider pipeline)
        {
            m_Provider = pipeline;
        }
    }
}