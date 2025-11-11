using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using Characters;

namespace GameData
{

    public class SpawnableData : SerializedMonoBehaviour
    {
        [Title("Player")]
        public GameObject PlayerPrefab;

        [Title("Item Drop")]
        public GameObject ItemDropPrefab;

        [Title("Mobs")]
        [DictionaryDrawerSettings(KeyLabel = "Enemy Type", ValueLabel = "Prefab")]
        public Dictionary<MobType, GameObject> EnemyPrefabs = new Dictionary<MobType, GameObject>()
        {
            { MobType.Slime, null},
            { MobType.Boar, null},
            { MobType.Beetle, null},
            { MobType.Specter, null},
            { MobType.Wyvern, null},
            { MobType.Bear, null}
        };

        [Title("Projectiles")]
        [DictionaryDrawerSettings(KeyLabel = "Name", ValueLabel = "Prefab")]
        public Dictionary<string, GameObject> Projectiles = new Dictionary<string, GameObject>();


        #region Singleton
        // singleton reference
        private static SpawnableData _instance;
        public static SpawnableData Instance { get { return _instance; } }

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

        public GameObject GetProjectile(string name)
        {
            Debug.Assert(Projectiles.ContainsKey(name), $"Projectile {name} not found.");

            return Projectiles[name];
        }
    }
}

