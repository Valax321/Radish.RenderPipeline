using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Rendering;

namespace Radish.Rendering
{
    [PublicAPI]
    public static class MaterialPropertyBlockPool
    {
        private static readonly ObjectPool<MaterialPropertyBlock> s_Pool = new(GetPropertyBlock, ReleasePropertyBlock);

        public static MaterialPropertyBlock Get()
        {
            return s_Pool.Get();
        }

        public static void Release(MaterialPropertyBlock block)
        {
            s_Pool.Release(block);
        }
        
        private static void GetPropertyBlock(MaterialPropertyBlock block)
        {
            block.Clear();
        }

        private static void ReleasePropertyBlock(MaterialPropertyBlock block)
        {
        }
    }
}