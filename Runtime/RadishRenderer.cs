using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Radish.Rendering
{
    [PublicAPI]
    public abstract class RadishRenderer : ScriptableObject
    {
        private readonly List<RenderPassBase> m_Passes = new();
        private bool m_Initialized;
        private RenderPassManager m_RenderPassManager;
        
        protected abstract void SetupFrameResources(RadishRenderPipeline pipeline);
        protected abstract void SetupFramePasses(RadishRenderPipeline pipeline);
        protected virtual void OnInitialized(RadishRenderPipeline pipeline) { }
        
        public void Initialize(RadishRenderPipeline pipeline)
        {
            if (!m_Initialized)
            {
                m_RenderPassManager = pipeline.renderPassManager;
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

        public void Invalidate()
        {
            m_RenderPassManager.InvalidateInitializers();
            m_RenderPassManager = null;
            m_Passes.Clear();
            m_Initialized = false;
        }
    }
}