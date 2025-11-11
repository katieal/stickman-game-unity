using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameData
{
    [CreateAssetMenu(fileName = "AbilityDatabaseSO", menuName = "Scriptable Objects/Ability Database")]
    public class AbilityDatabaseSO : SerializedScriptableObject
    {
        [Title("Abilities")]
        [AssetSelector(Paths = "Assets/Scriptable Objects/Combat Abilities/Player Abilities")]
        public List<Combat.AbilitySettingsSO> PlayerAbilities = new List<Combat.AbilitySettingsSO>();
        [AssetSelector(Paths = "Assets/Scriptable Objects/Combat Abilities/Enemy Abilities")]
        public List<Combat.AbilitySettingsSO> MobAbilities = new List<Combat.AbilitySettingsSO>();
        [AssetSelector(Paths = "Assets/Scriptable Objects/Combat Abilities")]
        public List<Combat.AbilitySettingsSO> TrainerAbilities = new List<Combat.AbilitySettingsSO>();

        private static List<Combat.AbilitySettingsSO> _staticPlayerList;
        private static List<Combat.AbilitySettingsSO> _staticMobList;
        private static List<Combat.AbilitySettingsSO> _staticTrainerList;

        private void OnEnable()
        {
            _staticPlayerList = PlayerAbilities;
            _staticMobList = MobAbilities;
            _staticTrainerList = TrainerAbilities;
        }

        public static IEnumerable GetAbilityNames()
        {
            List<string> names = new List<string>();
            names.Add("None");
            foreach(var ability in _staticPlayerList)
            {
                names.Add(ability.name);
            }
            foreach (var ability in _staticMobList)
            {
                names.Add(ability.name);
            }
            foreach (var ability in _staticTrainerList)
            {
                names.Add(ability.name);
            }

            return names;
        }

        public static IEnumerable GetPlayerAbilityNames()
        {
            List<string> names = new List<string>();
            names.Add("None");
            foreach (var ability in _staticPlayerList)
            {
                names.Add(ability.name);
            }
            return names;
        }

        public static IEnumerable GetMobAbilityNames()
        {
            List<string> names = new List<string>();
            names.Add("None");
            foreach (var ability in _staticMobList)
            {
                names.Add(ability.name);
            }

            return names;
        }

        public static IEnumerable GetTrainerAbilityNames()
        {
            List<string> names = new List<string>();
            names.Add("None");
            foreach (var ability in _staticTrainerList)
            {
                names.Add(ability.name);
            }

            return names;
        }
    }
}