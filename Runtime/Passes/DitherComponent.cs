using UnityEngine.Rendering;

namespace Radish.Rendering.Passes
{
    [SupportedOnRenderPipeline(typeof(RadishRenderPipelineAsset))]
    [VolumeComponentMenu("Rendering/Dithering")]
    public class DitherComponent : VolumeComponent
    {
        public EnumParameter<DitherMode> mode = new(DitherMode.None);
        public ClampedFloatParameter intensity = new(0.05f, 0.0f, 1.0f);
    }
}