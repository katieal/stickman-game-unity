using System;
using UnityEngine;

namespace Items
{
    public enum PropertyType { Equipment, Weapon, Material, Sellable, Stackable, Breakable, Potion, MobDrop, SkillBook, Usable, Effect }
    public enum WeaponType { Unarmed = -1, Sword = 0, Bow = 1, Staff = 2 }
    public enum MaterialType { Usable, MobDrop, Quest, Misc }
    public enum UsableType { Potion, Book, Effect } // NOT SURE IF THIS IS STAYING!!

    namespace Properties
    {
        public interface IItemProperty
        { 
            public PropertyType Type { get; }
        }

        [Serializable]
        public class Sellable : IItemProperty
        {
            public PropertyType Type { get { return PropertyType.Sellable; } }
            public int BuyPrice;
            public int SellPrice;
        }

        [Serializable]
        public class Stackable : IItemProperty
        {
            public PropertyType Type { get { return PropertyType.Stackable; } }
            public int StackSize;
        }

        [Serializable]
        public class MobDrop : IItemProperty
        {
            public PropertyType Type { get { return PropertyType.MobDrop; } }
        }

        #region Equipment Components
        [Serializable]
        public class Equipment : IItemProperty
        {
            public PropertyType Type { get { return PropertyType.Equipment; } }
            //public int MaxDurability;
        }

        [Serializable]
        public class Durability : IItemProperty
        {
            public PropertyType Type { get { return PropertyType.Breakable; } }
            public int MaxDurability;
        }

        [Serializable]
        public class Weapon : IItemProperty
        {
            public PropertyType Type { get { return PropertyType.Weapon; } }
            public WeaponType WeaponType;
            public float Damage;
            public float MagicDamage;
            public float Block;
        }
        #endregion


        #region Material Components
        [Serializable]
        public class Material : IItemProperty
        {
            public PropertyType Type { get { return PropertyType.Material; } }
            public MaterialType MaterialType;
        }

        [Serializable]
        public class Usable : IItemProperty
        {
            public PropertyType Type { get { return PropertyType.Usable; } }
            public UsableType UsableType;
        }

        [Serializable]
        public class Potion : IItemProperty
        {
            public PropertyType Type { get { return PropertyType.Potion; } }
            [field: SerializeField] public float HealthValue { get; private set; }
            [field: SerializeField] public float ManaValue { get; private set; }
            [field: SerializeField] public float SpeedValue { get; private set; }
        }

        [Serializable]
        public class SkillBook : IItemProperty
        {
            public PropertyType Type { get { return PropertyType.SkillBook; } }
            public string Skill;
        }

        [Serializable]
        public class Effect : IItemProperty
        {
            public PropertyType Type { get { return PropertyType.Effect; } }
            public float Duration;
            public float Health;
            public float Mana;
            public float Speed;
        }
        #endregion
    }
}