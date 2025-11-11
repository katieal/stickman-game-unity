using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using EventChannel;
using Characters.Quests;
using System.Collections.Generic;

/*
 * Rename to : QuestPopup? or QuestPopupWindow?
 * 
 */

namespace UserInterface
{
    public class QuestPromptWindow : MonoBehaviour
    {
        [Title("Components")]
        [SerializeField] private GameObject _questPanel;
        [SerializeField] private TMP_Text _questTitleText, _descriptionText;
        [SerializeField] private QuestObjectivePanel _objectivePanel;
        [SerializeField] private QuestRewardPanel _rewardPanel;

        // could also just keep two buttons and change the text accordingly? which would be better?????
        [SerializeField] private Button _acceptButton, _declineButton, _submitButton, _cancelButton;

        [Title("Responding to Events")]
        [SerializeField] private QuestBoolUniTaskEventSO _questPromptEvent;
        [SerializeField] private QuestBoolUniTaskEventSO _questCheckInEvent;

        private void OnEnable()
        {
            _questPanel.SetActive(false);
            _questPromptEvent.OnStartEvent += ShowQuestPrompt;
            _questCheckInEvent.OnStartEvent += ShowQuestCheckIn;
        }
        private void OnDisable()
        {
            _questPromptEvent.OnStartEvent -= ShowQuestPrompt;
            _questCheckInEvent.OnStartEvent -= ShowQuestCheckIn;
        }

        private void ShowQuestPrompt(QuestData questData)
        {
            // show quest panel
            _questPanel.SetActive(true);

            // set buttons
            _acceptButton.gameObject.SetActive(true);
            _declineButton.gameObject.SetActive(true);
            _submitButton.gameObject.SetActive(false);
            _cancelButton.gameObject.SetActive(false);

            // bind decision button callbacks
            _acceptButton.onClick.AddListener(() => OnQuestPromptResponse(true));
            _declineButton.onClick.AddListener(() => OnQuestPromptResponse(false));

            // set quest details
            //_questlineTitleText.text = questData.QuestlineName;
            _questTitleText.text = questData.QuestName;
            _descriptionText.text = questData.Description;

            // set objectives and rewards
            _objectivePanel.SetObjectives(questData.MissionString);
            _rewardPanel.SetRewards(questData.Rewards);
        }

        // called when a response button to quest prompt is clicked
        private void OnQuestPromptResponse(bool isAccepted)
        {
            // unbind buttons
            _acceptButton.onClick.RemoveAllListeners();
            _declineButton.onClick.RemoveAllListeners();

            // close quest panel
            _questPanel.SetActive(false);

            // broadcast response
            _questPromptEvent.EndEvent(isAccepted);
        }

        /// <summary>
        /// Show Quest data with objective progress and option to submit or cancel
        /// </summary>
        /// <param name="questData">Quest to display</param>
        private void ShowQuestCheckIn(QuestData questData)
        {
            // show quest panel
            _questPanel.SetActive(true);

            // set buttons
            _acceptButton.gameObject.SetActive(false);
            _declineButton.gameObject.SetActive(false);
            _submitButton.gameObject.SetActive(true);
            _cancelButton.gameObject.SetActive(true);

            // enable submit button if quest has been completed
            if (questData.IsComplete)
            {
                _submitButton.interactable = true;
                _submitButton.onClick.AddListener(() => OnCheckInResponse(true));
            }
            else
            {
                _submitButton.interactable = false;
            }

            _cancelButton.onClick.AddListener(() => OnCheckInResponse(false));

            // set quest details
            //_questlineTitleText.text = questData.QuestlineName;
            _questTitleText.text = questData.QuestName;
            _descriptionText.text = questData.Description;

            // set objectives and rewards
            _objectivePanel.SetObjectives(questData.MissionString);
            _rewardPanel.SetRewards(questData.Rewards);
        }

        // called when quest is submitted or cancel button is clicked
        private void OnCheckInResponse(bool isSubmitted)
        {
            // unbind buttons
            _submitButton.onClick.RemoveAllListeners();
            _cancelButton.onClick.RemoveAllListeners();

            // close quest panel
            _questPanel.SetActive(false);

            // broadcast response
            _questCheckInEvent.EndEvent(isSubmitted);
        }
    }
}