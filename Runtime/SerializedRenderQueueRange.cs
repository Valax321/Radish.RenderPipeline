using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Rendering;

namespace Radish.Rendering
{
    [Serializable, PublicAPI]
    public struct SerializedRenderQueueRange : IEquatable<SerializedRenderQueueRange>
    {
        [SerializeField] private int m_Min;
        [SerializeField] private int m_Max;

        public int Min
        {
            get => m_Min;
            set => m_Min = value;
        }

        public int Max
        {
            get => m_Max;
            set => m_Max = value;
        }

        public SerializedRenderQueueRange(int min, int max)
        {
            m_Min = min;
            m_Max = max;
        }

        public static implicit operator RenderQueueRange(SerializedRenderQueueRange range)
        {
            return new RenderQueueRange(range.m_Min, range.m_Max);
        }

        public static implicit operator SerializedRenderQueueRange(RenderQueueRange range)
        {
            return new SerializedRenderQueueRange(range.lowerBound, range.upperBound);
        }

        public bool Equals(SerializedRenderQueueRange other)
        {
            return m_Min == other.m_Min && m_Max == other.m_Max;
        }

        public override bool Equals(object obj)
        {
            return obj is SerializedRenderQueueRange other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(m_Min, m_Max);
        }
    }
}