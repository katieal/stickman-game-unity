using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Characters
{
    [CreateAssetMenu(fileName = "NpcSettingsSO", menuName = "Scriptable Objects/Settings/NPC")]
    public class NpcSettingsSO : SerializedScriptableObject
    {
        [field: SerializeField] public float MoveSpeed { get; private set; } = 1f;
    }
}