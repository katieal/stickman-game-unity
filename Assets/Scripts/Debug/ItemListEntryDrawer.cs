using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Items
{
    [Serializable]
    public struct ListEntry
    {
        public ItemSO ItemSO;
        public int Quantity;
    }

    [CustomPropertyDrawer(typeof(ListEntry))]
    public class ItemListEntryDrawer : PropertyDrawer
    {

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement container = new();

            var itemField = new PropertyField(property.FindPropertyRelative("ItemSO"));
            var quantityField = new PropertyField(property.FindPropertyRelative("Quantity"));

            container.Add(itemField);
            container.Add(quantityField);

            return container;
        }
    }
}
