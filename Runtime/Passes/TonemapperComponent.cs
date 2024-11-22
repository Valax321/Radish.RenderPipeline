using UnityEngine.Rendering;

namespace Radish.Rendering.Passes
{
    [SupportedOnRenderPipeline(typeof(RadishRenderPipelineAsset))]
    [VolumeComponentMenu("Post Processing/Tonemapper")]
    public class TonemapperComponent : VolumeComponent
    {
        public EnumParameter<TonemapperPass.TonemapType> type = new(TonemapperPass.TonemapType.Neutral);
    }
}