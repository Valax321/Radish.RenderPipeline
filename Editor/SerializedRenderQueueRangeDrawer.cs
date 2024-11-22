#if ODIN_INSPECTOR
using JetBrains.Annotations;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Radish.Rendering
{
    [UsedImplicitly]
    internal class SerializedRenderQueueRangeDrawer : OdinValueDrawer<SerializedRenderQueueRange>
    {
        private enum Preset
        {
            All,
            Opaque,
            Transparent,
            Custom
        }
        
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var rect = EditorGUILayout.GetControlRect();
            if (label != null)
                rect = EditorGUI.PrefixLabel(rect, label);

            var value = ValueEntry.SmartValue;
            float min = value.Min;
            float max = value.Max;

            var firstRect = rect.Split(0, 2);
            var secondRect = rect.Split(1, 2);

            SirenixEditorFields.MinMaxSlider(firstRect, ref min, ref max, RenderQueueRange.minimumBound, RenderQueueRange.maximumBound, true);
            
            var preset = Preset.Custom;
            if (value == RenderQueueRange.all)
                preset = Preset.All;
            else if (value == RenderQueueRange.opaque)
                preset = Preset.Opaque;
            else if (value == RenderQueueRange.transparent)
                preset = Preset.Transparent;
            
            preset = (Preset)SirenixEditorFields.EnumDropdown(secondRect, preset);
            value = preset switch
            {
                Preset.Opaque => RenderQueueRange.opaque,
                Preset.Transparent => RenderQueueRange.transparent,
                Preset.All => RenderQueueRange.all,
                _ => value
            };

            ValueEntry.SmartValue = value;
        }
    }
}
#endif