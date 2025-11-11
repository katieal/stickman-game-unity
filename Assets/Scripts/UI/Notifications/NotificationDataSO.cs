using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UserInterface.Notification
{
    // [ValueDropdown("@NotificationDataSO.GetKeys()")]
    // [SerializeField] private string _itemAddedKey;

    [InlineProperty(LabelWidth = 90)]
    public struct NotificationData
    {
        public Type Type;
        public string Message;
        public bool HasVariant;
        [ShowIf("HasVariant")] public string Variant;
    }

    [CreateAssetMenu(fileName = "NotificationDataSO", menuName = "Scriptable Objects/Notification Data")]
    public class NotificationDataSO : SerializedScriptableObject
    {
        [Title("Info")]
        public int MaxMessageLength = 20;

        [Title("Message Dict")]
        [DictionaryDrawerSettings(ValueLabel = "Message"), OnCollectionChanged("AfterCollectionChanged")]
        [ShowInInspector] [Searchable] public Dictionary<string, NotificationData> Messages = new Dictionary<string, NotificationData> ();

        private static Dictionary<string, NotificationData> _staticDict;

        public void AfterCollectionChanged(CollectionChangeInfo info, object value)
        {
            if (info.ChangeType == CollectionChangeType.SetKey) 
            {
                Debug.Assert(Messages[info.Key.ToString()].Message.Length <= MaxMessageLength, "Message is too long!");
                if (Messages[info.Key.ToString()].HasVariant) Debug.Assert(Messages[info.Key.ToString()].Variant.Length <= MaxMessageLength, "Variant is too long!");
            }
        }

        private void OnEnable()
        {
            _staticDict = Messages;
        }

        public static IEnumerable GetKeys()
        {
            return _staticDict.Keys;
        }
    }
}
