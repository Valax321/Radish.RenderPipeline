using System;
using UnityEngine.Rendering;

namespace Radish.Rendering
{
    public abstract class RadishRenderPipelineAsset : RenderPipelineAsset
    {
        protected sealed override RenderPipeline CreatePipeline()
        {
            return (RenderPipeline)Activator.CreateInstance(pipelineType, new object[] { this });
        }

        public sealed override string renderPipelineShaderTag => pipelineType.Name;
        public virtual bool lightsUseLinearIntensity => true;
        public virtual bool lightsUseColorTemperature => true;
        public abstract VolumeProfile defaultVolumeProfile { get; }
        public abstract Renderer renderer { get; }
        public virtual bool useSRPBatcher => true;
        public virtual bool allowDynamicResolution => false;
        public virtual MSAASamples msaaSamples => MSAASamples.None;
    }
}