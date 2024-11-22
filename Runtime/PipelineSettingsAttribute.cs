using System;

namespace Radish.Rendering
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class PipelineSettingsAttribute : Attribute
    {
        public Type type { get; }
        
        public PipelineSettingsAttribute(Type settingsType)
        {
            type = settingsType;
        }
    }
}