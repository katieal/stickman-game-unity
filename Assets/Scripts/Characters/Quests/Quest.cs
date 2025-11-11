using Managers;
using PixelCrushers.DialogueSystem;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization;
using Sirenix.Utilities.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Characters.Quests
{
    [Serializable]
    public class Quest 
    {
        [HideInInspector] public string QuestlineName;

        [TabGroup("Info")]
        [InlineButton(nameof(CopyGuid))] public string Name;
        [TabGroup("Info")]
        [ReadOnly] public Guid Guid;
        [TabGroup("Info")]
        [ReadOnly] public int QuestOrder;
        [TabGroup("Info")]
        [Tooltip("Descrption to be displayed in quest window.")]
        [TextArea(2, 4)] public string Description;
        [TabGroup("Info")]
        [HideLabel][RequiredListLength(MinLength = 1)] public List<IQuestCondition> Conditions;
        [TabGroup("Info")]
        [EnumToggleButtons, HideLabel] public QuestStatus Status;

        [TabGroup("Objective")]
        public List<IQuestMission> Missions = new();

        [TabGroup("Reward")]
        public List<QuestReward> Rewards = new();

        [TabGroup("Dialogue")]
        [ConversationPopup] public string Conversation;
        [TabGroup("Dialogue")]
        [ReadOnly] public int OnPrompt;
        [TabGroup("Dialogue")]
        [ReadOnly] public int OnAccept;
        [TabGroup("Dialogue")]
        [ReadOnly] public int OnReject;
        [TabGroup("Dialogue")]
        [ReadOnly] public int OnCheckin;
        [TabGroup("Dialogue")]
        [ReadOnly] public int OnComplete;
        

        #region Inspector
        public void SetOrder(int order)
        {
            QuestOrder = order;
        }

        private void CopyGuid() { GUIUtility.systemCopyBuffer = this.Guid.ToString(); }

        [TabGroup("Objective")]
        [Button]
        public void SetMissionIds()
        {
            for (int index = 0; index < Missions.Count; index++)
            {
                Missions[index].SetIds(this.Guid, index);
            }
        }
        #endregion

        #region Accessor Methods
        // not sure if using a data struct is the best way since I also need to pass the questline name
        public QuestData GetData()
        {
            QuestData data = new QuestData()
            {
                QuestlineName = this.QuestlineName,
                QuestName = Name,
                Description = this.Description,
                MissionString = new List<string>(),
                Rewards = this.Rewards
            };

            // get mission strings
            foreach (IQuestMission mission in Missions)
            {
                data.MissionString.Add(mission.GetMissionString());
            }

            return data;
        }

        public QuestData GetDataWithProgress()
        {
            QuestData data = new QuestData()
            {
                QuestlineName = this.QuestlineName,
                QuestName = Name,
                Description = this.Description,
                MissionString = new List<string>(),
                Rewards = this.Rewards,
                IsComplete = true
            };

            // get mission strings and determine if quest can be submitted
            foreach (IQuestMission mission in Missions)
            {
                data.MissionString.Add(mission.GetMissionString() + "\t" + mission.GetProgressString());

                // sets IsComplete to false if any of the missions are incomplete
                if (!mission.IsComplete) { data.IsComplete = false; }
            }

            return data;
        }

        public QuestDialogue GetDialogue()
        {
            QuestDialogue dialogue = new QuestDialogue()
            {
                Quest = Guid,
                Conversation = this.Conversation,
                OnPrompt = this.OnPrompt,
                OnAccept = this.OnAccept,
                OnReject = this.OnReject,
                OnCheckIn = this.OnCheckin,
                OnComplete = this.OnComplete,
            };
            return dialogue;
        }
    }
    #endregion


    #region Data Classes
    public enum QuestStatus { Inactive, Active, Complete, Denied }

    [Serializable]
    public class QuestData
    {
        public string QuestlineName;
        public string QuestName;
        public string Description;
        public List<string> MissionString;
        public List<QuestReward> Rewards;

        /// <summary>
        /// True if all the quest's missions have been completed, false otherwise
        /// </summary>
        public bool IsComplete;
    }

    public class QuestDialogue
    {
        public Guid Questline;
        public Guid Quest;
        public string Conversation;
        public int OnPrompt;
        public int OnAccept;
        public int OnReject;
        public int OnCheckIn;
        public int OnComplete;
    }
    #endregion


    #region Conditions

    public enum ConditionType { Day, Time, Key, Sequential }

    public interface IQuestCondition
    {
        public ConditionType Type { get; }
        public bool IsComplete { get; }
    }

    [Serializable]
    public class DayCondition : IQuestCondition
    {
        public ConditionType Type { get; } = ConditionType.Day;
        public bool IsComplete 
        { 
            get {
                // if EndDay is -1, there is no end date
                if (EndDay != -1) { return DayManager.Instance.CurrentDay >= StartDay && DayManager.Instance.CurrentDay <= EndDay; }
                else { return DayManager.Instance.CurrentDay >= StartDay; }  
            } 
        }

        [Tooltip("The minimum value (inclusive) of CurrentDay in order for condition to be true.")]
        public int StartDay;

        [Tooltip("The maximum value (inclusive) of CurrentDay in order for condition to be true. Enter -1 for no maximum day.")]
        public int EndDay;
    }

    [Serializable]
    public class TimeCondition : IQuestCondition
    {
        public ConditionType Type { get; } = ConditionType.Time;
        public bool IsComplete 
        {
            get {
                return TimeManager.Instance.CurrentTime >= StartTime && TimeManager.Instance.CurrentTime <= EndTime;
            } 
        }

        [TimeSelector]
        public int StartTime;
        [TimeSelector]
        public int EndTime;
    }

    [Serializable]
    public class KeyCondition : IQuestCondition
    {
        public ConditionType Type { get; } = ConditionType.Key;

        // value is evaluated in QuestManager
        public bool IsComplete { get; set; }

        public string Key;
    }

    [Serializable]
    public class SequentialCondition : IQuestCondition
    {
        public ConditionType Type { get; } = ConditionType.Sequential;
        public bool IsComplete { get; set; }
    }

    #endregion


    #region Missions

    /// <summary>
    /// Collect: Deliver X amount of X items
    /// Bounty: Slay X amount of X mob
    /// Time: Invest X hours 
    /// Key: pick a certain option when talking to someone/interacting with an object
    /// 
    /// </summary>
    public enum ObjectiveType { Collect, Bounty, Time, Key } // Add money mission?

    public interface IQuestMission
    {
        public ObjectiveType Type { get; }
        public Guid QuestGuid { get; set; }
        public int MissionId { get; set; }
        public bool IsComplete { get; set; }

        public void SetIds(Guid questGuid, int missionId)
        {
            QuestGuid = questGuid;
            MissionId = missionId;
        }
        public string GetMissionString();
        public string GetProgressString();


        // editor only
        public void Reset();
    }

    // address SO/mutable data thing AFTER you get it working

    [Serializable]
    public class CollectMission : IQuestMission
    {
        public ObjectiveType Type { get; } = ObjectiveType.Collect;
        public Guid QuestGuid { get; set; }
        public int MissionId { get; set; }
        public bool IsComplete { get; set; }

        public Items.ItemSO Item;
        public int RequiredNum;
        public int CurrentNum;

        public bool HasCustomMissionString;
        [ShowIf("HasCustomMissionString")] public string CustomMissionString;

        public string GetMissionString()
        {
            if (HasCustomMissionString) { return CustomMissionString; }

            string result = "Collect " + RequiredNum.ToString() + " " + Item.DisplayName;
            if (RequiredNum > 1) { result += "s"; }
            return result;
        }

        public string GetProgressString()
        {
            return "(" + CurrentNum.ToString() + "/" + RequiredNum.ToString() + ")";
        }

        public void OnItemCollected(string itemId, int num)
        {
            if (Item.ItemId.Equals(itemId))
            {
                CurrentNum += num;
                if (CurrentNum >= RequiredNum) { IsComplete = true; }
                else {  IsComplete = false; }
            }
        }


        // editor only
        public void Reset() { CurrentNum = 0; IsComplete = false; }
    }

    [Serializable]
    public class BountyMission : IQuestMission
    {
        public ObjectiveType Type { get; } = ObjectiveType.Bounty;
        public Guid QuestGuid { get; set; }
        public int MissionId { get; set; }
        public bool IsComplete { get; set; }

        public Characters.MobType MobType;
        public int RequiredKills;
        public int CurrentKills;

        public bool HasCustomMissionString;
        [ShowIf("HasCustomMissionString")] public string CustomMissionString;

        public string GetMissionString()
        {
            if (HasCustomMissionString) {  return CustomMissionString; }

            string result = result = "Slay " + RequiredKills.ToString() + " " + MobType.ToString();
            if (RequiredKills > 1) { result += "s"; }
            return result;
        }

        public string GetProgressString()
        {
            return "(" + CurrentKills.ToString() + "/" + RequiredKills.ToString() + ")";
        }

        public void OnMobKilled(Characters.MobType type)
        {
            if (MobType.Equals(type))
            {
                CurrentKills++;
                if (CurrentKills >= RequiredKills) { IsComplete = true; }
            }
        }

        // editor only
        public void Reset() { CurrentKills = 0; IsComplete = false; }
    }

    [Serializable]
    public class TimeMission : IQuestMission 
    {
        public ObjectiveType Type { get; } = ObjectiveType.Time;
        public Guid QuestGuid { get; set; }
        public int MissionId { get; set; }
        public bool IsComplete { get; set; }

        public int RequiredMinutes;
        public int CurrentMinutes;

        public bool HasCustomMissionString;
        [ShowIf("HasCustomMissionString")] public string CustomMissionString;

        public string GetMissionString()
        {
            if (HasCustomMissionString) { return CustomMissionString; }

            int hours = (int)(RequiredMinutes / 60);
            string result = hours.ToString() + " hour";
            // make hour plural if needed
            if (hours > 1) { result += "s "; }
            else { result += " "; }

            float minutes = RequiredMinutes % 60;
            if (minutes != 0)
            {
                result += (" and " + minutes.ToString() + " minutes");
            }
            return result;
        }

        public string GetProgressString()
        {
            return "(" + CurrentMinutes.ToString() + "/" + RequiredMinutes.ToString() + ")";
        }

        public void OnTimeSpent(Guid questId, int time)
        {
            if (QuestGuid.Equals(questId))
            {
                CurrentMinutes += time;
                if (CurrentMinutes >= RequiredMinutes) { IsComplete = true; }
            }
        }


        // editor only
        public void Reset() { CurrentMinutes = 0; IsComplete = false; }
    }

    [Serializable]
    public class KeyMission : IQuestMission
    {
        public ObjectiveType Type { get; } = ObjectiveType.Key;
        public Guid QuestGuid { get; set; }
        public int MissionId { get; set; }
        public bool IsComplete { get; set; }

        public string Key;
        public string MissionString;

        public string GetMissionString()
        {
            return MissionString;
        }

        public string GetProgressString()
        {
            return (IsComplete) ? "(1/1)" : "(0/1)";
        }

        public void OnKeyInvoked(string key)
        {
            if (Key.Equals(key))
            {
                IsComplete = true;
            }
        }

        // editor only
        public void Reset() { IsComplete = false; }
    }
    #endregion


    #region Rewards
    public enum RewardType { Item, Money, Experience, Skill }

    [Serializable]
    public struct QuestReward
    {
        [EnumToggleButtons, HideLabel]
        public RewardType Type;

        [ShowIf("Type", RewardType.Item)]
        [HideLabel] public Items.ItemEntry Item;

        [ShowIf("Type", RewardType.Money)]
        public int Money;

        [ShowIf("Type", RewardType.Experience)]
        public int ExperienceAmount;

        [ShowIf("Type", RewardType.Skill)]
        public string Skill;
    }

    #endregion



}