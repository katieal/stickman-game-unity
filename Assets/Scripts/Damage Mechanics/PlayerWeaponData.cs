using System.Collections.Generic;
using System;
using UnityEngine;
using Items;
using Sirenix.OdinInspector;

namespace Combat
{
    /// <summary>
    /// Holds data for player's weapons
    /// </summary>
    [Serializable]
    public class WeaponInstance
    {
        /// <summary>
        /// Is there a weapon of this type in the player's weapon belt?
        /// </summary>
        [ShowInInspector] public bool IsEquipped { get; set; }

        /// <summary>
        /// Is the equipped weapon broken?
        /// </summary>
        [ShowInInspector] public bool IsEnabled { get; set; }

        public AbilityInstance[] Abilities { get; private set; } = new AbilityInstance[4];

        #region Save/Load

        // default abilities taken from player settings
        public WeaponInstance(WeaponType type, AbilitySelectionPlayer[] selections)
        {
            for (int i = 0; i < selections.Length; i++)
            {
                // if there is no ability in the specified slot
                if (selections[i].Id == "None") 
                {
                    Abilities[i] = new AbilityInstance(i); 
                }
                else { Abilities[i] = GameData.AbilityDatabase.Instance.GetAbility(selections[i]); }
            }

            IsEquipped = Player.Inventory.Instance.CheckEquippedWeapon(type);
            IsEnabled = Player.Inventory.Instance.CheckEnabledWeapon(type);
        }

        // ability data loaded from save file
        public WeaponInstance(WeaponType type, AbilityData[] data)
        {
            for (int i = 0;  i < data.Length; i++)
            {
                // if there is no ability in the specified slot
                if (data[i] == null)
                {
                    // create new blank ability
                    Abilities[i] = new AbilityInstance(i);
                }
                else { Abilities[i] = GameData.AbilityDatabase.Instance.GetAbility(data[i]); }
            }

            IsEquipped = Player.Inventory.Instance.CheckEquippedWeapon(type);
            IsEnabled = Player.Inventory.Instance.CheckEnabledWeapon(type);
        }

        // called to save current ability data
        public AbilityData[] GetAbilityData()
        {
            AbilityData[] data = new AbilityData[4];
            for (int i = 0; i < Abilities.Length; i++)
            {
                if (Abilities[i].Id == "None") { data[i] = null; }
                else { data[i] = new AbilityData(Abilities[i]); }
            }
            return data;
        }
        #endregion

        public void StartCooldown(int abilityIndex)
        {
            // starts cooldown for selected ability
            Abilities[abilityIndex].Cooldown.Begin();
        }

        public bool CheckCooldown(int index)
        {
            return Abilities[index].Cooldown.End <= Time.time;
        }

        public Cooldown[] GetCooldowns()
        {
            return new Cooldown[4]
            {
                GetCooldown(0),
                GetCooldown(1),
                GetCooldown(2),
                GetCooldown(3)
            };
        }

        private Cooldown GetCooldown(int index)
        {
            return Abilities[index].IsEnabled ? 
                new Cooldown(Abilities[index].Cooldown.End, Abilities[index].Cooldown.Duration) : null;
        }
    }
}
