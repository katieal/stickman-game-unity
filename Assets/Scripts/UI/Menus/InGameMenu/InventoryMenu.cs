using EventChannel;
using Items;
using Items.Properties;
using NUnit.Framework;
using Player;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UserInterface {
    public class InventoryMenu : MonoBehaviour, IPointerClickHandler
    {
        [Title("Info")]
        [SerializeField][TagField] private string _inventorySlotTag;

        [Title("Components")]
        [SerializeField] private OptionsController _optionsController;
        [SerializeField] private GameObject _weaponBeltPanel;
        [SerializeField] private GameObject _inventoryPanel;
        //[SerializeField] private GameObject _itemsPanel;
        [Title("Info Panel Components")]
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private GameObject _statsPanel;
        [SerializeField] private List<TMP_Text> _statsText = new();

        [Title("Invoked Events")]
        [SerializeField] private IntBoolEventSO _equipWeaponEvent; 
        [SerializeField] private IntEventSO _useItemSO; // invindex
        [SerializeField] private IntIntEventSO _removeItemByIndexEvent;

        private ItemSlot[] _beltSlots;
        private ItemSlot[] _invSlots;


        private void Awake()
        {
            _beltSlots = _weaponBeltPanel.GetComponentsInChildren<ItemSlot>();
            _invSlots = _inventoryPanel.GetComponentsInChildren<ItemSlot>();
        }

        private void OnEnable()
        {
            BindSlotCallbacks();
            UpdateUI();

            _optionsController.HideOptions();
        }
        private void OnDisable()
        {
            // close options menu
            _optionsController.HideOptions();
        }

        private void BindSlotCallbacks()
        {
            foreach (ItemSlot slot in _beltSlots)
            {
                slot.OnLeftClick.AddListener((itemInSlot) => ClearInformation());
                slot.OnLeftClick.AddListener((itemInSlot) => ShowItemInfo(itemInSlot));
                slot.OnLeftClick.AddListener((itemInSlot) => slot.Highlight());
                slot.OnRightClick.AddListener((itemInSlot, pos) => ShowOptions(itemInSlot, true, pos));
            }

            foreach (ItemSlot slot in _invSlots)
            {
                slot.OnLeftClick.AddListener((itemInSlot) => ClearInformation());
                slot.OnLeftClick.AddListener((itemInSlot) => ShowItemInfo(itemInSlot));
                slot.OnLeftClick.AddListener((itemInSlot) => slot.Highlight());
                slot.OnRightClick.AddListener((itemInSlot, pos) => ShowOptions(itemInSlot, false, pos));
            }
        }

        private void UpdateUI()
        {
            ClearInformation();

            // set weapon slot items
            foreach (ItemSlot slot in _beltSlots)
            {
                // set item if possible, otherwise set slot to empty
                if (Inventory.Instance.GetWeaponFromBelt(slot.WeaponType, out ItemInstance invItem))
                {
                    slot.SetItem(invItem);
                }
                else { slot.SetEmpty(); }
            }

            // set inventory slot items
            for (int index = 0; index < _invSlots.Length; index++)
            {
                // return false if no item at that index
                if (Inventory.Instance.GetItem(index, out ItemInstance invItem))
                {
                    _invSlots[index].SetItem(invItem);
                }
                else { _invSlots[index].SetEmpty(); }
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            // clear display info by clicking on the background window
            ClearInformation();
        }

        #region Info Panel
        private void ShowItemInfo(ItemInstance selected)
        {
            _nameText.text = selected.DisplayName;
            _descriptionText.text = selected.Description;

            // show stats if item is equipment
            if (selected.HasProperty(PropertyType.Equipment))
            {
                // show weapon stats
                if (selected.GetProperty(PropertyType.Weapon, out Weapon weapon))
                {
                    _statsPanel.SetActive(true);
                    _statsText[0].text = weapon.Damage.ToString();
                    _statsText[1].text = weapon.MagicDamage.ToString();
                    _statsText[2].text = weapon.Block.ToString();
                }

                // show durability stat
                if (selected.HasProperty(PropertyType.Breakable))
                {
                    if (selected.Durability <= 0) { _statsText[3].color = Color.red; }
                    else { _statsText[3].color = Color.white; }
                    _statsText[3].text = selected.Durability.ToString() + " / " + selected.MaxDurability.ToString();
                }
                else { _statsText[3].text = " -- "; }
            }
            else { _statsPanel.SetActive(false);  }
        }

        // clear UI info panel
        private void ClearInformation()
        {
            // clear all text
            _nameText.text = string.Empty;
            _descriptionText.text = string.Empty;
            // remove stats panel
            _statsPanel.SetActive(false);
            // unhighlight all slots
            UnhighlightAll();
            // close options menu
            _optionsController.HideOptions();
        }
        #endregion

        // unhighlight all equipment slots
        private void UnhighlightAll()
        {
            foreach (ItemSlot slot in _beltSlots) { slot.Unhighlight(); }
            foreach (ItemSlot slot in _invSlots) { slot.Unhighlight(); }
        }

        #region Options Menu
        private void ShowOptions(ItemInstance item, bool isWeaponBelt, Vector2 position)
        {
            // clear previous options menu data and callbacks
            _optionsController.HideOptions();

            // subscribe to equip/unequip buttons if item is equipment
            if (item.HasProperty(PropertyType.Equipment))
            {
                if (isWeaponBelt)
                {
                    _optionsController.UnequipButton.onClick.AddListener(() => EquipItem(item.InvIndex, false));
                }
                else { _optionsController.EquipButton.onClick.AddListener(() => EquipItem(item.InvIndex, true)); }
            }

            // subscribe to use button if item is usable
            if (item.HasProperty(PropertyType.Usable))
            {
                _optionsController.UseButton.onClick.AddListener(() => UseItem(item.InvIndex));
            }

            // subscribe to drop button if item is sellable (only sellable items can be dropped) 
            if (item.HasProperty(PropertyType.Sellable))
            {
                _optionsController.DropButton.onClick.AddListener(() => _optionsController.ShowDropItem(item.Quantity));
                _optionsController.OnDropItem.AddListener((quantity) => DropItem(item.InvIndex, quantity));
            }

            // show options menu
            _optionsController.ShowOptions(position, 
                item.HasProperty(PropertyType.Equipment) && !isWeaponBelt,
                item.HasProperty(PropertyType.Equipment) && isWeaponBelt,
                item.HasProperty(PropertyType.Usable),
                item.HasProperty(PropertyType.Sellable) );
        }


        private void EquipItem(int index, bool isEquipped)
        {
            _equipWeaponEvent.InvokeEvent(index, isEquipped);
            UpdateUI();
        }

        private void UseItem(int index)
        {
            // invoke with item InvIndex
            _useItemSO.InvokeEvent(index);
            UpdateUI();
        }

        private void DropItem(int index, int quantity)
        {
            _removeItemByIndexEvent.InvokeEvent(index, quantity);
            UpdateUI();
        }
        #endregion
    }
}
