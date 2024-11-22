using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Radish.Rendering
{
    [PublicAPI]
    public abstract class Renderer : ScriptableObject
    {
        private readonly List<RenderPassBase> m_Passes = new();
        private bool m_Initialized;
        
        protected abstract void SetupFrameResources(RadishRenderPipeline pipeline);
        protected abstract void SetupFramePasses(RadishRenderPipeline pipeline);
        protected virtual void OnInitialized(RadishRenderPipeline pipeline) { }
        
        public void Initialize(RadishRenderPipeline pipeline)
        {
            if (!m_Initialized)
            {
                SetupFrameResources(pipeline);
                SetupFramePasses(pipeline);
                OnInitialized(pipeline);
                m_Initialized = true;
            }
        }

        protected void EnqueuePass<T>(T pass) where T : RenderPassBase
        {
            m_Passes.Add(pass);
        }

        public virtual void RecordCameraPasses(RadishRenderPipeline pipeline, in CameraContext cameraContext)
        {
            foreach (var pass in m_Passes)
            {
                pass.AddToGraph(pipeline, in cameraContext);
            }
        }

        public void Cleanup()
        {
            m_Passes.Clear();
            m_Initialized = false;
        }
    }
}