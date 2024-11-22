using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine.Rendering;

namespace Radish.Rendering
{
    [InitializeOnLoad]
    internal static class SettingsInitializer
    {
        static SettingsInitializer()
        {
            RenderPipelineManager.activeRenderPipelineCreated += OnPipelineCreated;
        }

        private static void OnPipelineCreated()
        {
            if (RenderPipelineManager.currentPipeline is RadishRenderPipeline)
            {
                var t = EditorGraphicsSettings.GetRenderPipelineGlobalSettingsAsset(RenderPipelineManager.currentPipeline
                    .GetType());

                if (!t)
                {
                    var settings = ObjectFactory.CreateInstance<RadishRenderPipelineSettings>();
                    AssetDatabase.CreateAsset(settings,
                        $"Assets/{RenderPipelineManager.currentPipeline.GetType().Name}_Settings.asset");
                    
                    EditorGraphicsSettings.SetRenderPipelineGlobalSettingsAsset(RenderPipelineManager.currentPipeline.GetType(), settings);
                }
            }
        }
    }
}