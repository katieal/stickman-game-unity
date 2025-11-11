using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utility;
using Items.Properties;
using Material = Items.Properties.Material;

namespace GameData
{
    public class ItemDatabase : SerializedMonoBehaviour
    {
        [field: Title("Database")]
        [SerializeField] private Items.ItemDictionarySO _items;
        [SerializeField] private IconDictionarySO _icons;

        #region Singleton
        // singleton reference
        private static ItemDatabase _instance;
        public static ItemDatabase Instance { get { return _instance; } }

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

        // in something like shop menu, keep list of item sos
        // on add, send that item's guid to this class to get instance
        public Items.ItemSO GetItem(string itemId) 
        {
            return _items.Items.Find(x => x.ItemId == itemId);
        }

        public Sprite GetIcon(string itemId)
        {
            return _icons.GetIcon(itemId);
        }


        [Button]
        public void SyncIconDictionary()
        {
            foreach (Items.ItemSO item in _items.Items)
            {
                if (!_icons.Icons.ContainsKey(item.ItemId)) {
                    _icons.Icons.Add(item.ItemId, null);
                }
            }
        }
    }
}