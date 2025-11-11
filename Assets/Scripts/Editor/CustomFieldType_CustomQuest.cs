using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PixelCrushers.DialogueSystem
{

    [CustomFieldTypeService.Name("Custom Quest")]
    public class CustomFieldType_CustomQuest : CustomFieldType
    {
        [Title("References")]
        [SerializeField] private Managers.QuestManager _questManager;

        public GUIContent[] questTitles = null;
        private List<QuestPopupInfoHolder> _popupInfo;

        public override string Draw(string currentValue, DialogueDatabase database)
        {
            return EditorGUILayout.TextField(currentValue);
        }


        public override string Draw(Rect rect, string currentValue, DialogueDatabase database)
        {
            return EditorGUI.TextField(rect, currentValue);
        }

        private void GetQuests()
        {
            if (questTitles != null) return; // Already initialized.
            List<GUIContent> list = new();

            //List<Characters.Quests.QuestData> data;


            questTitles = list.ToArray();
        }


        public struct QuestPopupInfoHolder
        {
            public string Name;
            public Guid Guid;
        }
    }
}

namespace PixelCrushers.DialogueSystem
{

    [CustomFieldTypeService.Name("Conversation")]
    public class CustomFieldType_Conversation : CustomFieldType
    {
        public GUIContent[] conversationTitles = null;

        public override string Draw(string currentValue, DialogueDatabase database)
        {
            InitializeConversationTitles();
            if (conversationTitles == null)
            {
                return EditorGUILayout.TextField(currentValue);
            }
            EditorGUI.BeginChangeCheck();
            var index = EditorGUILayout.Popup(GUIContent.none, GetIndex(currentValue), conversationTitles);
            if (EditorGUI.EndChangeCheck())
            {
                return (1 <= index && index < conversationTitles.Length) ? conversationTitles[index].text : string.Empty;
            }
            else
            {
                return currentValue;
            }
        }

        public override string Draw(Rect rect, string currentValue, DialogueDatabase database)
        {
            InitializeConversationTitles();
            if (conversationTitles == null)
            {
                return EditorGUILayout.TextField(currentValue);
            }
            EditorGUI.BeginChangeCheck();
            var index = EditorGUI.Popup(rect, GUIContent.none, GetIndex(currentValue), conversationTitles);
            if (EditorGUI.EndChangeCheck())
            {
                return (1 <= index && index < conversationTitles.Length) ? conversationTitles[index].text : string.Empty;
            }
            else
            {
                return currentValue;
            }
        }

        protected void InitializeConversationTitles()
        {
            if (conversationTitles != null) return; // Already initialized.
            var list = new List<GUIContent>();
            list.Add(new GUIContent("(None)"));
            var database = EditorTools.FindInitialDatabase();
            if (database != null)
            {
                foreach (var conversation in database.conversations)
                {
                    list.Add(new GUIContent(conversation.Title));
                }
            }
            conversationTitles = list.ToArray();
        }

        protected int GetIndex(string currentValue)
        {
            if (conversationTitles != null)
            {
                for (int i = 1; i < conversationTitles.Length; i++)
                {
                    if (string.Equals(currentValue, conversationTitles[i]))
                    {
                        return i;
                    }
                }
            }
            return 0;
        }
    }
}