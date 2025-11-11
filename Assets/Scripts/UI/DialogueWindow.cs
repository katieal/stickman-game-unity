using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

using EventChannel;
using Characters.Dialogue;
using System.Collections;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace UserInterface
{
    public class DialogueWindow : MonoBehaviour, IPointerClickHandler
    {
        [Title("Dialogue Components")]
        [SerializeField] private GameObject _dialoguePanel;
        [SerializeField] private TMP_Text _dialogueText;

        [Title("Invoked Events")]
        [SerializeField] private GameStateRequestEventSO _gameStateEvent;
        [SerializeField] private BoolEventSO _greyBackgroundEvent;

        [Title("UniTask Events")]
        [SerializeField] private StringListVoidUniTaskEventSO _dialogueEvent;
        [SerializeField] private StringListVoidUniTaskEventSO _plainDialogueEvent;

        private List<string> _currentLines;
        private int _dialogueIndex = 0;
        private bool _isTyping = false;

        // maybe think of a better way to do this?
        private bool _isPlainDialogue = false;

        private Coroutine _typingCoroutine;

        private void Awake()
        {
            _dialoguePanel.SetActive(false);
        }
        private void OnEnable()
        {
            _dialogueEvent.OnStartEvent += OnShowDialogue;
            _plainDialogueEvent.OnStartEvent += OnShowPlainDialogue;
        }
        private void OnDisable()
        {
            _dialogueEvent.OnStartEvent -= OnShowDialogue;
            _plainDialogueEvent.OnStartEvent -= OnShowPlainDialogue;
        }

        private void OnShowDialogue(List<string> dialogue)
        {
            _isPlainDialogue = false;
            // pause game
            _gameStateEvent.RequestEvent(Managers.GameStateMachine.StateType.Pause);
            _greyBackgroundEvent.InvokeEvent(true);

            _currentLines = dialogue;
            _dialogueIndex = 0;
            // enable dialogue box and display first text
            _dialogueText.text = "";
            _dialoguePanel.SetActive(true);
            ShowText();
        }

        private void CloseDialogue()
        {
            _dialoguePanel.SetActive(false);
            // resume game
            _greyBackgroundEvent.InvokeEvent(false);
            _gameStateEvent.RequestEvent(Managers.GameStateMachine.StateType.Run);

            _dialogueEvent.EndEvent();
        }

        /// <summary>
        /// Show dialogue without any additional screen effects
        /// </summary>
        /// <param name="prompt"></param>
        private void OnShowPlainDialogue(List<string> prompt)
        {
            _isPlainDialogue = true;

            _currentLines = prompt;
            _dialogueIndex = 0;
            // enable dialogue box and display first text
            _dialogueText.text = "";
            _dialoguePanel.SetActive(true);
            ShowText();
        }

        private void ClosePlainDialogue()
        {
            _dialoguePanel.SetActive(false);
            _plainDialogueEvent.EndEvent();
        }

        private void ShowText()
        {
            _dialogueText.text = "";
            _typingCoroutine = StartCoroutine(TextCoroutine(_currentLines[_dialogueIndex]));
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            // check if text is currently being typed
            if (!_isTyping)
            {
                // increment index
                _dialogueIndex++;

                // if there are still more lines to show
                if (_dialogueIndex < _currentLines.Count)
                {
                    // show next section 
                    ShowText();
                }
                else
                {
                    // if all lines have been shown, close dialogue window
                    if (_isPlainDialogue) { ClosePlainDialogue(); }
                    else { CloseDialogue(); }
                }
            }
            else
            {
                StopCoroutine(_typingCoroutine);
                _isTyping = false;
                _dialogueText.text = _currentLines[_dialogueIndex];
            }
        }

        private IEnumerator TextCoroutine(string text)
        {
            _isTyping = true;

            for (int index = 0; index < text.Length; index++)
            {
                _dialogueText.text += text[index];
                yield return new WaitForSecondsRealtime(0.05f);
            }

            _isTyping = false;
        }

        [Button]
        public void ShowBox(bool isShown)
        {
            _dialoguePanel.SetActive(isShown);
        }

        [Button]
        public void AddText()
        {
            _dialogueText.text += "e";
        }
    }
}