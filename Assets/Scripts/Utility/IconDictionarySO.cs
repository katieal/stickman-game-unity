using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    [CreateAssetMenu(fileName = "IconDictionarySO", menuName = "Scriptable Objects/Icon Dictionary")]
    public class IconDictionarySO : SerializedScriptableObject
    {
        public Sprite DefaultSprite;

        // maybe change to a dictionary of GUIDs?
        public Dictionary<string, Sprite> Icons = new();


        public Sprite GetIcon(string name)
        {
            if (!Icons.ContainsKey(name)) { return  DefaultSprite; }
            return Icons[name];
        }
    }
}