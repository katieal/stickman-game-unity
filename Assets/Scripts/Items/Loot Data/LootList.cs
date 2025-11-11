using Sirenix.OdinInspector;
using System.Collections.Generic;
using System;
using UnityEngine;
using Sirenix.OdinInspector.Editor;
using UnityEngine.Events;

namespace Items
{
    [Serializable]
    public class DropChance
    {
        [Tooltip("Number of this item to be dropped.")]
        public int Quantity = 0;
        //public Vector2Int QuantityRange;

        [Tooltip("On random number from 0-100, if number falls between min (inclusive) and max (inclusive), this quantity will drop.")]
        [MinMaxSlider("Min", "Max", true)]
        [OnValueChanged("OnRangeChanged")]
        public Vector2Int DropRange;

        [HideInInspector] public float Min = 0;
        [HideInInspector] public float Max = 100;

        //public UnityAction<DropChance> OnDropRangeChanged;

        public void OnRangeChanged()
        {
            //OnDropRangeChanged.Invoke(this);
        }
    }

    [Serializable]
    public class ItemDrop
    {
        //[AssetList(CustomFilterMethod = "IsMobDrop")]
         public ItemSO Item;
        [RequiredListLength(MinLength = 1)] [ListDrawerSettings(DraggableItems = false, ShowFoldout = false, ShowIndexLabels = false)]
        public List<DropChance> DropChances;


        //private bool IsMobDrop(ItemSO item)
        //{
        //    return (item.Tags.Contains(PropertyType.MobDrop));
        //}

        /*
        [RequiredListLength(MinLength = 1)]
        [ListDrawerSettings(CustomAddFunction = "AddNewDropChance", DraggableItems = false)]
        [OnCollectionChanged(After = "AfterCollectionChanged")]
        public List<DropChance> DropChances;


        public DropChance AddNewDropChance()
        {
            DropChance drop = new DropChance();
            if (DropChances.Count > 0) { drop.Min = DropChances[^1].DropRange.y; }
            

            return drop;
        }

        public void AfterCollectionChanged(CollectionChangeInfo info, object value)
        {
            //Debug.Log(info);
        }

        public void OnDropChanceChanged(DropChance dropChance)
        {

        }
        */
    }
}