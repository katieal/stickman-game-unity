using Characters.Quests;
using NUnit.Framework.Interfaces;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UserInterface
{
    public class QuestLogWindow : MonoBehaviour
    {
        [Title("Controller Reference")]
        [SerializeField] private Managers.QuestManager _questManager;

        [Title("Quest List Components")]
        [SerializeField] private GameObject _questListContent;
        [SerializeField] private GameObject _questEntryPrefab;
        [SerializeField] private GameObject _questDetailsPanel;
        [SerializeField] private TMP_Text _noActiveQuestsLabel;

        [Title("Quest Details Components")]
        [SerializeField] private TMP_Text _questlineTitleText;
        [SerializeField] private TMP_Text _questTitleText, _descriptionText;
        [SerializeField] private QuestObjectivePanel _objectivePanel;
        [SerializeField] private QuestRewardPanel _rewardPanel;

        private List<QuestLogEntry> _questList = new();
        private QuestData _selectedQuest = null;

        private void OnEnable()
        {
            _questDetailsPanel.SetActive(false);
            UpdateQuestLog(_questManager.GetActiveQuests());
        }
        private void OnDisable()
        {
            
        }

        // update the quests displayed in the quest log
        private void UpdateQuestLog(List<QuestData> data)
        {
            ResetPanel();

            // if there are no active quests
            if (data.IsNullOrEmpty())
            {
                _noActiveQuestsLabel.gameObject.SetActive(true);
            }
            else
            {
                _noActiveQuestsLabel.gameObject.SetActive(false);

                // create list of quests to be shown
                foreach (QuestData quest in data)
                {
                    QuestLogEntry entry = Instantiate(_questEntryPrefab, _questListContent.transform).GetComponent<QuestLogEntry>();
                    // show quest name
                    entry.Init(quest.QuestName);
                    // call entry.SetSelected() based on if entry was already selected
                    entry.Button.onClick.AddListener(() => entry.SetSelected(SelectQuest(quest)));
                    _questList.Add(entry);
                }
            }
        }

        private bool SelectQuest(QuestData quest)
        {
            // deselect all quests (deselects the previous selected quest)
            foreach (QuestLogEntry entry in _questList)
            {
                entry.SetSelected(false);
            }

            // if quest was already selected, deselect it
            if (_selectedQuest == quest)
            {
                _selectedQuest = null;
                _questDetailsPanel.SetActive(false);
                return false;
            }
            // if otherwise, select this quest
            else
            {
                _selectedQuest = quest;
                ShowDetails(quest);
                return true;
            }
        }

        private void ShowDetails(QuestData questData)
        {
            // show details panel
            _questDetailsPanel.SetActive(true);

            // set quest details
            _questlineTitleText.text = questData.QuestlineName;
            _questTitleText.text = questData.QuestName;
            _descriptionText.text = questData.Description;

            // set objectives and rewards
            _objectivePanel.SetObjectives(questData.MissionString);
            _rewardPanel.SetRewards(questData.Rewards);
        }

        private void ResetPanel()
        {
            // delete all entries in quest list panel
            for (int index = _questList.Count - 1; index >= 0; index--)
            {
                _questList[index].DestroySelf();
            }
            _questList.Clear();

            // hide details panel
            _questDetailsPanel.SetActive(false);
        }
    }
}