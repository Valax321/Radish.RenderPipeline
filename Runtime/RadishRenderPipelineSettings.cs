using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Radish.Rendering
{
    [SupportedOnRenderPipeline(typeof(RadishRenderPipelineAsset))]
    public class RadishRenderPipelineSettings 
        : RenderPipelineGlobalSettings<RadishRenderPipelineSettings, RadishRenderPipeline>
    {
        [SerializeReference] private List<IRenderPipelineGraphicsSettings> m_Settings = new();
        protected override List<IRenderPipelineGraphicsSettings> settingsList => m_Settings;
    }
}