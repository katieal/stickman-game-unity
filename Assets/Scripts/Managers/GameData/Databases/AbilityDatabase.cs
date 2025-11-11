using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using Combat;

namespace GameData
{
    public class AbilityDatabase : SerializedMonoBehaviour
    {
        // TODO: this class also needs to store info on if each ability has been learned
        [SerializeField] private AbilityDatabaseSO _abilityDatabase;

        #region Singleton
        // singleton reference
        private static AbilityDatabase _instance;
        public static AbilityDatabase Instance { get { return _instance; } }

        private void Awake()
        {
            // singleton pattern
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }
        }
        #endregion

        /// <summary>
        /// Method used by all characters to get an instance of their combat abilities
        /// </summary>
        /// <param name="type">Type of character the ability belongs to</param>
        /// <param name="selection"></param>
        /// <returns></returns>
        public AbilityInstance GetAbility(IAbilitySelection selection)
        {
            AbilitySettingsSO ability = FindAbility(selection.Id);

            if (ability != null) { return new AbilityInstance(ability); }

            return null;
        }

        /// <summary>
        /// Player Only
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public AbilityInstance GetAbility(AbilityData data)
        {
            AbilitySettingsSO ability = FindAbility(data.Id);

            if (ability != null) { return new AbilityInstance(ability, data); }

            return null;
        }

        private AbilitySettingsSO FindAbility(string id)
        {
            if (FindPlayerAbility(id, out AbilitySettingsSO playerAbility)) { return playerAbility; }
            else if (FindMobAbility(id, out AbilitySettingsSO mobAbility)) { return mobAbility; }
            else if (FindTrainerAbility(id, out AbilitySettingsSO trainerAbility)) { return trainerAbility; }

            return null;
        }

        private bool FindPlayerAbility(string id, out AbilitySettingsSO ability)
        {
            ability = _abilityDatabase.PlayerAbilities.Find(x => x.name.Equals(id));
            return ability != null;
        }

        private bool FindMobAbility(string id, out AbilitySettingsSO ability)
        {
            ability = _abilityDatabase.MobAbilities.Find(x => x.name.Equals(id));
            return ability != null;
        }

        private bool FindTrainerAbility(string id, out AbilitySettingsSO ability)
        {
            ability = _abilityDatabase.TrainerAbilities.Find(x => x.name.Equals(id));
            return ability != null;
        }
    }
}