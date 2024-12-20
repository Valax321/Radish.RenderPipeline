using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Radish.Rendering.Settings
{
    [Serializable]
    [SupportedOnRenderPipeline(typeof(RadishRenderPipelineAsset))]
    [UnityEngine.Categorization.CategoryInfo(Name = "Default Volume Settings")]
    public class RadishDefaultVolumeSettings : IDefaultVolumeProfileSettings
    {
        [SerializeField] private VolumeProfile m_Profile;
        
        public int version => 0;

        public bool isAvailableInPlayerBuild => true;

        public VolumeProfile volumeProfile
        {
            get => m_Profile;
            set => m_Profile = value;
        }
    }
}