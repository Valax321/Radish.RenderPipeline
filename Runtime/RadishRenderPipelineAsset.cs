using System;
using UnityEngine.Rendering;

namespace Radish.Rendering
{
    /// <summary>
    /// Base class for render pipeline assets using Radish features.
    /// </summary>
    /// <remarks>
    /// Some notes about how this all must work:
    /// <list type="bullet">
    ///     <item>
    ///         You *must* implement child classes for <see cref="RadishRenderPipelineAsset"/>, <see cref="RadishRenderPipeline"/> and <see cref="RadishRenderPipelineSettings{TPipeline}"/>
    ///     </item>
    ///     <item>
    ///         To get the global settings working properly in the editor UI,
    ///         you need to override <see cref="RenderPipelineAsset.EnsureGlobalSettings"/> and call <see cref="RadishRenderPipelineSettings{TPipeline}.Ensure"/>.
    ///     </item>
    ///     <item>
    ///         Mark up your global settings class with <see cref=" UnityEngine.Rendering.DisplayInfoAttribute"/> and <see cref="System.ComponentModel.DisplayNameAttribute"/>.
    ///     </item>
    ///     <item>
    ///         Use <see cref="UnityEngine.Categorization.CategoryInfoAttribute"/> on your <see cref="IRenderPipelineGraphicsSettings"/> and <see cref="IRenderPipelineResources"/> types to alter display name/order.
    ///     </item>
    /// </list>
    /// </remarks>
    public abstract class RadishRenderPipelineAsset : RenderPipelineAsset
    {
        protected sealed override RenderPipeline CreatePipeline()
        {
            return (RenderPipeline)Activator.CreateInstance(pipelineType, new object[] { this });
        }

        public sealed override string renderPipelineShaderTag => pipelineType.Name;
        public virtual bool lightsUseLinearIntensity => true;
        public virtual bool lightsUseColorTemperature => true;
        public abstract VolumeProfile defaultVolumeProfile { get; }
        public abstract RadishRenderer renderer { get; }
        public virtual bool useSRPBatcher => true;
    }
}