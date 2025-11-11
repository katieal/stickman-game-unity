using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Characters
{
    [CreateAssetMenu(fileName = "PlayerSettingsSO", menuName = "Scriptable Objects/Settings/Player")]
    public class PlayerSettingsSO : CharacterSettingsSO, IDamagableSettings, IManaSettings
    {
        #region IDamagable 
        [field: Title("Damagable Stats")]
        [field: SerializeField] public float MaxHealth { get; private set; } = 100;

        [field: Tooltip("Number seconds to wait between each attack.")]
        [field: SerializeField] public float AttackSpeed { get; private set; } = 2;

        [field: Tooltip("The base amount of knockback this character takes after getting hit.")]
        [field: SerializeField] public Vector2 Knockback { get; private set; } = new Vector2(10, 0);

        [field: Tooltip("Character is frozen for this many seconds after getting hit.")]
        [field: SerializeField] public float StaggerSeconds { get; private set; } = 0.2f;
        #endregion

        [field: Title("Player Stats")]
        [field: SerializeField] public float MaxMana { get; private set; } = 100;

        [field: Title("Default Combat Abilities")]
        [field: SerializeField]
        public Dictionary<Items.WeaponType, Combat.AbilitySelectionPlayer[]> DefaultAbilities { get; private set; } = new()
        {
            {Items.WeaponType.Sword, new Combat.AbilitySelectionPlayer[4] },
            {Items.WeaponType.Bow, new Combat.AbilitySelectionPlayer[4] },
            {Items.WeaponType.Staff, new Combat.AbilitySelectionPlayer[4] }
        };
    }
}