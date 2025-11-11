using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Items;
using TMPro;
using Sirenix.OdinInspector;
using UnityEngine.Events;

namespace UserInterface
{
    public class ItemSlot : MonoBehaviour, IPointerClickHandler
    {
        [Title("Components")]
        [SerializeField] private Image _itemIcon;
        [SerializeField] private GameObject _selectedPanel;
        [SerializeField] private GameObject _quantityPanel;
        [SerializeField] private TMP_Text _quantityText;
        [field: Tooltip("Is this slot part of the weapon belt?")]
        [field: SerializeField] public bool IsWeaponBelt { get; private set; }
        [field: SerializeField][field: ShowIf("IsWeaponBelt")] public WeaponType WeaponType { get; private set; }

        // click callbacks
        public UnityEvent<ItemInstance> OnLeftClick;
        public UnityEvent<ItemInstance, Vector2> OnRightClick;

        // slot is enabled/disabled depending on player inventory size
        public bool IsEnabled { get; private set; }

        // storing the item in a var in this script means item can be changed w/o needing to change callbacks
        private ItemInstance _item;

        #region Init
        public void SetItem(ItemInstance newItem)
        {
            SetEmpty();
            _item = newItem;
            _itemIcon.gameObject.SetActive(true);
            _itemIcon.sprite = GameData.ItemDatabase.Instance.GetIcon(newItem.ItemId);
            if (newItem.HasProperty(PropertyType.Stackable))
            {
                _quantityPanel.SetActive(true);
                _quantityText.text = newItem.Quantity.ToString();
            }
        }

        public void SetEmpty()
        {
            _item = null;
            _itemIcon.gameObject.SetActive(false);
            _itemIcon.sprite = null;
            _quantityPanel.SetActive(false);
            _quantityText.text = string.Empty;
            Unhighlight();
        }
        #endregion

        private void OnDisable()
        {
            RemoveListeners();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_item == null) { return; }

            if (eventData.button == PointerEventData.InputButton.Left)
            {
                OnLeftClick?.Invoke(_item);
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                OnRightClick?.Invoke(_item, eventData.pressPosition);
            }
        }

        public void RemoveListeners()
        {
            OnLeftClick.RemoveAllListeners(); 
            OnRightClick.RemoveAllListeners();
        }

        public void SetEnabled(bool enabled) { IsEnabled = enabled; this.gameObject.SetActive(enabled); }

        public void Highlight()
        {
            _selectedPanel.SetActive(true);
        }

        public void Unhighlight()
        {
            _selectedPanel.SetActive(false);
        }
    }
}
