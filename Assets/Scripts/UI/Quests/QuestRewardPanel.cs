using Characters.Quests;
using Items;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

namespace UserInterface
{
    [Serializable]
    public struct RewardStruct
    {
        [ChildGameObjectsOnly] public GameObject Panel;
        [ChildGameObjectsOnly] public TMP_Text NameText;
        [ChildGameObjectsOnly] public Image Image;
        [ChildGameObjectsOnly] public TMP_Text QuantityText;
    }

    public class QuestRewardPanel : SerializedMonoBehaviour
    {
        [Title("Components")]
        [SerializeField] private TMP_Text _rewardLabel;
        [SerializeField][RequiredListLength(4)] private List<RewardStruct> _rewardEntries = new();

        public void SetRewards(List<QuestReward> rewards)
        {
            // make label singular or plural
            if (rewards.Count > 1) { _rewardLabel.text = "Rewards"; }
            else { _rewardLabel.text = "Reward"; }

            // populate or disable each reward entry panel 
            for (int index = 0; index < _rewardEntries.Count; index++)
            {
                if (index < rewards.Count)
                {
                    // enable reward entry panel
                    _rewardEntries[index].Panel.SetActive(true);

                    // set quest reward info
                    if (rewards[index].Type == RewardType.Item)
                    {
                        SetItem(_rewardEntries[index], rewards[index].Item);
                    }
                    else if (rewards[index].Type == RewardType.Money)
                    {
                        SetMoney(_rewardEntries[index], rewards[index].Money);
                    }
                    else if (rewards[index].Type == RewardType.Experience)
                    {
                        SetExperience(_rewardEntries[index], rewards[index].ExperienceAmount);
                    }
                    else
                    {
                        SetSkill(_rewardEntries[index], rewards[index].Skill);
                    }
                }
                else
                {
                    // if reward count is less than 4, disable extra reward entries
                    _rewardEntries[index].Panel.SetActive(false);
                }
            }
        }

        private void SetItem(RewardStruct reward, Items.ItemEntry item)
        {
            reward.Image.sprite = GameData.ItemDatabase.Instance.GetIcon(item.ItemSO.ItemId);
            reward.NameText.text = item.ItemSO.DisplayName;
            if (item.Quantity > 1)
            {
                reward.QuantityText.enabled = true;
                reward.QuantityText.text = item.Quantity.ToString();
            }
            else { reward.QuantityText.enabled = false; }
        }

        private void SetMoney(RewardStruct reward, int amount)
        {
            reward.Image.sprite = GameData.SpriteDatabase.Instance.Sprites.GetSprite("Icons", "Coin");
            reward.NameText.text = "Coins";
            reward.QuantityText.enabled = true;
            reward.QuantityText.text = amount.ToString();
        }

        private void SetExperience(RewardStruct reward, int amount)
        {
            reward.Image.sprite = GameData.SpriteDatabase.Instance.Sprites.GetSprite("Icons", "Experience");
            reward.NameText.text = "Experience";
            reward.QuantityText.enabled = true;
            reward.QuantityText.text = amount.ToString();
        }

        private void SetSkill(RewardStruct reward, string skill)
        {
            reward.Image.sprite = null;
            reward.NameText.text = skill;
            reward.QuantityText.enabled = false;
        }
    }
}