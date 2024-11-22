using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Radish.Rendering.Settings
{
    [Serializable]
    [SupportedOnRenderPipeline(typeof(RadishRenderPipelineAsset))]
    [UnityEngine.Categorization.CategoryInfo(Name = "Radish Builtin Resources")]
    public class RadishBuiltinResources : IRenderPipelineResources
    {
        [ResourcePath("Hidden/Radish/ColorSpaceConvert", SearchType.ShaderName)]
        [SerializeField] private Shader m_ColorSpaceConvertShader;

        [ResourcePath("Hidden/Radish/CameraTargetBlit", SearchType.ShaderName)]
        [SerializeField] private Shader m_BlitShader;
        
        public int version => 0;
        public bool isAvailableInPlayerBuild => true;

        public Shader colorSpaceConvertShader => m_ColorSpaceConvertShader;
        public Shader blitShader => m_BlitShader;
    }
}