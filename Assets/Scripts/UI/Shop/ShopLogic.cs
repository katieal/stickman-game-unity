using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Sirenix.OdinInspector;

using Items;
using EventChannel;
using Cysharp.Threading.Tasks;
using Items.Properties;
namespace Shop
{
    public class ShopLogic : MonoBehaviour
    {
        [Title("Shop Components")]
        [Tooltip("The Content object for the store's inventory viewport")]
        [SerializeField] private Transform _shopContainer;
        [Tooltip("Prefab for store buttons")]
        [SerializeField] private GameObject _shopButtonPrefab;

        [Title("Transaction Screen Components")]
        [Tooltip("The content object for the transaction screen viewport")]
        [SerializeField] private Transform _purchaseContainer;
        [Tooltip("Prefab for transaction entry panel")]
        [SerializeField] private GameObject _purchasePrefab;
        [Tooltip("Text for displaying total cost of purchase")]
        [SerializeField] private TMP_Text _totalText;
        [Tooltip("Button to make purchase")]
        [SerializeField] private UnityEngine.UI.Button _purchaseButton;

        [Title("Notifications")]
        [ValueDropdown("@NotificationDataSO.GetKeys()")]
        [SerializeField] private string _inventoryFullKey;
        [ValueDropdown("@NotificationDataSO.GetKeys()")]
        [SerializeField] private string _moneyErrorKey;

        [Title("Shop Items")]
        [Tooltip("Items to list for sale")]
        [AssetList(CustomFilterMethod = nameof(IsSellableItem))]
        [SerializeField] private List<ItemSO> _itemsToList;
        // TODO in the future: change to ItemEntry in order to customize list order and limit quantity that can be purchased

        [Title("Invoked Events")]
        [SerializeField] private IntIntRequestEventSO _changeMoneySO;
        [SerializeField] private ItemEventSO _addItemSO;
        [SerializeField] private StringBoolEventSO _notificationSO;

        //[Title("Request Events (Invoke and Listen)")]
        //[SerializeField] private IntBoolRequestEventSO _checkBalanceSO;

        private List<PurchaseEntry> _purchaseList = new();
        private int _totalCost;

        private void Awake()
        {
            InitButtons();
        }

        private void OnEnable()
        {
            _totalCost = 0;
            _totalText.text = "0";
        }
        private void OnDisable()
        {
            // reset transaction screen
            ResetTransaction();
        }

        #region Inspector
        private bool IsSellableItem(ItemSO item)
        {
            return item.HasProperty(PropertyType.Sellable);
        }
        #endregion

        private void InitButtons()
        {
            foreach (ItemSO item in _itemsToList)
            {
                // instantiate new button and child it to button container
                ShopButton shopButton = Instantiate(_shopButtonPrefab, _shopContainer).GetComponent<ShopButton>();

                // init button
                shopButton.Init(GameData.ItemDatabase.Instance.GetIcon(item.ItemId), item.DisplayName, item.GetProperty<Sellable>().BuyPrice);
                // set button callback
                shopButton.Button.onClick.AddListener(() => AddItemToBuy(item));
            }
            // purchase callback
            _purchaseButton.onClick.AddListener(() => MakePurchase());
        }

        #region Hard References
        private bool CheckPlayerBalance(int cost)
        {
            //Debug.Log("cost: " + cost);
            return Player.Wallet.Instance.CheckBalance(cost);
        }

        private bool HasRoomInInventory(int count)
        {
            return Player.Inventory.Instance.HasRoom(count);
        }
        #endregion

        private void AddItemToBuy(ItemSO item)
        {
            // check if item can be added to existing purchase list
            if (CheckForExistingItem(item, out int itemIndex) != -1)
            {
                // add 1 to the purchase entry
                _purchaseList[itemIndex].ChangeQuantity(1);
            }
            else
            {
                // create new purchase entry
                PurchaseEntry entry = Instantiate(_purchasePrefab, _purchaseContainer).GetComponent<PurchaseEntry>();

                // limit max num of items in a single purchase entry to stack size
                int max = 1;
                if (item.GetProperty(PropertyType.Stackable, out Stackable stackable))
                {
                    max = stackable.StackSize;
                }
                entry.Init(item, 1, max, item.GetProperty<Sellable>().BuyPrice);

                // add listeners
                entry.QuantityInput.onEndEdit.AddListener(delegate { OnQuantityChanged(entry); });
                entry.DeleteButton.onClick.AddListener(delegate { DeleteEntry(entry); });

                // add item to list of purchases
                _purchaseList.Add(entry);
            }

            // display item total
            CalculateTotal();
        }

        private int CheckForExistingItem(ItemSO item, out int itemIndex)
        {
            // check if item is already in list and if there is room in the stack, return -1 if not
            for (int index = 0; index < _purchaseList.Count; index++)
            {
                if (_purchaseList[index].Item.ItemId == item.ItemId && _purchaseList[index].Quantity < _purchaseList[index].MaxQuantity)
                {
                    itemIndex = index;
                    return itemIndex;
                }
            }
            itemIndex = -1;
            return itemIndex;
        }

        private void OnQuantityChanged(PurchaseEntry purchase)
        {
            if (purchase.Quantity == 0) 
            {
                DeleteEntry(purchase);
            }
            else if (purchase.Quantity > purchase.MaxQuantity)
            {
                purchase.SetQuantity(purchase.MaxQuantity);
            }

            // recalculate total screen
            CalculateTotal();
        }

        // delete the entry located at index
        private void DeleteEntry(PurchaseEntry purchase)
        {
            int index = _purchaseList.IndexOf(purchase);
            _purchaseList[index].DestroySelf();
            _purchaseList.RemoveAt(index);
            CalculateTotal();
        }

        private void CalculateTotal()
        {
            _totalCost = 0;
            foreach (PurchaseEntry entry in _purchaseList)
            {
                _totalCost += entry.CalculateTotalCost();
            }

            // set color of total text based on if player has enough money
            if (CheckPlayerBalance(_totalCost))
            {
                _totalText.color = Color.black;
            }
            else { _totalText.color = Color.red; }

            // set text
            _totalText.text = _totalCost.ToString();
        }

        private void MakePurchase()
        {
            // check if player inventory has room for all the items
            if (!HasRoomInInventory(_purchaseList.Count)) 
            {
                // send inventory full notification
                _notificationSO.InvokeEvent(_inventoryFullKey, false);
                return;
            }

            if (!CheckPlayerBalance(_totalCost))
            {
                // send not enough money notification
                _notificationSO.InvokeEvent(_moneyErrorKey, false);
                return;
            }

            // subtract total from player balance
            _changeMoneySO.OnRequestEvent(-_totalCost);
            // add items to player inventory
            foreach (PurchaseEntry purchase in _purchaseList)
            {
                _addItemSO.InvokeEvent(new ItemInstance(purchase.Item, purchase.Quantity));
            }

            // reset window
            ResetTransaction();
        }

        // reset entire transaction screen
        private void ResetTransaction()
        {
            for (int index = _purchaseList.Count - 1;  index >= 0; index--)
            {
                _purchaseList[index].DestroySelf();
            }

            _purchaseList.Clear();

            // reset total
            _totalCost = 0;
            _totalText.color = Color.black;
            _totalText.text = "0";
        }
    }
}
