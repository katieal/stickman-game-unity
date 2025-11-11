using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.AddressableAssets.Build;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Items
{ 

    [Serializable]
    public class LootTable 
    {
        [RequiredListLength(MinLength = 1)]
        [Tooltip("List of possible items to drop on death.")]
        public List<ItemDrop> Loot;
        [Tooltip("Items from this table only drop in special circumstances.")]
        public List<ItemDrop> Special;
        
        public void AddSpecialDrop(ItemDrop itemDrop)
        {
            Special.Add(itemDrop);
        }


        public List<ItemInstance> CalculateLoot()
        {

            // list to hold drops
            List<ItemInstance> loot = new();

            // roll for each item in loot table
            foreach (ItemDrop item in Loot)
            {
                ItemInstance result = RollForItem(item);
                // leave redundant quantity check
                if (result != null && result.Quantity != 0) { loot.Add(result); }
            }
            
            return loot;
        }

        private ItemInstance RollForItem(ItemDrop item)
        {
            #region Debug
            if (item.DropChances.Count <= 0)
            {
                throw new Exception("item has no drop chances!!");
            }
            #endregion
            
            // a guaranteed drop
            if (item.DropChances.Count == 1)
            {
                // TODO: add quantity range in?
                if (item.DropChances[0].Quantity != 0)
                {
                    return new ItemInstance(item.Item, item.DropChances[0].Quantity);
                }
                else { return null; }
            }

            // roll for chances
            int roll = UnityEngine.Random.Range(0, 101);
            foreach (DropChance dropChance in item.DropChances)
            {
                if (roll >= dropChance.DropRange.x && roll <= dropChance.DropRange.y)
                {
                    if (dropChance.Quantity != 0)
                    {
                        return new ItemInstance(item.Item, dropChance.Quantity);
                    }
                    else { return null; }
                }
            }
            return null;
        }
    }
}
