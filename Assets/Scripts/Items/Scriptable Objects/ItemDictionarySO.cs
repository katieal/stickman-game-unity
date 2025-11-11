using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Items
{
    // probably should rename to itemdatabase
    [CreateAssetMenu(fileName = "ItemDictionarySO", menuName = "Scriptable Objects/Items/Item Dictionary")]
    public class ItemDictionarySO : SerializedScriptableObject
    {
        // TODO future: look into searching by tag maybe? maybe add some subdivisions?
        [AssetSelector]
        public List<ItemSO> Items = new();
    }
}