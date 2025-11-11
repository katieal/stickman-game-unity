using Items;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector.Editor.ValueResolvers;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Utility
{
    public class DictionaryKeyAttributeDrawer : OdinAttributeDrawer<DictionaryKeyAttribute, string>
    {
        private ValueResolver<string[]> keysResolver;

        protected override void Initialize()
        {
            this.keysResolver = ValueResolver.Get<string[]>(this.Property, this.Attribute.KeyArrayName);
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            // draw errors if there are any
            ValueResolver.DrawErrors(this.keysResolver);

            // get values
            string[] keys = this.keysResolver.GetValue();

            // get current value
            int current = System.Array.IndexOf(keys, this.ValueEntry.SmartValue);
            if (current < 0) 
            {
                SirenixEditorGUI.ErrorMessageBox("Current value is unassigned or invalid");
                current = 0; 
            }

            // show popup only if dict has keys
            if (keys.Length > 0)
            {
                EditorGUI.BeginChangeCheck();
                int index = SirenixEditorFields.Dropdown(label, current, keys);

                if (EditorGUI.EndChangeCheck())
                {
                    this.ValueEntry.SmartValue = keys[index];
                }
            }
            else
            {
                SirenixEditorGUI.ErrorMessageBox("Dictionary is empty!");
            }
        }
    }
}