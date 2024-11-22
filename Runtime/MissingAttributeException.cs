using System;

namespace Radish.Rendering
{
    public class MissingAttributeException : Exception
    {
        public MissingAttributeException(Type attributeType) : base($"{attributeType.FullName} not found")
        {
        }
    }
}