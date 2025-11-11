using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

using Items;

using EventChannel;
using System;
using UnityEngine.Events;
using Items.Properties;
using Material = Items.Properties.Material;

namespace Player
{
    public class Inventory : SerializedMonoBehaviour
    {
        [Title("Notifications")]
        [ValueDropdown("@NotificationDataSO.GetKeys()")]
        [SerializeField] private string _itemAddedKey;
        [ValueDropdown("@NotificationDataSO.GetKeys()")]
        [SerializeField] private string _inventoryFullKey;
        [ValueDropdown("@NotificationDataSO.GetKeys()")]
        [SerializeField] private string _weaponBrokenKey;
        [ValueDropdown("@NotificationDataSO.GetKeys()")]
        [SerializeField] private string _unequipErrorKey;

        [TitleGroup("Invoked Events")]
        [SerializeField] private StringBoolEventSO _notificationSO;
        [Tooltip("Event invoked when a weapon is broken/becomes unusable")]
        [SerializeField] [Required] private WeaponTypeBoolEventSO _toggleWeaponAbilitiesEvent;
        [SerializeField] private WeaponTypeBoolEventSO _weaponBeltStatusEvent; 
        [SerializeField] private FloatVector2RequestEventSO _changeHealthSO;
        [SerializeField] private StringIntEventSO _itemCountChangedEvent;

        [TitleGroup("Listening to Events")]
        [SerializeField] private PlayerStaticDataEventSO _loadStaticDataEvent;
        [Header("CRUD Events")]
        [SerializeField] private ItemEventSO _addItemSO;
        [SerializeField] private IntEventSO _useItemSO; // invIndex
        [SerializeField] private WeaponTypeEventSO _useWeaponSO;
        [SerializeField] private IntIntEventSO _removeItemByIndexEvent;
        [SerializeField] private StringIntEventSO _removeItemByIdEvent;

        [Header("Equipment Events")]
        // weapon fields
        [SerializeField] private IntBoolEventSO _equipWeaponInputSO; // invoked by player in inventory menu

        // hold currently equipped weapons
        [field: SerializeField] public Dictionary<WeaponType, ItemInstance> WeaponBelt { get; private set; } = new()
        {
            { WeaponType.Sword, null },
            { WeaponType.Bow, null },
            { WeaponType.Staff, null }
        };

        // list to hold inventory
        [field: SerializeField] public List<ItemInstance> Items { get; private set; } = new();

        public int MaxSize { get; private set; } = 8;


        #region Singleton
        // singleton reference
        private static Inventory _instance;
        public static Inventory Instance { get { return _instance; } }

        private void Awake()
        {
            // singleton pattern
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }
        }
        #endregion

        private void OnEnable()
        {
            _loadStaticDataEvent.OnInvokeEvent += OnLoadStaticData;

            _addItemSO.OnInvokeEvent += AddItem;
            _useItemSO.OnInvokeEvent += Use;
            _useWeaponSO.OnInvokeEvent += UseWeapon;
            _removeItemByIndexEvent.OnInvokeEvent += RemoveByIndex;
            _removeItemByIdEvent.OnInvokeEvent += RemoveById;
            
            _equipWeaponInputSO.OnInvokeEvent += Equip;
        }

        private void OnDisable()
        {
            _loadStaticDataEvent.OnInvokeEvent -= OnLoadStaticData;

            _addItemSO.OnInvokeEvent -= AddItem;
            _useItemSO.OnInvokeEvent -= Use;
            _useWeaponSO.OnInvokeEvent -= UseWeapon;
            _removeItemByIndexEvent.OnInvokeEvent -= RemoveByIndex;
            _removeItemByIdEvent.OnInvokeEvent -= RemoveById;

            _equipWeaponInputSO.OnInvokeEvent -= Equip;
        }

        #region Utility
        /// <summary>
        /// Check if an item instance would be placed in a new stack if it is added to inventory.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool NeedsNewStack(ItemInstance item)
        {
            // return true if item isn't stackable
            if (!item.HasProperty(PropertyType.Stackable)) { return true; }

            // otherwise, check if there is enough room in existing stacks
            return CheckExistingStacks(item);
        }

        // checks if item would fit into any existing item stacks upon being added
        private bool CheckExistingStacks(ItemInstance item)
        {
            // track extra capacity in current inventory stacks
            int capacity = 0;

            // look for matching items in inventory
            for (int index = 0; index < Items.Count; index++)
            {
                if (item.ItemId == Items[index].ItemId)
                {
                    // if it can fit new quantity onto current stack, return true
                    if (Items[index].Quantity + item.Quantity <= item.StackSize) { return true; }

                    // add any extra room on stack to capacity
                    capacity += (Items[index].StackSize - Items[index].Quantity);
                    // return true if there is enough capacity
                    if (capacity >= item.Quantity) { return true; }
                }
            }

            // return check if enough capacity was found
            return (capacity >= item.Quantity);
        }
        #endregion

        #region Save/Load Data

        public List<ItemData> GetItemData()
        {
            List<ItemData> itemData = new List<ItemData>();

            // add weapon belt data
            foreach (WeaponType type in WeaponBelt.Keys)
            {
                if (WeaponBelt[type] != null) { itemData.Add(WeaponBelt[type].GetData()); }
            }

            // add inventory data
            foreach (ItemInstance item in Items)
            {
                itemData.Add(item.GetData());
            }

            return itemData;
        }

        private void OnLoadStaticData(SaveData.PlayerStaticData data)
        {
            this.MaxSize = data.MaxInventorySize;

            foreach (ItemData itemData in data.Inventory)
            {
                // Load Item belt data
                // only weapon belt indexes are <100
                if (itemData.InvIndex < 100)
                {
                    WeaponBelt[(WeaponType)itemData.InvIndex] = new ItemInstance(itemData);
                }
                // load item inventory data
                else
                {
                    Items.Add(new ItemInstance(itemData));

                    // TODO: DETERMINE IF THIS IS NECESSARY!!!!!!!!!!!
                    SendQuestUpdate(itemData.ItemId, itemData.Quantity);
                }
            }
        }
        #endregion

        #region Notification
        /// <summary>
        /// Notify quest tracker about item changes
        /// </summary>
        /// <param name="itemGuid"></param>
        /// <param name="quantity"></param>
        private void SendQuestUpdate(string itemId, int quantity)
        {
            _itemCountChangedEvent.InvokeEvent(itemId, quantity);
        }

        private void NotifyItemAdded()
        {
            _notificationSO.InvokeEvent(_itemAddedKey, false);
        }
        private void NotifyItemsAdded()
        {
            _notificationSO.InvokeEvent(_itemAddedKey, true);
        }
        private void NotifyFull() { _notificationSO.InvokeEvent(_inventoryFullKey, false); }
        #endregion

        #region Inventory Index

        /*
         *  ==== Weapon Belt ====
         *   [00]    [01]    [02]
         *  (sword)  (bow)  (staff)
         * 
         *  ==== Inventory ====
         *   [100] [101] [102] etc.
         * 
         */

        private int GetNextIndex() { return 100 + Items.Count; }

        private int GetInvIndex(int index) { return index - 100; }
        /// <summary>
        /// Reset InvIndex for all items in inventory.
        /// Weapon belt indexes = (int)WeaponType
        /// Inventory indexes = 100 + ItemsIndex
        /// </summary>
        private void RefreshItemIndexes()
        {
            foreach (WeaponType type in WeaponBelt.Keys)
            {
                WeaponBelt[type]?.SetIndex((int)type);
            }

            for (int index = 0; index < Items.Count; index++)
            {
                Items[index].SetIndex(100 + index);
            }
        }
        #endregion

        #region Public Methods
        public bool GetWeaponFromBelt(WeaponType type, out ItemInstance item) 
        { 
            item = WeaponBelt[type];
            return item != null;
        }
        public bool GetItem(int index, out ItemInstance item)
        {
            if (index >= Items.Count)
            {
                item = null;
                return false;
            }
            else
            {
                item = Items[index];
                return true;
            }
        }
        public ItemInstance GetItem(int index)
        {
            if (index >= Items.Count) { return null; } 
            return Items[index];
        }
        
        // check if an item can fit in inventory
        public bool HasRoom(ItemInstance item) 
        {
            // check if the item will need a new inventory stack
            if (NeedsNewStack(item))
            {
                // if yes, return true only if inventory has room for another stack
                return Items.Count < MaxSize;
            }
            else
            {
                // otherwise, items will fit into existing stacks
                return true;
            }
        }

        // check if a number of items can fit into inventory
        // any more code than this would get complicated
        public bool HasRoom(int count)
        {
            // track theoretical new inventory size
            //int newSize = _inventory.Count;

            return count + Items.Count <= MaxSize;
        }

        public bool CheckEquippedWeapon(WeaponType type)
        {
            return WeaponBelt[type] != null;
        }
        public bool CheckEnabledWeapon(WeaponType type)
        {
            return (WeaponBelt[type] != null && WeaponBelt[type].Durability > 0);
        }
        public float GetDamage(WeaponType type) { return WeaponBelt[type].GetProperty<Weapon>().Damage; }
        #endregion

        #region Add Item Methods
        private void AddItem(ItemInstance item)
        {
            if (item.HasProperty(PropertyType.Stackable))
            {
                AddStackable(item);
            }
            else
            {
                AddNewStack(item);
            }
        }

        private void AddNewStack(ItemInstance item)
        {
            if (Items.Count < MaxSize)
            {
                item.SetIndex(GetNextIndex());
                Items.Add(item);
                // make notification singular or plural
                if (item.Quantity > 1) { NotifyItemsAdded(); }
                else { NotifyItemAdded(); }

                SendQuestUpdate(item.ItemId, item.Quantity);
            }
            else
            {
                NotifyFull();
            }
        }

        private void AddWithoutNotify(ItemInstance item)
        {
            if (Items.Count < MaxSize)
            {
                item.SetIndex(GetNextIndex());
                Items.Add(item);
            }
        }

        // add a stackable item to inventory
        private void AddStackable(ItemInstance item)
        {
            // check for an existing stack in inventory
            if (Items.Exists(x => x.ItemId.Equals(item.ItemId))) 
            {
                // add item to inventory, adding to existing stacks first
                for (int index = 0; index < Items.Count; index++)
                {
                    if (item.ItemId == Items[index].ItemId)
                    {
                        // skip if stack is already full
                        if (Items[index].Quantity >= item.StackSize) { continue; }
                        else if (Items[index].Quantity + item.Quantity <= item.StackSize)
                        {
                            // if new items can fit into a current stack, add to current stack and return
                            Items[index].ChangeQuantity(item.Quantity);
                            // make notification singular or plural
                            if (item.Quantity > 1) { NotifyItemsAdded(); }
                            else { NotifyItemAdded(); }

                            SendQuestUpdate(item.ItemId, item.Quantity);

                            return;
                        }
                        else
                        {
                            // calculate num needed to max out stack
                            int numToAdd = item.StackSize - Items[index].Quantity;
                            // subtract amount needed to max out stack size from item
                            item.ChangeQuantity(-numToAdd);
                            // max out current stack
                            Items[index].SetQuantity(item.StackSize);

                            SendQuestUpdate(item.ItemId, numToAdd);
                        }
                    }
                }
            }

            // if no existing items match the new item, or if there is quantity left over, add it as a new stack
            if (item.Quantity > 0) { AddNewStack(item); }
        }
        #endregion

        #region Use Methods
        private void Use(int index)
        {
            int invIndex = GetInvIndex(index);


            if (Items[invIndex].GetProperty(PropertyType.Potion, out Potion potion))
            {
                UsePotion(potion);
            }
            else if (Items[invIndex].GetProperty(PropertyType.SkillBook, out SkillBook skill))
            {

            }
            else if (Items[invIndex].GetProperty(PropertyType.Effect, out Effect effect))
            {

            }

            // decrease quantity by 1
            RemoveByIndex(index, 1);
        }

        private void UsePotion(Potion potion)
        {
            if (potion.HealthValue > 0)
            {
                _changeHealthSO.OnRequestEvent(potion.HealthValue);
            }
            // TODO: ADD MANA AND SPEED EFFECTS
        }

        private void UseWeapon(WeaponType type)
        {
            WeaponBelt[type].ChangeDurability(-1);

            // if weapon breaks
            if (WeaponBelt[type].Durability <= 0)
            {
                // disable weapon abilities
                _toggleWeaponAbilitiesEvent.InvokeEvent(type, false);

                // notify player
                _notificationSO.InvokeEvent(_weaponBrokenKey, false);
            }
        }
        #endregion

        #region Remove Methods
        private void RemoveByIndex(int index, int quantity)
        {
            string itemId;

            // only weapon belt item indexes are <100
            if (index < 100)
            {
                WeaponType type = (WeaponType)index;
                itemId = WeaponBelt[type].ItemId;

                // remove weapon
                WeaponBelt[type] = null;
                // unequip weapon
                _weaponBeltStatusEvent.InvokeEvent(type, false);
            }
            else
            {
                // if index > 100, item is in main inventory
                int invIndex = GetInvIndex(index);
                itemId = Items[invIndex].ItemId;

                // if there is only 1 of the item, just remove it
                if (Items[invIndex].Quantity == 1) { Items.RemoveAt(invIndex); }
                else
                {
                    // if item is stackable, remove requested quantity
                    Items[invIndex].ChangeQuantity(-quantity);
                    // remove item if stack is empty
                    if (Items[invIndex].Quantity <= 0) { Items.RemoveAt(invIndex); }
                }
            }

            RefreshItemIndexes();
            SendQuestUpdate(itemId, -quantity);
        }

        /// <summary>
        /// Remove an item with matching ID from inventory.
        /// Item MUST be in inventory, not weapon belt.
        /// </summary>
        /// <param name="guid">ID of item to remove</param>
        /// <param name="quantity">Number to remove</param>
        private void RemoveById(string itemId, int quantity)
        {
            #region Debug
            Debug.Assert(Items.Exists(x => x.ItemId.Equals(itemId)), $"RemoveByGuid Error: Item with ID {itemId} not found!");

            // ensure there are enough of desired item to remove
            int totalItems = 0;
            for (int index = 0; index < Items.Count; index++)
            {
                if (itemId == Items[index].ItemId)
                {
                    totalItems += Items[index].Quantity;
                    if (totalItems >= quantity) { break; }
                }
            }

            Debug.Assert(totalItems >= quantity, $"RemoveByGuid Error: Insufficient items with Guid {itemId}");
            #endregion

            int remaining = quantity;

            // iterate over all inventory stacks until required quantity is removed
            for (int index = Items.Count - 1; index >= 0; index--)
            {
                if (itemId == Items[index].ItemId)
                {
                    if (remaining > Items[index].Quantity)
                    {
                        // if stack contains fewer items than needed to remove, remove stack and continue 
                        // with remaining quantity
                        remaining -= Items[index].Quantity;
                        Items.RemoveAt(index);
                    }
                    else if (remaining == Items[index].Quantity) 
                    {
                        // if stack contains exactly enough items, remove stack and break
                        Items.RemoveAt(index);
                        break;
                    }
                    else
                    {
                        // if stack has more than enough items, subtract the quantity and break
                        Items[index].ChangeQuantity(-remaining);
                        break;
                    }
                }
            }

            RefreshItemIndexes();
            SendQuestUpdate(itemId, -quantity);
        }
        #endregion

        #region Equipment Methods

        private void Equip(int id, bool isEquipped)
        {
            if (isEquipped) { EquipWeapon(id); }
            else { UnequipWeapon(id); }
        }

        // this method cannot be called on a weapon that is already equipped
        private void EquipWeapon(int index)
        {
            // temp variable
            ItemInstance weapon = Items[GetInvIndex(index)];
            WeaponType type = weapon.GetProperty<Weapon>().WeaponType;

            // remove weapon from inventory
            Items.Remove(weapon);
            SendQuestUpdate(weapon.ItemId, -1);

            // if there is already an equipped weapon of that type, add it back to inventory
            if (WeaponBelt[type] != null) 
            {
                AddWithoutNotify(WeaponBelt[type]);
                SendQuestUpdate(weapon.ItemId, 1);
            }

            // set equipped weapon
            WeaponBelt[type] = weapon;
            // refresh indexes
            RefreshItemIndexes();

            // if weapon has durability, enable skills for this weapon type
            if (WeaponBelt[type].Durability > 0) { _toggleWeaponAbilitiesEvent.InvokeEvent(type, true); }
            else { _toggleWeaponAbilitiesEvent.InvokeEvent(type, false); }

            // broadcast equipped weapon
            _weaponBeltStatusEvent.InvokeEvent(type, true);
        }

        private void UnequipWeapon(int index)
        {
            // check if there is room in inventory to unequip weapon
            if (Items.Count < MaxSize)
            {
                // index should be equal to weapon type
                WeaponType type = (WeaponType)index;
                // get item to remove
                ItemInstance weapon = WeaponBelt[type];

                // remove weapon from equipped slots
                WeaponBelt[type] = null;
                // add weapon to inventory
                AddWithoutNotify(weapon);
                // notify change
                SendQuestUpdate(weapon.ItemId, 1);

                // change status to unarmed
                _weaponBeltStatusEvent.InvokeEvent(type, false);
            }
            else
            {
                // send message that weapon has no room to be unequipped
                _notificationSO.InvokeEvent(_unequipErrorKey, false);
            }
        }
        #endregion
    }
}