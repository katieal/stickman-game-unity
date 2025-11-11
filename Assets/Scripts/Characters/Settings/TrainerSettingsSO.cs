using Sirenix.OdinInspector;
using UnityEngine;

namespace Characters
{
    [CreateAssetMenu(fileName = "TrainerSettingsSO", menuName = "Scriptable Objects/Settings/Trainer")]
    public class TrainerSettingsSO : CharacterSettingsSO, IDamagableSettings
    {
        #region IDamagable 
        [field: Title("Damagable Stats")]
        [field: SerializeField] public float MaxHealth { get; private set; }

        [field: Tooltip("Number seconds to wait between each attack.")]
        [field: SerializeField] public float AttackSpeed { get; private set; }

        [field: Tooltip("The base amount of knockback this character takes after getting hit.")]
        [field: SerializeField] public Vector2 Knockback { get; private set; } = new Vector2(10, 0);

        [field: Tooltip("Character is frozen for this many seconds after getting hit.")]
        [field: SerializeField] public float StaggerSeconds { get; private set; } = 0.2f;
        #endregion

        // ********** TODO: ADD IN COMBAT STUFF **********

        //[Title("Trainer Info")]
        //[field: SerializeField] public ItemData.LootTable LootTable { get; private set; } = new ItemData.LootTable();
    }
}