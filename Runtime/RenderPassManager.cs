using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Rendering.RenderGraphModule;

namespace Radish.Rendering
{
    public class RenderPassManager
    {
        public struct DataInitContext
        {
            public ResourceIdentifier id { get; internal set; }
            public Camera camera { get; internal set; }
            public RenderPassManager passManager => pipeline.renderPassManager;
            public RadishRenderPipeline pipeline { get; internal set; }
            public RenderGraph graph => pipeline.graph;
        }

        public readonly struct DataWrapper<T>
        {
            public T value { get; }
            public ResourceIdentifier id { get; }
            private RenderPassManager owner { get; }

            public DataWrapper(T value, ResourceIdentifier id, RenderPassManager owner)
            {
                this.value = value;
                this.id = id;
                this.owner = owner;
            }

            public T RegisterUse(T newValue)
            {
                return owner.RegisterUse(id, newValue);
            }
        }
        
        public RadishRenderPipeline pipeline { get; }

        private readonly Dictionary<int, object> m_FrameData = new();

        private readonly Dictionary<int, Func<DataInitContext, object>> m_DataInitializers = new();

        private Camera m_ActiveCamera;
        
        public RenderPassManager(RadishRenderPipeline pipeline)
        {
            this.pipeline = pipeline;
        }

        public void BeginCameraFrame(Camera camera)
        {
            m_ActiveCamera = camera;
        }

        public void EndCameraFrame()
        {
            m_FrameData.Clear();
            m_ActiveCamera = null;
        }

        public void BuildResource<TData>(ResourceIdentifier id, Func<DataInitContext, TData> initFunc)
        {
            m_DataInitializers[id.id] = c => initFunc(c);
        }

        [PublicAPI]
        public DataWrapper<TData> Get<TData>(in ResourceIdentifier id)
        {
            if (m_ActiveCamera is null)
                throw new NullReferenceException("Active camera is null");

            if (!m_FrameData.TryGetValue(id.id, out var resource))
            {
                if (!m_DataInitializers.TryGetValue(id.id, out var init))
                    throw new GraphicsResourceException($"Could not find initializer for '{id}'");

                resource = init(new DataInitContext
                {
                    camera = m_ActiveCamera,
                    pipeline = pipeline,
                    id = id
                });
                m_FrameData.Add(id.id, resource);
            }
            
            if (resource is TData data)
                return new DataWrapper<TData>(data, id, this);

            throw new GraphicsResourceException($"Resource '{id}' is not a {typeof(TData).FullName}");
        }

        public TData RegisterUse<TData>(ResourceIdentifier id, TData data)
        {
            m_FrameData[id.id] = data;
            return data;
        }

        public void InvalidateInitializers()
        {
            m_DataInitializers.Clear();
        }
    }
}