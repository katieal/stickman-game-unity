using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

namespace PixelCrushers.DialogueSystem
{

    [CustomFieldTypeService.Name("Variable")]
    public class CustomFieldType_Variable : CustomFieldType
    {
        public override string Draw(string currentValue, DialogueDatabase database)
        {
            RequireDialogueEditorWindow(database);
            return PixelCrushers.DialogueSystem.DialogueEditor.DialogueEditorWindow.instance.DrawAssetPopup<Variable>(currentValue, (database != null) ? database.variables : null, null);
        }
        public override string Draw(Rect rect, string currentValue, DialogueDatabase database)
        {
            return PixelCrushers.DialogueSystem.DialogueEditor.DialogueEditorWindow.instance.DrawAssetPopup<Variable>(rect, currentValue, (database != null) ? database.variables : null, null);
        }
    }
}