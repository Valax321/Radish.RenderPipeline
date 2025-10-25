namespace Radish.Rendering
{
    public abstract class RenderPassBase
    {
        public abstract void AddToGraph(IRenderPassManagerProvider pipeline, in CameraContext cameraContext);
    }
}