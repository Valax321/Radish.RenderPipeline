using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

namespace Radish.Rendering
{
    public interface IRenderPassManagerProvider
    {
        public RenderGraph graph { get; }
        public RenderPassManager renderPassManager { get; }
        public RenderPipeline pipeline { get; }
    }
}