using Cysharp.Threading.Tasks;
using EventChannel;
using PixelCrushers.DialogueSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Characters
{
    public class NpcController : SerializedMonoBehaviour, IInteractable
    {
        [Title("Components")]
        [SerializeField] private INpc _npcData;

        [Title("Invoked Events")]
        [SerializeField] private NpcNameEventSO _npcEvent;

        public void Interact()
        {
            _npcEvent.InvokeEvent(_npcData.Name);
        }


        /*
        [Title("Components")]
        [SerializeField] private NpcData _npcData;

        [Title("UniTask Events")]
        [SerializeField] private StringListVoidUniTaskEventSO _dialogueEvent;
        [SerializeField] private StringBoolUniTaskEventSO _checkQuestsEvent;

        /// <summary>
        /// Track current index for default dialogue lines
        /// </summary>
        private int _defaultIndex = 0;

        // var for readability
        private Dialogue.DialogueSetSO _dialogue;

        private void Awake()
        {
            _dialogue = _npcData.Dialogue;
        }

        public void Interact()
        {
            TalkWithPlayer().Forget();
        }

        private async UniTaskVoid TalkWithPlayer()
        {
            // check if npc can offer any qu ests
            _checkQuestsEvent.StartEvent(_npcData.Settings.NpcName);
            bool result = await _checkQuestsEvent.Task;
            // if no quest related dialogue can be sent, send default dialogue
            if (!result) { SendDefaultDialogue(); }
        }

        private void SendDefaultDialogue()
        {
            #region Debug
#if UNITY_EDITOR
            Debug.Assert(_dialogue.DefaultLines.Count > _defaultIndex, "Character needs default dialogue!");
#endif
            #endregion
            // send default dialogue lines
            _dialogueEvent.StartEvent(_dialogue.DefaultLines[_defaultIndex].Text);
            _defaultIndex++;

            if (_defaultIndex >= _dialogue.DefaultLines.Count) { _defaultIndex = 0; }
        }

        */
    }
}