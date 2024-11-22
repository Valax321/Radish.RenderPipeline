using UnityEngine.Rendering;

namespace Radish.Rendering.Passes
{
    [SupportedOnRenderPipeline(typeof(RadishRenderPipelineAsset))]
    [VolumeComponentMenu("Post Processing/FXAA")]
    public class FXAAComponent : VolumeComponent
    {
        public EnumParameter<FXAAQuality> quality = new(FXAAQuality.High);
    }
}