using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Radish.Rendering.Settings
{
    [Serializable]
    public class RadishBuiltinResources : IRenderPipelineResources
    {
        [ResourcePath("Hidden/Radish/ColorSpaceConvert", SearchType.ShaderName)]
        [SerializeField] private Shader m_ColorSpaceConvertShader;
        
        public int version => 0;

        public Shader colorSpaceConvertShader => m_ColorSpaceConvertShader;
    }
}