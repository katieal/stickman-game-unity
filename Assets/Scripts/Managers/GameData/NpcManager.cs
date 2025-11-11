using Sirenix.OdinInspector;
using UnityEngine;
using PixelCrushers.DialogueSystem;
using Characters.Quests;
using EventChannel;
using Characters;
using Cysharp.Threading.Tasks;
using System;

namespace Managers
{
    public class NpcManager : MonoBehaviour
    {
        [Title("References")]
        [SerializeField] private QuestManager _questManager;

        [Title("Invoked Events")]
        [SerializeField] private GameStateRequestEventSO _gameStateRequestEvent;
        [SerializeField] private BoolEventSO _greyBackgroundEvent;
        [SerializeField] private StringBoolUniTaskEventSO _trainEvent;

        [Title("Listening to Events")]
        [SerializeField] private NpcNameEventSO _npcEvent;

        /*
         * When npc is interacted with, send event to this class with npc name
         * this class will then check with the quest manager,
         * then trigger appropriate dialogue in lua dialogue database
         * 
         */


        /*
         * OnEventCall:
         *  - if npc has active quest, trigger that
         *  - if npc has assignable quest, trigger
         * 
         */

        private void OnEnable()
        {
            _npcEvent.OnInvokeEvent += EvaluateDialogue;
        }
        private void OnDisable()
        {
            _npcEvent.OnInvokeEvent -= EvaluateDialogue;
        }

        private void PauseGame(bool isPaused)
        {
            if (isPaused)
            {
                _gameStateRequestEvent.RequestEvent(GameStateMachine.StateType.Pause);
                _greyBackgroundEvent.InvokeEvent(true);
            }
            else
            {
                _gameStateRequestEvent.RequestEvent(GameStateMachine.StateType.Run);
                _greyBackgroundEvent.InvokeEvent(false);
            }
        }

        private void EvaluateDialogue(NpcName name)
        {
            if (name == NpcName.Eomri) 
            { 
                StartDefaultResponseMenu(name).Forget(); 
            }
            else if (_questManager.CheckForActiveQuest(name, out QuestDialogue activeDialogue) != null)
            {
                StartQuestCheckInAsync(name, activeDialogue).Forget();
            }
            else if (_questManager.CheckForAssignableQuest(name, out QuestDialogue newDialogue) != null)
            {
                StartQuestPromptAsync(name, newDialogue).Forget();
            }
            else
            {
                StartDefaultConversationAsync(name).Forget();
            }
        }

        private async UniTaskVoid StartDefaultResponseMenu(NpcName name)
        {
            PauseGame(true);

            string response = await SendConversationResponseAsync(name.ToString() + "/Default");

            if (response == "Train")
            {
                // open training window, await for it to close
                _trainEvent.StartEvent(name.ToString());
                await _trainEvent.Task;
            }
            else
            {
                await WaitForConversationAsync();
            }

            PauseGame(false);
        }


        private async UniTaskVoid StartQuestCheckInAsync(NpcName name, QuestDialogue dialogue)
        {
            PauseGame(true);

            // send checkin dialogue and wait for it to finish
            await SendDialogueAsync(dialogue.Conversation, dialogue.OnCheckIn);

            // display checkin window
            bool result = await _questManager.SendQuestCheckIn(name, dialogue.Questline);

            // complete quest if it was submitted
            if (result)
            {
                await SendDialogueAsync(dialogue.Conversation, dialogue.OnComplete);
                _questManager.CompleteQuest(name, dialogue.Questline);

                // check if next quest can be prompted
                if (_questManager.CheckForAssignableQuest(name, out QuestDialogue newDialogue) != null)
                {
                    StartQuestPromptAsync(name, newDialogue).Forget();
                }
                else
                {
                    // if next quest can't be prompted, unpause game
                    PauseGame(false);
                }
            }
            else
            {
                // unpause game if user selects cancel
                PauseGame(false);
            }
        }


        private async UniTaskVoid StartQuestPromptAsync(NpcName name, QuestDialogue dialogue)
        {
            PauseGame(true);
            
            // send quest prompt dialogue and wait for it to finish
            await SendDialogueAsync(dialogue.Conversation, dialogue.OnPrompt);

            // display quest prompt window
            bool result = await _questManager.SendQuestPrompt(name, dialogue.Questline);

            // send dialogue based on if user accepts or rejects quest
            if (result)
            {
                await SendDialogueAsync(dialogue.Conversation, dialogue.OnAccept);
            }
            else
            {
                await SendDialogueAsync(dialogue.Conversation, dialogue.OnReject);
            }

            PauseGame(false);
        }

        private async UniTaskVoid StartDefaultConversationAsync(NpcName name)
        {
            PauseGame(true);

            await SendDialogueAsync(name.ToString() + "/Default");

            PauseGame(false);
        }


        /// <summary>
        /// UniTask wrapper method for DialogueManager OnConversationEnd callback
        /// </summary>
        /// <param name="conversation">Name of conversation to start</param>
        /// <returns>UniTask</returns>
        private async UniTask SendDialogueAsync(string conversation)
        {
            UniTaskCompletionSource<bool> tcs = new UniTaskCompletionSource<bool>();

            // task finishes when conversation ends
            void OnPromptEnd(Transform actor)
            {
                tcs.TrySetResult(true);
                DialogueManager.instance.conversationEnded -= OnPromptEnd;
            }

            DialogueManager.instance.conversationEnded += OnPromptEnd;

            // start conversation
            DialogueManager.StartConversation(conversation);
            await tcs.Task;
        }

        /// <summary>
        /// UniTask wrapper method for DialogueManager OnConversationEnd callback
        /// </summary>
        /// <param name="conversation">Name of conversation</param>
        /// <param name="entryIndex">Index of dialogue entry to start conversation at</param>
        /// <returns>UniTask</returns>
        private async UniTask SendDialogueAsync(string conversation, int entryIndex)
        {
            UniTaskCompletionSource<bool> tcs = new UniTaskCompletionSource<bool>();

            // task finishes when conversation ends
            void OnPromptEnd(Transform actor)
            {
                tcs.TrySetResult(true);
                DialogueManager.instance.conversationEnded -= OnPromptEnd;
            }

            DialogueManager.instance.conversationEnded += OnPromptEnd;

            // start conversation
            DialogueManager.StartConversation(conversation, null, null, entryIndex);
            await tcs.Task;
        }

        private async UniTask<string> SendConversationResponseAsync(string conversation)
        {
            // do I need to wait for stuff to end????
            //UniTaskCompletionSource<bool> onEndTcs = new UniTaskCompletionSource<bool>();
            UniTaskCompletionSource<string> responseTcs = new UniTaskCompletionSource<string>();
            
            void OnResponseSelected (object sender, SelectedResponseEventArgs e)
            {
                // unsubscribe
                DialogueManager.instance.conversationView.SelectedResponseHandler -= OnResponseSelected;
                // send result
                responseTcs.TrySetResult(e.response.formattedText.text);
            }


            //DialogueManager.instance.conversationView.FinishedSubtitleHandler 

            // start conversation
            DialogueManager.StartConversation(conversation);

            // subscribe to response selected event
            DialogueManager.instance.conversationView.SelectedResponseHandler += OnResponseSelected;

            return await responseTcs.Task;
        }

        private async UniTask WaitForConversationAsync()
        {
            UniTaskCompletionSource<bool> tcs = new UniTaskCompletionSource<bool>();

            // task finishes when conversation ends
            void OnPromptEnd(Transform actor)
            {
                tcs.TrySetResult(true);
                DialogueManager.instance.conversationEnded -= OnPromptEnd;
            }

            DialogueManager.instance.conversationEnded += OnPromptEnd;

            // start conversation
            await tcs.Task;
        }

        private async UniTask OldHolder(string conversation)
        {
            UniTaskCompletionSource<string> tcs = new UniTaskCompletionSource<string>();

            void OnResponseSelected(object sender, SelectedResponseEventArgs e)
            {
                // unsubscribe
                DialogueManager.instance.conversationView.SelectedResponseHandler -= OnResponseSelected;
                // send result
                tcs.TrySetResult(e.response.formattedText.text);
            }

            // subscribe to response selected event
            DialogueManager.instance.conversationView.SelectedResponseHandler += OnResponseSelected;

            // start conversation
            DialogueManager.StartConversation(conversation);
            await tcs.Task;
        }

    }
}