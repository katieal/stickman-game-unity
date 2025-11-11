using Characters;
using Characters.Quests;
using Cysharp.Threading.Tasks;
using EventChannel;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace Managers
{
    public struct QuestInfo
    {
        public NpcName NpcName;
        public int QuestlineIndex;
        public Quest Quest;
    }

    public class QuestManager : SerializedMonoBehaviour
    {
        [Title("Components")]
        public Dictionary<NpcName, List<QuestlineSO>> QuestDatabase = new Dictionary<NpcName, List<QuestlineSO>>();

        [Title("Invoked Events")]
        [SerializeField] private ItemEventSO _addItemEvent;
        [SerializeField] private StringIntEventSO _removeItemByIdEvent;
        [SerializeField] private IntIntRequestEventSO _changeMoneyEvent;

        [Title("UniTask Events")]
        [SerializeField] private VoidStringListUniTaskEventSO _saveConditionKeysEvent;
        [SerializeField] private QuestBoolUniTaskEventSO _questPromptEvent;
        [SerializeField] private QuestBoolUniTaskEventSO _questCheckInEvent;

        // trackable game events
        [Title("Listening to Events")]
        [SerializeField] private GameStateDataEventSO _loadGameDataEvent;
        [SerializeField] private StringIntEventSO _itemCountChangedEvent;
        [SerializeField] private MobTypeEventSO _mobKilledEvent; // add invoke to mob sm's death state
        [SerializeField] private GuidIntEventSO _timeSpentEvent; // guid = quest guid
        [SerializeField] private StringEventSO _missionKeyEvent;
        [SerializeField] private StringEventSO _conditionKeyEvent;

        // list of guids for active quests?
        private List<QuestInfo> _activeQuests = new List<QuestInfo>();

        // list of triggered quest condition keys 
        private List<string> _conditionKeys = new();

        private void OnEnable()
        {
            LoadActiveQuests();
            _loadGameDataEvent.OnInvokeEvent += OnLoadGameData;
            _saveConditionKeysEvent.OnStartEvent += OnSaveConditionKeys;
            _conditionKeyEvent.OnInvokeEvent += OnConditionKeyEvent;
        }
        private void OnDisable()
        {
            _loadGameDataEvent.OnInvokeEvent -= OnLoadGameData;
            _saveConditionKeysEvent.OnStartEvent -= OnSaveConditionKeys;
            _conditionKeyEvent.OnInvokeEvent -= OnConditionKeyEvent;
        }

        #region Load Data
        private void OnLoadGameData(SaveData.GameStateData data)
        {
            _conditionKeys = data.TriggeredConditionKeys;
        }
        private void LoadActiveQuests()
        {
            // TODO: IMPLEMENT!!
        }
        #endregion

        private void OnSaveConditionKeys()
        {
            _saveConditionKeysEvent.EndEvent(new List<string>(_conditionKeys));
        }

        #region Debug
        private void CheckName(NpcName name)
        {
            Debug.Assert(QuestDatabase.ContainsKey(name), $"{name} not found in dictionary");
        }
        #endregion

        #region Utility Methods
        private int FindQuestlineIndex(NpcName name, Guid questlineId)
        {
            List<QuestlineSO> questlines = QuestDatabase[name];
            for (int index = 0; index < questlines.Count; index++)
            {
                if (questlines[index].Guid.Equals(questlineId)) { return index; }
            }
            return -1;
        }
        #endregion

        #region Public Methods
        public List<QuestData> GetActiveQuests()
        {
            List<QuestData> dataList = new List<QuestData>();
            foreach (QuestInfo info in _activeQuests)
            {
                QuestData data = info.Quest.GetDataWithProgress();
                dataList.Add(data);
            }

            return dataList;
        }

        public bool CheckActiveQuests(Guid questGuid)
        {
            foreach (QuestInfo info in _activeQuests)
            {
                if (info.Quest.Guid.Equals(questGuid)) { return true; }
            }
            return false;
        }

        public QuestDialogue CheckForActiveQuest(NpcName name, out QuestDialogue dialogue)
        {
            CheckName(name);

            // if npc has an active quest, return it
            foreach (QuestInfo info in _activeQuests)
            {
                if (info.NpcName == name) 
                {
                    dialogue = info.Quest.GetDialogue();
                    dialogue.Questline = QuestDatabase[info.NpcName][info.QuestlineIndex].Guid;
                    return dialogue; 
                }
            }
            // if no active quest, return empty
            dialogue = null;
            return dialogue;
        }

        public QuestDialogue CheckForAssignableQuest(NpcName name, out QuestDialogue dialogue)
        {
            CheckName(name);

            List<QuestlineSO> questlines = QuestDatabase[name];

            // go through each of npcs questlines
            for (int index = 0; index < questlines.Count; index++)
            {
                // move to next questline if it has already been completed
                if (questlines[index].Status == QuestStatus.Complete) { continue; }

                // check the status of the current quest in each questline
                if (questlines[index].CurrentQuest.Status == QuestStatus.Inactive)
                {
                    // check if quest can be triggered
                    if (CheckQuestConditions(questlines[index].CurrentQuest.Conditions))
                    {
                        dialogue = questlines[index].CurrentQuest.GetDialogue();
                        dialogue.Questline = questlines[index].Guid;
                        return dialogue;
                    }
                }
                else if (questlines[index].CurrentQuest.Status == QuestStatus.Denied) 
                {
                    // if quest was denied, send prompt again without verifying conditions
                    dialogue = questlines[index].CurrentQuest.GetDialogue();
                    dialogue.Questline = questlines[index].Guid;
                    return dialogue;
                }
            }

            // if no quests can be prompted, return null
            dialogue = null;
            return dialogue; 
        }

        public async UniTask<bool> SendQuestPrompt(NpcName name, Guid questlineId)
        {
            // get relevant questline
            int index = FindQuestlineIndex(name, questlineId);
            QuestlineSO questline = QuestDatabase[name][index];

            // show quest prompt window and wait for user response
            _questPromptEvent.StartEvent(questline.CurrentQuest.GetData());
            bool response = await _questPromptEvent.Task;

            if (response)
            {
                StartQuest(name, index);

                return true;
            }
            else
            {
                questline.CurrentQuest.Status = QuestStatus.Denied;

                return false;
            }
        }

        public async UniTask<bool> SendQuestCheckIn(NpcName name, Guid questlineId)
        {
            int index = FindQuestlineIndex(name, questlineId);
            QuestlineSO questline = QuestDatabase[name][index];

            // show quest checkin window and wait for user response
            _questCheckInEvent.StartEvent(questline.CurrentQuest.GetDataWithProgress());
            bool response = await _questCheckInEvent.Task;
            return response;
        }

        public void CompleteQuest(NpcName name, Guid questlineId)
        {
            // temp var for readability
            int index = FindQuestlineIndex(name, questlineId);
            QuestlineSO questline = QuestDatabase[name][index];
            Quest quest = QuestDatabase[name][index].CurrentQuest;

            // remove quest from active quests
            _activeQuests.Remove(_activeQuests.Find(x => x.Quest.Equals(quest)));

            // update statuses
            quest.Status = QuestStatus.Complete;
            questline.CurrentIndex++;
            // set questline to complete if all quests have been completed
            if (questline.CurrentIndex >= questline.Quests.Count)
            {
                questline.Status = QuestStatus.Complete;
            }

            // unsubscribe all mission listeners
            foreach (IQuestMission mission in quest.Missions)
            {
                if (mission is CollectMission collectMission)
                {
                    _itemCountChangedEvent.OnInvokeEvent -= collectMission.OnItemCollected;
                    // remove quest items from inventory
                    _removeItemByIdEvent.InvokeEvent(collectMission.Item.ItemId, collectMission.RequiredNum);
                }
                else if (mission is BountyMission bountyMission)
                {
                    _mobKilledEvent.OnInvokeEvent -= bountyMission.OnMobKilled;
                }
                else if (mission is TimeMission timeMission)
                {
                    _timeSpentEvent.OnInvokeEvent -= timeMission.OnTimeSpent;
                }
                else if (mission is KeyMission keyMission)
                {
                    _missionKeyEvent.OnInvokeEvent -= keyMission.OnKeyInvoked;
                }
            }

            // give quest reward
            foreach (QuestReward reward in quest.Rewards)
            {
                if (reward.Type == RewardType.Item)
                {
                    _addItemEvent.InvokeEvent(new Items.ItemInstance(reward.Item.ItemSO, reward.Item.Quantity));
                }
                else if (reward.Type == RewardType.Money)
                {
                    _changeMoneyEvent.RequestEvent(reward.Money);
                }
                else if (reward.Type == RewardType.Experience)
                {
                    Debug.Log($"gained {reward.ExperienceAmount} experience!!");
                }
                else
                {
                    // skill reward
                    // TODO: IMPLEMENT
                    Debug.Log("Gained skill!!!");
                }
            }
        }
        #endregion
        
        private void OnConditionKeyEvent(string key)
        {
            _conditionKeys.Add(key);
        }

        private bool CheckQuestConditions(List<IQuestCondition> conditions)
        {
            foreach (IQuestCondition condition in conditions)
            {
                if (condition is KeyCondition key)
                {
                    // if key has been triggered, condition is complete
                    if (_conditionKeys.Contains(key.Key))
                    {
                        key.IsComplete = true;
                        continue;
                    }
                    else { return false; }
                }
                else if (!condition.IsComplete) { return false; }
            }

            return true;
        }

        private void StartQuest(NpcName name, int questlineIndex)
        {
            // temp var for readability
            Quest quest = QuestDatabase[name][questlineIndex].CurrentQuest;

            // set quest and questline to active
            QuestDatabase[name][questlineIndex].Status = QuestStatus.Active;
            quest.Status = QuestStatus.Active;

            QuestInfo info = new QuestInfo()
            {
                NpcName = name,
                QuestlineIndex = questlineIndex,
                Quest = quest,
            };

            // subscribe mission listeners
            foreach (IQuestMission mission in quest.Missions)
            {
                if (mission is CollectMission collectMission)
                {
                    _itemCountChangedEvent.OnInvokeEvent += collectMission.OnItemCollected;
                }
                else if (mission is BountyMission bountyMission)
                {
                    _mobKilledEvent.OnInvokeEvent += bountyMission.OnMobKilled;
                }
                else if (mission is TimeMission timeMission)
                {
                    _timeSpentEvent.OnInvokeEvent += timeMission.OnTimeSpent;
                }
                else if (mission is KeyMission keyMission)
                {
                    _missionKeyEvent.OnInvokeEvent += keyMission.OnKeyInvoked;
                }
            }

            _activeQuests.Add(info);
        }
    }
}