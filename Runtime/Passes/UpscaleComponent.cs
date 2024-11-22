using UnityEngine.Rendering;

namespace Radish.Rendering.Passes
{
    [SupportedOnRenderPipeline(typeof(RadishRenderPipelineAsset))]
    [VolumeComponentMenu("Post Processing/Upscale")]
    public class UpscaleComponent : VolumeComponent
    {
        public EnumParameter<ResolutionScaleMethod> upscaleMethod = new(ResolutionScaleMethod.Bilinear);
        public BoolParameter sharpenImage = new(true);
    }
}