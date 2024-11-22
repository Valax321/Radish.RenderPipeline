using UnityEngine;
using UnityEngine.Rendering;

namespace Radish.Rendering
{
    public readonly struct CameraContext
    {
        public readonly Camera Camera;
        public readonly CullingResults CullResults;
        public VolumeStack VolumeStack => VolumeManager.instance.stack;

        public CameraContext(Camera camera, in CullingResults cullResults)
        {
            Camera = camera;
            CullResults = cullResults;
        }
    }
}