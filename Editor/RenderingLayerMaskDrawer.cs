#if ODIN_INSPECTOR
using System.Collections.Generic;
using JetBrains.Annotations;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Radish.Rendering
{
    [UsedImplicitly]
    internal class RenderingLayerMaskDrawer : OdinValueDrawer<RenderingLayerMask>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var rect = EditorGUILayout.GetControlRect();
            if (label != null)
                rect = EditorGUI.PrefixLabel(rect, label);

            var value = ValueEntry.SmartValue;
            var selection = GetSelectedItemsFromMask(value.value);
            var items = RenderingLayerMask.GetDefinedRenderingLayerNames();
            if (SirenixEditorFields.Dropdown(rect, selection, items, true))
            {
                ValueEntry.SmartValue = new RenderingLayerMask
                {
                    value = SelectionToMask(selection)
                };
            }
        }

        private static IList<int> GetSelectedItemsFromMask(uint mask)
        {
            var values = RenderingLayerMask.GetDefinedRenderingLayerValues();
            var l = new List<int>();
            for (var i = 0; i < RenderingLayerMask.GetDefinedRenderingLayerCount(); ++i)
            {
                if ((mask & values[i]) != 0)
                    l.Add(i);
            }
            return l;
        }

        private static uint SelectionToMask(IList<int> mask)
        {
            uint v = 0;
            var values = RenderingLayerMask.GetDefinedRenderingLayerValues();
            foreach (var i in mask)
            {
                v |= (uint)values[i];
            }

            return v;
        }
    }
}
#endif