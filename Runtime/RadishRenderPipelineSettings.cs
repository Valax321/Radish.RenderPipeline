using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Rendering;

namespace Radish.Rendering
{
    [SupportedOnRenderPipeline(typeof(RadishRenderPipelineAsset))]
    public abstract class RadishRenderPipelineSettings<TSelf, TPipeline> 
        : RenderPipelineGlobalSettings<TSelf, TPipeline>
        where TSelf : RenderPipelineGlobalSettings<TSelf, TPipeline>
        where TPipeline : RenderPipeline
    {
        [SerializeField] private RenderPipelineGraphicsSettingsContainer m_Settings = new();
        protected override List<IRenderPipelineGraphicsSettings> settingsList => m_Settings.settingsList;

#if UNITY_EDITOR
        protected static string GetDefaultPath() => $"Assets/Settings/{typeof(TPipeline).Name}GlobalSettings.asset";
        
        public static TSelf Ensure(bool canCreateNewAsset = true)
        {
            var currentInstance = GraphicsSettings.GetSettingsForRenderPipeline<TPipeline>() as TSelf;
            
            if (RenderPipelineGlobalSettingsUtils.TryEnsure<TSelf, TPipeline>(ref currentInstance, GetDefaultPath(),
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