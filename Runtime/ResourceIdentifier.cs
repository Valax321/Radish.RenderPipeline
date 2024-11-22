using System;
using UnityEngine;

namespace Radish.Rendering
{
    public readonly struct ResourceIdentifier : IEquatable<ResourceIdentifier>
    {
        public string name { get; }
        public int id { get; }

        public ResourceIdentifier(string name)
        {
            this.name = name;
            id = Shader.PropertyToID(this.name);
        }

        public bool Equals(ResourceIdentifier other)
        {
            return id == other.id;
        }

        public override bool Equals(object obj)
        {
            return obj is ResourceIdentifier other && Equals(other);
        }

        public override int GetHashCode()
        {
            return id;
        }

        public override string ToString()
        {
            return name;
        }
    }
}