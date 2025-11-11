using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UserInterface
{
    public class AbilitySlotSelectionUI : MonoBehaviour, IPointerClickHandler
    {
        [Title("Components")]
        [SerializeField] private TMP_Text _keybindText;
        [field: SerializeField] public int Index { get; private set; }

        // click callbacks
        public UnityEvent<int> OnLeftClick;

        #region Init
        public void SetKeybind(string keybindText)
        {
            _keybindText.text = keybindText;
        }
        #endregion

        private void OnEnable()
        {
            OnLeftClick.AddListener((x) => Highlight(true));
        }
        private void OnDisable()
        {
            OnLeftClick.RemoveAllListeners();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left) { OnLeftClick?.Invoke(Index); }
        }

        public void Highlight(bool isHighlighted)
        {
            // highlight panel
        }
    }
}