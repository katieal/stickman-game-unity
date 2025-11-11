using EventChannel;
using Items;
using Player;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UserInterface;
namespace Shop
{
    public class SellLogic : MonoBehaviour, IPointerClickHandler
    {
        [Header("Items Components")]
        [SerializeField] private GameObject _weaponBeltPanel;
        [SerializeField] private GameObject _inventoryPanel;
        [SerializeField] private GameObject _itemsPanel;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _descriptionText;

        [Header("Transaction Screen Components")]
        [Tooltip("The content object for the transaction screen viewport")]
        [SerializeField] private Transform _transactionContainer;
        [Tooltip("Prefab for transaction entry panel")]
        [SerializeField] private GameObject _transactionPrefab;
        [Tooltip("Text for displaying transaction total")]
        [SerializeField] private TMP_Text _totalText;
        [Tooltip("Button to make transaction")]
        [SerializeField] private UnityEngine.UI.Button _completeButton;

        [Header("Invoked Events")]
        [SerializeField] private IntIntEventSO _removeItemSO;
        [SerializeField] private IntEventSO _changeMoneySO;

        private ItemSlot[] _inventorySlots;
        private ItemSlot _selectedSlot;
        private List<PurchaseEntry> _sellList = new();
        private int _total;


        private void Awake()
        {
            _inventorySlots = _itemsPanel.GetComponentsInChildren<ItemSlot>();
        }

        private void OnEnable()
        {
            UpdateUI();
        }
        private void OnDisable()
        {
            
        }

        private void UpdateUI()
        {
            //ResetPanel();

            //for (int index = 0; index < _inventorySlots.Length; index++)
            //{
            //    ItemInstance item;

            //    if (_inventorySlots[index].IsWeaponBelt)
            //    {
            //        // set equipped weapon slots
            //        item = Inventory.Instance.WeaponBelt[(WeaponType)index];
            //    }
            //    else
            //    {
            //        // subtract 3 for the first 3 equipped weapon slots
            //        item = Inventory.Instance.GetItem(index - 3);
            //    }

            //    if (item != null)
            //    {
            //        // assign gear to slot
            //        _inventorySlots[index].SetItem(item);
            //    }
            //    else
            //    {
            //        //clear slot if inventory is empty
            //        _inventorySlots[index].ClearItem();
            //    }
            //}
        }

        public void OnPointerClick(PointerEventData eventData)
        {

            //// do nothing if object clicked isn't an inventory slot
            //if (!eventData.pointerCurrentRaycast.gameObject.CompareTag("InventorySlot")) { HideItemInfo(); return; }

            //ItemSlot slot = eventData.pointerCurrentRaycast.gameObject.GetComponent<ItemSlot>();

            //// do nothing if slot has no item
            //if (slot.IsEmpty()) { return; }


            //if (eventData.button == PointerEventData.InputButton.Left)
            //{
            //    // set slot to selected
            //    if (_selectedSlot != slot)
            //    {
            //        _selectedSlot = slot;
            //        slot.Highlight();
            //        ShowItemInfo(slot.Item);
            //    }


            //}
        }

        private void ShowItemInfo(ItemInstance item)
        {

        }
        private void HideItemInfo()
        {

        }

        private void AddItemForSale()
        {

        }

        private void ResetPanel()
        {

        }
    }
}
