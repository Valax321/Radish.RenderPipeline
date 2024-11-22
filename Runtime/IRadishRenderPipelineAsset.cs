using UnityEngine.Rendering;

namespace Radish.Rendering
{
    public interface IRadishRenderPipelineAsset
    {
        bool lightsUseLinearIntensity { get; }
        bool lightsUseColorTemperature { get; }
        
        RenderPipelineAsset AsConcreteType() => (RenderPipelineAsset)this;
    }
}