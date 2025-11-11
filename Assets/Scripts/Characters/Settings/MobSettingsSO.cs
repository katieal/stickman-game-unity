using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Characters
{
    public enum MobType { None, Slime, Boar, Beetle, Specter, Wyvern, Bear };
    //public enum CombatType { Aggressive, Normal, Passive }

    [CreateAssetMenu(fileName = "MobSettingsSO", menuName = "Scriptable Objects/Settings/Mob")]
    public class MobSettingsSO : CharacterSettingsSO, IDamagableSettings
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


        [field: Title("Mob Info")]
        [field: SerializeField] public MobType MobType;
        //[field: Tooltip("Radius of the attack range collider.")]
        //[field: SerializeField] public float AttackRange { get; private set; }
        //[field: SerializeField] public CombatType CombatType { get; private set; }

        //[field: Tooltip("Does this mob have ranged attacks?")]
        //[field: SerializeField] public bool IsRanged { get; private set; }

        [field: Tooltip("If true, enemy will be briefly frozen(staggered) every time they are damaged. For " +
            "enemies with longer stagger times, this could enable them to be stunlocked.")]
        [field: SerializeField] public bool IsStaggeredWhenHit { get; private set; }

        [field: SerializeField] public Items.LootTable LootTable { get; private set; } = new Items.LootTable();

        [field: Tooltip("List of Mob's combat abilities. Index = priority where 0 is highest.")]
        [field: SerializeField] public List<Combat.AbilitySelectionMob> Abilities { get; private set; }
    }
}