using Sirenix.OdinInspector;
using UnityEngine;

namespace Characters
{
    [CreateAssetMenu(fileName = "CharacterSettingsSO", menuName = "Scriptable Objects/Settings/Base Character")]
    public class CharacterSettingsSO : SerializedScriptableObject
    {
        [TextArea] public string Description;

        [field: SerializeField] public float MoveSpeed { get; private set; } = 1f;
    }

    public interface IDamagableSettings
    {
        [Title("Damagable Stats")]
        public float MaxHealth { get; }

        [Tooltip("Number seconds to wait between each attack.")]
        public float AttackSpeed { get; }

        [Tooltip("The base amount of knockback this character takes after getting hit.")]
        public Vector2 Knockback { get; }

        [Tooltip("Character is frozen for this many seconds after getting hit.")]
        public float StaggerSeconds { get; }
    }

    public interface IManaSettings
    {
        public float MaxMana { get; }
    }
}