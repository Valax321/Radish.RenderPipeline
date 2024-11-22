using JetBrains.Annotations;
using UnityEngine;

namespace Radish.Rendering
{
    [PublicAPI]
    public static class GraphicsUtility
    {
        public static bool isSRGB => QualitySettings.activeColorSpace == ColorSpace.Linear;
    }
}