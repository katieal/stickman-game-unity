using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class TimeSelectorAttributeDrawer : OdinAttributeDrawer<TimeSelectorAttribute, int>
{
    static int[] hoursArray = Enumerable.Range(0, 24).ToArray();
    string[] hoursStrings = hoursArray.Select(i => string.Format("{0:00}", i)).ToArray();
    static int[] minutesArray = Enumerable.Range(0, 12).Select(x => x * 5).ToArray();
    //string[] minutesStrings = minutesArray.Select(i => i.ToString()).ToArray();

    protected override void DrawPropertyLayout(GUIContent label)
    {
        Rect rect = EditorGUILayout.GetControlRect();
        int value = this.ValueEntry.SmartValue;

        // property label
        if (label != null)
        {
            EditorGUI.LabelField(rect.AlignLeft(rect.width * 0.2f), label);
        }
        rect = rect.AlignRight(rect.width * 0.8f);

        // format is (rect1) HH hoursDropdown MM minutesDropdown  (rect2) Total totalMinutes
        // split rect dropdowns : total
        Rect rect1 = rect.AlignLeft(rect.width * 0.65f);
        Rect rect2 = rect.AlignRight(rect.width * 0.3f);

        // hours and minutes dropdwon
        GUIHelper.PushLabelWidth(50);
        EditorGUI.BeginChangeCheck();
        int hours = SirenixEditorFields.Dropdown<int>(rect1.AlignLeft(rect1.width * 0.5f), "HH", (value / 60), hoursArray, hoursStrings);
        int minutes = SirenixEditorFields.Dropdown<int>(rect1.AlignRight(rect1.width * 0.5f), new GUIContent("MM"), (value % 60), minutesArray);
        if (EditorGUI.EndChangeCheck())
        {
            this.ValueEntry.SmartValue = (hours * 60) + minutes;
        }
        GUIHelper.PopLabelWidth();

        // show current minutes value
        GUIHelper.PushLabelWidth(70);
        EditorGUI.BeginDisabledGroup(true);
        SirenixEditorFields.IntField(rect2, "Total", value);
        EditorGUI.EndDisabledGroup();
        GUIHelper.PopLabelWidth();
    }
}
