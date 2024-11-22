namespace Radish.Rendering
{
    public abstract class RenderPassBase
    {
        public abstract void AddToGraph(RadishRenderPipeline pipeline, in CameraContext cameraContext);
    }
}