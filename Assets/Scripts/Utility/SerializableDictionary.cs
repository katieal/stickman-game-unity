using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    // -----------
    // Serializable dictionary classes (mainly used for JSON save data)
    // source: https://www.reddit.com/r/Unity3D/comments/1gepsv3/why_doesnt_unity_serialize_dictionaries
    // -----------

    /// <summary>
    /// Standard serializable dictionary class
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    [Serializable]
    public class SerializableDictionary<K, V> : Dictionary<K, V>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<K> m_Keys = new List<K>();

        [SerializeField]
        private List<V> m_Values = new List<V>();

        public void OnBeforeSerialize()
        {
            m_Keys.Clear();
            m_Values.Clear();
            using Enumerator enumerator = GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<K, V> current = enumerator.Current;
                m_Keys.Add(current.Key);
                m_Values.Add(current.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            Clear();
            for (int i = 0; i < m_Keys.Count; i++)
            {
                Add(m_Keys[i], m_Values[i]);
            }

            m_Keys.Clear();
            m_Values.Clear();
        }
    }

    /// <summary>
    /// Serializable dictionary class with [SerializeReference] values
    /// </summary>
    /// <typeparam name="K">Key type</typeparam>
    /// <typeparam name="V">Serialized Reference value type</typeparam>
    [Serializable]
    public class SerializableRefValueDictionary<K, V> : Dictionary<K, V>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<K> m_Keys = new List<K>();

        [SerializeReference]
        private List<V> m_Values = new List<V>();

        public void OnBeforeSerialize()
        {
            m_Keys.Clear();
            m_Values.Clear();
            using Enumerator enumerator = GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<K, V> current = enumerator.Current;
                m_Keys.Add(current.Key);
                m_Values.Add(current.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            Clear();
            for (int i = 0; i < m_Keys.Count; i++)
            {
                Add(m_Keys[i], m_Values[i]);
            }

            m_Keys.Clear();
            m_Values.Clear();
        }
    }
}