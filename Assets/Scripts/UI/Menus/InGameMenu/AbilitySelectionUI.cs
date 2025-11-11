using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UserInterface
{
    public class AbilitySelectionUI : MonoBehaviour, IPointerClickHandler
    {
        [Title("Components")]
        [SerializeField] private Image _abilityImage;
        [SerializeField] private TMP_Text _nameText;

        // click callbacks
        public UnityEvent<string> OnLeftClick;

        // hold ability ID
        private string _abilityId;

        // unlearned/disabled abilities will still be shown
        private bool _isEnabled;

        #region Init
        public void SetAbility(Sprite icon, string abilityName, string abilityId, bool isEnabled)
        {
            _abilityImage.sprite = icon;
            _nameText.name = abilityName;
            _abilityId = abilityId;
            _isEnabled = isEnabled;
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
            if (eventData.button == PointerEventData.InputButton.Left && _isEnabled) { OnLeftClick?.Invoke(_abilityId); }
        }

        public void Highlight(bool isHighlighted)
        {
            // highlight panel
        }
    }
}