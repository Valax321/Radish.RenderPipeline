using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Rendering;

namespace Radish.Rendering
{
    [SupportedOnRenderPipeline(typeof(RadishRenderPipelineAsset))]
    public abstract class RadishRenderPipelineSettings<TPipeline> 
        : RenderPipelineGlobalSettings<RadishRenderPipelineSettings<TPipeline>, TPipeline>
        where TPipeline : RenderPipeline
    {
        [SerializeField] private RenderPipelineGraphicsSettingsContainer m_Settings = new();
        protected override List<IRenderPipelineGraphicsSettings> settingsList => m_Settings.settingsList;

#if UNITY_EDITOR
        private static string GetDefaultPath() => $"Assets/Settings/{typeof(TPipeline).Name}GlobalSettings.asset";
        
        public static RadishRenderPipelineSettings<TPipeline> Ensure(bool canCreateNewAsset = true)
        {
            var currentInstance = GraphicsSettings.GetSettingsForRenderPipeline<TPipeline>() as RadishRenderPipelineSettings<TPipeline>;
            
            if (RenderPipelineGlobalSettingsUtils.TryEnsure<RadishRenderPipelineSettings<TPipeline>, TPipeline>(ref currentInstance, GetDefaultPath(),
                    canCreateNewAsset))
            {
                if (currentInstance)
                {
                    // Upgrade here if necessary
                    //EditorUtility.SetDirty(currentInstance);
                    AssetDatabase.SaveAssetIfDirty(currentInstance);
                }

                return currentInstance;
            }

            return null;
        }
#endif
    }
}