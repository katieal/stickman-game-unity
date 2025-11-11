using Items.Properties;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Material = Items.Properties.Material;

namespace Items 
{
    [Serializable]
    public struct ItemEntry
    {
        [AssetSelector(Paths = "Assets/Scriptable Objects/Items")]
        public ItemSO ItemSO;
        public int Quantity;
    }

    [Serializable]
    public struct ItemData
    {
        public string ItemId;
        public int InvIndex;
        public int Quantity;
        public int Durability;
    }

    // serializable dictionary for item properties
    [Serializable]
    public class ItemPropertyDict : Utility.SerializableRefValueDictionary<PropertyType, IItemProperty> { }

    [Serializable]
    public class ItemInstance
    {
        [field: SerializeField] public string ItemId { get; private set; }
        [field: SerializeField] public string DisplayName { get; private set; }  // display name
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public int Quantity { get; private set; }
        [field: SerializeField] public int Durability { get; private set; }
        [field: SerializeField] public int InvIndex { get; private set; } // for use in inventory only

        [field: SerializeField] public ItemPropertyDict Properties { get; private set; } = new();

        // utility
        public int StackSize { get { return this.GetProperty<Stackable>().StackSize; } }
        public int MaxDurability { get { return this.GetProperty<Durability>().MaxDurability; } }

        #region Constructors
        // the itemdata class uses this constructor to make instances
        public ItemInstance(ItemSO itemSO, int quantity = 1)
        {
            this.ItemId = itemSO.ItemId;
            this.DisplayName = itemSO.DisplayName;
            this.Description = itemSO.Description;
            this.Durability = -1; // item does not have durability by default
            this.Quantity = quantity;

            SetProperties(itemSO);
        }

        public ItemInstance(ItemData data)
        {
            ItemSO itemSO = GameData.ItemDatabase.Instance.GetItem(data.ItemId);

            this.ItemId = itemSO.ItemId;
            this.DisplayName = itemSO.DisplayName;
            this.Description = itemSO.Description;

            this.InvIndex = data.InvIndex;
            this.Durability = data.Durability;
            this.Quantity = data.Quantity;

            SetProperties(itemSO);
        }
        #endregion

        public ItemData GetData()
        {
            ItemData data = new ItemData()
            {
                ItemId = this.ItemId,
                InvIndex = this.InvIndex,
                Durability = this.Durability,
                Quantity = this.Quantity,
            };
            return data;
        }

        public void SetIndex(int index) { this.InvIndex = index; }

        #region Item Properties
        // init item properties
        private void SetProperties(ItemSO itemSO)
        {
            foreach (PropertyType type in itemSO.ItemProperties.Keys)
            {
                IItemProperty property = itemSO.ItemProperties[type];
                this.Properties.Add(type, property);

                if (type == PropertyType.Breakable)
                {
                    this.Durability = itemSO.GetProperty<Durability>().MaxDurability;
                }
            }
        }

        public T GetProperty<T>() where T : IItemProperty
        {
            return this.Properties.Values.OfType<T>().FirstOrDefault();
        }

        /// <summary>
        /// Method to access an item's property
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="property"></param>
        /// <returns>True if item has property, false if not</returns>
        public bool GetProperty<T>(PropertyType type, out T property) where T : IItemProperty
        {
            if (!this.HasProperty(type))
            {
                property = default(T);
                return false;
            }

            // if item has property
            property = (T)this.Properties[type];
            return true;
        }

        public bool HasProperty(PropertyType type)
        {
            return this.Properties.ContainsKey(type);
        }
        #endregion

        #region Mutators
        public void ChangeQuantity(int amount)
        {
            // if amount isn't being changed to 0, assert that it is stackable
            if (Quantity + amount != 0)
            {
                Debug.Assert(this.Properties.ContainsKey(PropertyType.Stackable), $"Can't change quantity of non stackable item {this.DisplayName}");
            }

            Quantity += amount;
            Quantity = Mathf.Clamp(Quantity, 0, this.GetProperty<Stackable>().StackSize);
        }
        public void SetQuantity(int quantity)
        {
            Quantity = quantity;
            Quantity = Mathf.Clamp(Quantity, 0, this.GetProperty<Stackable>().StackSize);
        }
        public void ChangeDurability(int amount)
        {
            // items with no durability will have negative max durability
            if (this.GetProperty<Durability>().MaxDurability < 0) { return; }

            Durability += amount;
            Durability = Mathf.Clamp(Durability, 0, this.GetProperty<Durability>().MaxDurability);
        }
        public void SetDurability(int durability)
        {
            // items with no durability will have negative max durability
            if (this.GetProperty<Durability>().MaxDurability < 0) { return; }

            Durability = durability;
            Durability = Mathf.Clamp(Durability, 0, this.GetProperty<Durability>().MaxDurability);
        }
        #endregion
    }
}