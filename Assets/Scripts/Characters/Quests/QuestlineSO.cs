using PixelCrushers.DialogueSystem;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Characters.Quests
{
    [CreateAssetMenu(fileName = "QuestlineSO", menuName = "Scriptable Objects/Questline")]
    public class QuestlineSO : SerializedScriptableObject
    {
        [Title("Questline Details")]
        public string Source;
        [InlineButton(nameof(CopyGuid))] public string Name;
        [ReadOnly] public readonly Guid Guid = Guid.NewGuid();
        public string Description;
        [EnumToggleButtons, HideLabel] public QuestStatus Status;
        [ShowIf("Status", QuestStatus.Active)] public int CurrentIndex = 0;

        [Title("Dialogue")]
        [ConversationPopup] public string Conversation;

#if UNITY_EDITOR
        [FoldoutGroup("Dialogue Assignment")]
        [ValueDropdown("GetQuestIndexList")]
        public int QuestNumber;

        [BoxGroup("Dialogue Assignment/Entries")]
        [DialogueEntryPopup] public int OnPrompt;
        [BoxGroup("Dialogue Assignment/Entries")]
        [DialogueEntryPopup] public int OnAccept;
        [BoxGroup("Dialogue Assignment/Entries")]
        [DialogueEntryPopup] public int OnReject;
        [BoxGroup("Dialogue Assignment/Entries")]
        [DialogueEntryPopup] public int OnCheckin;
        [BoxGroup("Dialogue Assignment/Entries")]
        [DialogueEntryPopup] public int OnComplete;

        [ButtonGroup("Dialogue Assignment/Button")]
        public void AssignEntryFields()
        {
            Quests[QuestNumber - 1].Conversation = Conversation;
            Quests[QuestNumber - 1].OnPrompt = OnPrompt;
            Quests[QuestNumber - 1].OnAccept = OnAccept;
            Quests[QuestNumber - 1].OnReject = OnReject;
            Quests[QuestNumber - 1].OnCheckin = OnCheckin;
            Quests[QuestNumber - 1].OnComplete = OnComplete;
        }
#endif

        [Title("Quests")]
        [NonSerialized, OdinSerialize]
        [ListDrawerSettings(AddCopiesLastElement = false, CustomAddFunction = "CustomAddQuest", OnTitleBarGUI = "DrawRefreshButton")]
        public List<Quest> Quests = new List<Quest>();

        [HideInInspector]
        public Quest CurrentQuest { get { return Quests[CurrentIndex]; } }


#if UNITY_EDITOR
        [Button]
        public void ResetQuestline()
        {
            CurrentIndex = 0;
            this.Status = QuestStatus.Inactive;
            foreach (Quest quest in Quests)
            {
                quest.Status = QuestStatus.Inactive;

                foreach (IQuestMission mission in quest.Missions)
                {
                    mission.Reset();
                }
            }
        }

        private IEnumerable<int> GetQuestIndexList()
        {
            return Enumerable.Range(1, Quests.Count);
        }
        private Quest CustomAddQuest()
        {
            return new Quest()
            {
                QuestlineName = this.Name,
                Guid = Guid.NewGuid(),
                QuestOrder = Quests.Count
            };
        }

        private void DrawRefreshButton()
        {
            if (SirenixEditorGUI.ToolbarButton(EditorIcons.Refresh))
            {
                for (int i = 0; i < Quests.Count; i++)
                {
                    Quests[i].SetOrder(i);
                    Quests[i].QuestlineName = this.Name;
                }
            }
        }
        private void CopyGuid() { GUIUtility.systemCopyBuffer = this.Guid.ToString(); }
#endif

    }

}