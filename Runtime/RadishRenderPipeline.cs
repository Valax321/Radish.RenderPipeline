using System;
using Radish.Rendering.Settings;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

namespace Radish.Rendering
{
    public abstract class RadishRenderPipeline : RenderPipeline
    {
        public RadishRenderPipelineAsset asset { get; }
        public RenderGraph graph { get; private set; }
        public RenderPassManager renderPassManager { get; private set; }
        
        public RadishRenderPipeline(RadishRenderPipelineAsset asset)
        {
            this.asset = asset;

            GraphicsSettings.lightsUseLinearIntensity = asset.lightsUseLinearIntensity;
            GraphicsSettings.lightsUseColorTemperature = asset.lightsUseColorTemperature;

            var volumeProfileSettings = GraphicsSettings.GetRenderPipelineSettings<RadishDefaultVolumeSettings>();
            VolumeManager.instance.Initialize(volumeProfileSettings.volumeProfile, asset.defaultVolumeProfile);
        }

        protected override void Dispose(bool disposing)
        {
            CleanupRenderGraph();
            VolumeManager.instance.Deinitialize();
            
            base.Dispose(disposing);
        }

        protected override void Render(ScriptableRenderContext context, Camera[] cameras)
        {
            GraphicsSettings.useScriptableRenderPipelineBatching = asset.useSRPBatcher;
            try
            {
                InitializeRenderGraph();
                BeginFrameRendering(context, cameras);
                
                foreach (var camera in cameras)
                    RenderCamera(camera, context);

                EndFrameRendering(context, cameras);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex, asset);
                // If we get here the render graph is likely borked,
                // so we clean it up and try again fresh.
                CleanupRenderGraph();
            }
        }

        private void RenderCamera(Camera camera, ScriptableRenderContext context)
        {
            BeginCameraRendering(context, camera);
            GetCameraVolumeManagerData(camera, out var mask, out var t);
            
            VolumeManager.instance.Update(t, mask);
            
            if (!camera.TryGetCullingParameters(out var cameraCullingParams))
                return;
            
            var cameraCullResults = context.Cull(ref cameraCullingParams);
            
            context.SetupCameraProperties(camera);

            var sampler = new ProfilingSampler(camera.name);
            var buffer = CommandBufferPool.Get(camera.name);
            var graphParams = new RenderGraphParameters
            {
                commandBuffer = buffer,
                currentFrameIndex = Time.frameCount,
                scriptableRenderContext = context,
                executionName = sampler.name,
                rendererListCulling = true
            };
            
            graph.BeginRecording(in graphParams);
            {
                using var _ = new RenderGraphProfilingScope(graph, sampler);
                RecordCameraPasses(new CameraContext(camera, in cameraCullResults));
            }
            graph.EndRecordingAndExecute();
            
            context.ExecuteCommandBuffer(buffer);
            CommandBufferPool.Release(buffer);
            context.Submit();
            
            EndCameraRendering(context, camera);
        }

        protected virtual void GetCameraVolumeManagerData(Camera camera, out LayerMask layerMask,
            out Transform transform)
        {
            layerMask = new LayerMask { value = int.MaxValue };
            transform = camera.transform;
        }
        
        private void RecordCameraPasses(in CameraContext cameraContext)
        {
            if (!asset.renderer)
            {
                Debug.LogError("No renderer assigned", asset);
                return;
            }
            
            renderPassManager.BeginCameraFrame(cameraContext.Camera);
            asset.renderer.RecordCameraPasses(this, in cameraContext);
            renderPassManager.EndCameraFrame();
        }

        private void InitializeRenderGraph()
        {
            graph ??= new RenderGraph(GetType().Name);
            renderPassManager ??= new RenderPassManager(this);
            asset.renderer?.Initialize(this);
        }

        private void CleanupRenderGraph()
        {
            asset.renderer?.Cleanup();
            renderPassManager = null;
            
            if (graph is not null)
            {
                graph.Cleanup();
                graph = null;
            }
        }
    }
}