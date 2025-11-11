using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Items.Properties;
using Material = Items.Properties.Material;

namespace Items
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Items/ItemSO")]
    public class ItemSO : SerializedScriptableObject
    {
        [Title("Base Data")]
        [Tooltip("Unique Item ID")]
        [Required] public string ItemId;
        [Tooltip("Display name of the item")]
        [Required] public string DisplayName;
        [TextArea] public string Description;
        [field: SerializeField] public Dictionary<PropertyType, IItemProperty> ItemProperties { get; private set; } = new();

        #region Inspector
        [Button]
        public void SyncProperties()
        {
            for (int index = 0; index < ItemProperties.Count; index++)
            {
                if (ItemProperties.ElementAt(index).Value == null)
                {
                    PropertyType type = ItemProperties.ElementAt(index).Key;
                    switch (type)
                    {
                        case PropertyType.Equipment: ItemProperties[type] = new Equipment(); break;
                        case PropertyType.Weapon: ItemProperties[type] = new Weapon(); break;
                        case PropertyType.Material: ItemProperties[type] = new Material(); break;
                        case PropertyType.Usable: ItemProperties[type] = new Usable(); break;
                        case PropertyType.Sellable: ItemProperties[type] = new Sellable(); break;
                        case PropertyType.Stackable: ItemProperties[type] = new Stackable(); break;
                        case PropertyType.Breakable: ItemProperties[type] = new Durability(); break;
                        case PropertyType.Potion: ItemProperties[type] = new Potion(); break;
                        case PropertyType.MobDrop: ItemProperties[type] = new MobDrop(); break;
                        default: Debug.Log($"{type} has not been implemented"); break;
                    }
                }
            }
        }

        [TitleGroup("Property Presets")]
        [ButtonGroup("Property Presets/Buttons")]
        public void AddWeapon()
        {
            this.ItemProperties.Add(PropertyType.Equipment, new Equipment());
            this.ItemProperties.Add(PropertyType.Weapon, new Weapon());
            this.ItemProperties.Add(PropertyType.Sellable, new Sellable());
            this.ItemProperties.Add(PropertyType.Breakable, new Durability());
        }
        [ButtonGroup("Property Presets/Buttons")]
        public void AddPotion()
        {
            this.ItemProperties.Add(PropertyType.Material, new Material());
            this.ItemProperties.Add(PropertyType.Usable, new Usable());
            this.ItemProperties.Add(PropertyType.Sellable, new Sellable());
            this.ItemProperties.Add(PropertyType.Stackable, new Stackable());
            this.ItemProperties.Add(PropertyType.Potion, new Potion());
        }
        #endregion

        public T GetProperty<T>() where T : IItemProperty
        {
            return ItemProperties.Values.OfType<T>().FirstOrDefault();
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
            if (!this.ItemProperties.ContainsKey(type)) 
            { 
                property = default(T);
                return false; 
            }

            // if item has property
            property = (T)this.ItemProperties[type];
            return true;
        }


        public bool HasProperty(PropertyType type)
        {
            return this.ItemProperties.ContainsKey(type);
        }
    }
}