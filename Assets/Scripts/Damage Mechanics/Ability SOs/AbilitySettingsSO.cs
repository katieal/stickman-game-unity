using Combat;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Combat
{
    [CreateAssetMenu(fileName = "AbilitySettingsSO", menuName = "Scriptable Objects/Combat/Combat Ability")]
    public class AbilitySettingsSO : ScriptableObject
    {
        [Title("Ability Info")]
        [Tooltip("Ability Display name")]
        public string DisplayName;
        [EnumToggleButtons] public AbilityType AbilityType;
        [Tooltip("The name of the trigger in the animation controller.")]
        public string AnimationTrigger = "Attack";
        [Tooltip("Does this ability have a separate casting animation? If yes, Attack is triggered by end of animation." +
            "Otherwise, Attack is triggered after AttackDelay seconds.")]
        public bool HasCastAnimation;
        [ShowIf("HasCastAnimation")] public string CastAnimationTrigger;
        [Tooltip("Amount of delay before Attack is called.")]
        [SuffixLabel("seconds", Overlay = true)]
        [HideIf("HasCastAnimation")] public float AttackDelay;
        [Tooltip("Can ability execution be interrupted?")]
        public bool IsCancelable;
        [EnumToggleButtons] public TriggerType TriggerType;
        [ShowIf("TriggerType", TriggerType.AbilityRange)]
        [Tooltip("Ability can trigger if enemy is within this range.")]
        public float TriggerRange;

        [Title("Ability Stats")]
        [Tooltip("The amount of mana it takes to cast this ability.")]
        [SuffixLabel("int", Overlay = true)]
        public int ManaCost = 0;
        [Tooltip("The time the entity must wait beefore using this ability again.")]
        [SuffixLabel("seconds", Overlay = true)]
        public float CooldownSeconds;

        [HideIfGroup("AbilityType", AbilityType.Spell, GroupID = "DamageGroup")]
        [TitleGroup("DamageGroup/Damage Stats")]
        [Tooltip("The base damage of the ability.")]
        public float Damage;
        [TitleGroup("DamageGroup/Damage Stats")]
        [Tooltip("The enemy will take this much times their regular knockback.")]
        public float KnockbackMultiplier = 1f;
        [TitleGroup("DamageGroup/Damage Stats")]
        [Tooltip("Enemies hit will be frozen for this many seconds.")]
        [SuffixLabel("seconds", Overlay = true)]
        public float StunSeconds = 0f;

        [ShowIfGroup("AbilityType", AbilityType.Mele, GroupID = "MeleGroup")]
        [TitleGroup("MeleGroup/Mele Stats")]
        public bool IsSingleTarget;

        [ShowIfGroup("AbilityType", AbilityType.Ranged, GroupID = "RangedGroup")]
        [TitleGroup("RangedGroup/Ranged Stats")] 
        [Tooltip("Name of projectile prefab in Spawnable Database")] public string ProjectileName;
        [TitleGroup("RangedGroup/Ranged Stats")] public float Velocity = 10f;

        [ShowIfGroup("AbilityType", AbilityType.Spell, GroupID = "SpellGroup")]
        [TitleGroup("SpellGroup/Spell Stats")]
        public float Health, Mana, Armor, Speed;
    }

    public enum AbilityType { Mele, Ranged, Charge, Spell }

    /// <summary>
    /// Default = character's default attack range
    /// AbilityRange = custom ability range
    /// OnSight = on enemy spotted
    /// </summary>
    public enum TriggerType { Default, AbilityRange, OnSight }

    public class AbilityInstance
    {
        /// <summary>
        /// Name of the ability's SO object
        /// </summary>
        public string Id;
        public string DisplayName;
        public AbilityType Type;
        public string AnimationTrigger;
        public bool HasCastAnimation;
        public string CastAnimationTrigger;
        public float AttackDelay;
        public bool IsCancelable;
        public TriggerType Trigger;
        public float TriggerRange;

        public int ManaCost;

        public Cooldown Cooldown;
        public int AbilityIndex; // index of ability in ability bar
        /// <summary>
        /// Is this ability usable?
        /// </summary>
        public bool IsEnabled;

        // Damage Info
        public float Damage;
        public float KnockbackMultiplier;
        public float StunSeconds;

        // Mele Info
        public bool IsSingleTarget;

        // Ranged Info
        public string ProjectileName;
        public float Velocity;

        // Spell Info
        public float Health;
        public float Mana;
        public float Armor;
        public float Speed;

        private void InitWithSettings(AbilitySettingsSO settings)
        {
            this.Id = settings.name;
            this.DisplayName = settings.DisplayName;
            this.Type = settings.AbilityType;
            this.AnimationTrigger = settings.AnimationTrigger;
            this.HasCastAnimation = settings.HasCastAnimation;
            this.CastAnimationTrigger = (settings.HasCastAnimation) ? settings.CastAnimationTrigger : null;
            this.AttackDelay = (settings.HasCastAnimation) ? 0 : settings.AttackDelay;
            this.IsCancelable = settings.IsCancelable;
            this.ManaCost = settings.ManaCost;
            this.Trigger = settings.TriggerType;
            this.TriggerRange = (settings.TriggerType == TriggerType.AbilityRange) ? settings.TriggerRange : 0;

            this.Damage = (settings.AbilityType != AbilityType.Spell) ? settings.Damage : 0;
            this.KnockbackMultiplier = (settings.AbilityType != AbilityType.Spell) ? settings.KnockbackMultiplier : 0;
            this.StunSeconds = (settings.AbilityType != AbilityType.Spell) ? settings.StunSeconds : 0;

            this.ProjectileName = (settings.AbilityType == AbilityType.Ranged) ? settings.ProjectileName : string.Empty;
            this.Velocity = (settings.AbilityType == AbilityType.Ranged) ? settings.Velocity : 0;

            this.Health = (settings.AbilityType == AbilityType.Spell) ? settings.Health : 0;
            this.Mana = (settings.AbilityType == AbilityType.Spell) ? settings.Mana : 0;
            this.Armor = (settings.AbilityType == AbilityType.Spell) ? settings.Armor : 0;
            this.Speed = (settings.AbilityType == AbilityType.Spell) ? settings.Speed : 0;
        }

        /// <summary>
        /// Constructor for null abilities
        /// </summary>
        /// <param name="abilityIndex"></param>
        public AbilityInstance(int abilityIndex)
        {
            this.Id = "None";
            this.DisplayName = "Empty Ability Slot";
            this.IsEnabled = false;
            this.Cooldown = null;
            this.AbilityIndex = abilityIndex;
        }

        public AbilityInstance(AbilitySettingsSO settings, int abilityIndex = 0, bool isEnabled = true)
        {
            InitWithSettings(settings);
            this.Cooldown = new Cooldown(-1, settings.CooldownSeconds);
            this.AbilityIndex = abilityIndex;
            this.IsEnabled = isEnabled;
        }

        public AbilityInstance(AbilitySettingsSO settings, AbilityData data)
        {
            InitWithSettings(settings);

            this.Cooldown = new Cooldown(Time.time + data.CurrentCooldown, settings.CooldownSeconds);
            this.AbilityIndex = data.AbilityIndex;
            this.IsEnabled = data.IsEnabled;
        }

        public void SetEnabled(bool isEnabled) { this.IsEnabled = isEnabled; }  
    }

    [Serializable]
    public class Cooldown
    {
        public float End;
        public float Duration;

        public Cooldown() { }
        public Cooldown(float end, float duration)
        {
            End = end;
            Duration = duration;
        }

        public void Begin()
        {
            End = Time.time + Duration;
        }
    }

    /// <summary>
    /// Allow selecting an ability from AbilityDatabase in inspector
    /// </summary>
    public interface IAbilitySelection
    {
        public string Id { get; }
        public int AbilityIndex { get; }
    }

    /// <summary>
    /// Struct to allow selecting an ability from AbilityDatabase in inspector
    /// </summary>
    [Serializable]
    [InlineProperty(LabelWidth = 100)]
    public struct AbilitySelectionPlayer : IAbilitySelection
    {
        [field: ValueDropdown("@GameData.AbilityDatabaseSO.GetPlayerAbilityNames()")]
        [field: SerializeField] public string Id { get; set; }
        [field: SerializeField] public int AbilityIndex { get; set; }
    }

    /// <summary>
    /// Struct to allow selecting an ability from AbilityDatabase in inspector
    /// </summary>
    [Serializable]
    [InlineProperty(LabelWidth = 100)]
    public struct AbilitySelectionMob : IAbilitySelection
    {
        [field: ValueDropdown("@GameData.AbilityDatabaseSO.GetMobAbilityNames()")]
        [field: SerializeField] public string Id { get; set; }
        [field: SerializeField] public int AbilityIndex { get; set; }
    }

    /// <summary>
    /// Struct to allow selecting an ability from AbilityDatabase in inspector
    /// </summary>
    [Serializable]
    [InlineProperty(LabelWidth = 100)]
    public struct AbilitySelectionTrainer : IAbilitySelection
    {
        [field: ValueDropdown("@GameData.AbilityDatabaseSO.GetTrainerAbilityNames()")]
        [field: SerializeField] public string Id { get; set; }
        [field: SerializeField] public int AbilityIndex { get; set; }
    }

    // TODO: MAKE a separate struct that only holds ID and index so cd is init? what happens if it isn't??? hide it in inspector?

    /// <summary>
    /// Class to hold save data
    /// </summary>
    [Serializable]
    public class AbilityData
    {
        public string Id;
        public bool IsEnabled;
        /// <summary>
        /// Cooldown time remaining
        /// </summary>
        public float CurrentCooldown;
        public int AbilityIndex;

        public AbilityData(AbilityInstance ability) 
        {
            this.Id = ability.Id;
            this.IsEnabled = ability.IsEnabled;
            this.CurrentCooldown = (ability.Cooldown.End <= Time.time) ? 0 : (ability.Cooldown.End - Time.time);
            this.AbilityIndex = ability.AbilityIndex;
        }
    }

    public struct DamageInstance
    {
        public float Damage;
        public float KnockbackMultiplier;
        public float StunDuration;

        public DamageInstance(float damage, float kb, float stun)
        {
            this.Damage = damage;
            this.KnockbackMultiplier = kb;
            this.StunDuration = stun;
        }
    }
}

