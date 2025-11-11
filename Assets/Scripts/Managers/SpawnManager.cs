using Characters;
using Cysharp.Threading.Tasks;
using EventChannel;
using Items;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Splines.SplineInstantiate;

namespace Scenes
{

    public class SpawnManager : SerializedMonoBehaviour
    {
        [TitleGroup("Script Reference")]
        [SerializeField] private GameData.SpawnableData _spawnables;

        [TitleGroup("Events")]
        [TitleGroup("Events/Listening")]
        [SerializeField] private GameStateDataEventSO _loadGameDataEvent;
        [SerializeField] private VoidSceneInfoUniTaskEventSO _saveSceneDataEvent;
        //[SerializeField] private SceneNameVoidUniTaskEventSO _movePlayerSceneEvent;
        [SerializeField] private LocationVoidUniTaskEventSO _spawnSceneObjectsEvent;
        [SerializeField] private ItemVector3EventSO _spawnItemSO;

        /// <summary>
        /// Holds data about spawned mobs in each scene. 
        /// Scene index = scene build index
        /// NOTE: THIS DATA SHOULD GET WIPED AT THE END OF EVERY IN GAME DAY
        /// </summary>
        private SceneObjectInfo[] _sceneObjects;

        private GameObject _spawnedPlayer;
        private Location _playerLocation;


        // x coordinate of item drop spawn can be position +- this value
        private float _xvariance = 0.2f;

        private void OnEnable()
        {
            _loadGameDataEvent.OnInvokeEvent += OnLoadGameData;
            _saveSceneDataEvent.OnStartEvent += OnSaveSceneData;
            //_movePlayerSceneEvent.OnStartEvent += OnMovePlayer;
            _spawnSceneObjectsEvent.OnStartEvent += SpawnSceneObjects;
            _spawnItemSO.OnInvokeEvent += OnSpawnItem;
        }

        private void OnDisable()
        {
            _loadGameDataEvent.OnInvokeEvent -= OnLoadGameData;
            _saveSceneDataEvent.OnStartEvent -= OnSaveSceneData;
            //_movePlayerSceneEvent.OnStartEvent -= OnMovePlayer;
            _spawnSceneObjectsEvent.OnStartEvent += SpawnSceneObjects;
            _spawnItemSO.OnInvokeEvent -= OnSpawnItem;
        }

        private void Start()
        {
            _sceneObjects = new SceneObjectInfo[SceneManager.sceneCountInBuildSettings];

            // init scene object database
            for (int i = 0; i < _sceneObjects.Length; i++)
            {
                _sceneObjects[i] = new SceneObjectInfo(null, null);
            }
        }

        #region Debug
        public void SpawnMob(MobType type, int spawnIndex)
        {
            // temp vars for readability
            SpawnPoint[] spawnPoints = SceneRefManager.Instance.MobSpawns;
            GameObject mobPrefab = _spawnables.EnemyPrefabs[type];
            int buildIndex = SceneManager.GetActiveScene().buildIndex;

            // init spawned mob data if needed
            if (_sceneObjects[buildIndex].MobSpawns == null)
            {
                _sceneObjects[buildIndex].MobSpawns = new SpawnedMob[spawnPoints.Length];
            }

            // spawn mob at that index
            GameObject mob = Instantiate(mobPrefab, spawnPoints[spawnIndex].Spawn.position, Quaternion.identity);
            // set patrol with true index of spawn point
            mob.GetComponentInChildren<Characters.StateMachine.MobStateMachine>().SetPatrol(spawnIndex);

            // update save data
            _sceneObjects[buildIndex].MobSpawns[spawnIndex] = new SpawnedMob() { Type = type };
        }

        #endregion


        #region Save/Load Data
        private void OnSaveSceneData()
        {
            // update current player location
            _playerLocation = SceneRefManager.Instance.GetClosestLocation(_spawnedPlayer.transform.position);

            SceneInfo info = new SceneInfo()
            {
                PlayerLocation = _playerLocation,
                SceneObjects = new SceneObjectInfo[_sceneObjects.Length]
            };

            // iterate through data for each scene and make value copies of arrays
            for (int i = 0; i < _sceneObjects.Length; i++)
            {
                // make null if stored value is null, otherwise make a copy
                info.SceneObjects[i] = new SceneObjectInfo()
                {
                    MobSpawns = CopyArray<SpawnedMob>(_sceneObjects[i].MobSpawns),
                    NpcSpawns = CopyArray(_sceneObjects[i].NpcSpawns)
                };
            }

            // end save event
            _saveSceneDataEvent.EndEvent(info);
        }

        private void OnLoadGameData(SaveData.GameStateData data)
        {
            // make value copy of save data
            _sceneObjects = new SceneObjectInfo[data.SceneInfo.SceneObjects.Length];

            // iterate through data for each scene and make value copies of arrays
            for (int i = 0; i < data.SceneInfo.SceneObjects.Length; i++)
            {
                // make null if stored value is null, otherwise make a copy
                _sceneObjects[i] = new SceneObjectInfo()
                {
                    MobSpawns = CopyArray(data.SceneInfo.SceneObjects[i].MobSpawns),
                    NpcSpawns = CopyArray(data.SceneInfo.SceneObjects[i].NpcSpawns)
                };
            }
        }

        /// <summary>
        /// Make a value copy of an array.
        /// Can accept and return null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        private T[] CopyArray<T>(T[] data)
        {
            T[] copy;

            if (data == null) { copy = null; }
            else
            {
                copy = new T[data.Length];
                System.Array.Copy(data, copy, data.Length);
            }
            return copy;
        }
        #endregion

        #region Event Callbacks
        private void OnSpawnItem(Items.ItemInstance item, Vector3 position)
        {
            Vector3 posvariance = new Vector3(UnityEngine.Random.Range(-_xvariance, _xvariance), 0, 0);
            GameObject drop = Instantiate(_spawnables.ItemDropPrefab, position + posvariance, Quaternion.identity);
            drop.GetComponent<ItemPickup>().Init(item);
        }
        #endregion

        #region Spawning
        private void SpawnSceneObjects(Location playerLocation)
        {
            // get currently active sceneinfo
            Scene currentScene = SceneManager.GetActiveScene();
            Enum.TryParse<SceneName>(currentScene.name, true, out var sceneName);
            SceneSettingsSO settings = GameData.SceneDatabase.Instance.GetSceneSettings(currentScene.name);

            #region Debug
            Debug.Assert(currentScene.buildIndex == settings.BuildIndex, $"Scene {sceneName} settings build index mismatch!");
            #endregion

            // delete old player reference
            _spawnedPlayer = null;

            // spawn player in scene
            if (playerLocation != Location.Null) { SpawnPlayer(playerLocation); }
            _playerLocation = playerLocation;

            //SpawnNPCs(scene);

            SpawnEnemies(currentScene.buildIndex, settings);

            _spawnSceneObjectsEvent.EndEvent();
        }

        // public for debug
        public void SpawnPlayer(Location location)
        {
            // get transform of spawn point
            Transform pos = SceneRefManager.Instance.GetTransform(location);
            // spawn player in specified location
            _spawnedPlayer = Instantiate(_spawnables.PlayerPrefab, pos.position, Quaternion.identity);

            // set camera to track player
            FindFirstObjectByType<CinemachineCamera>().Target.TrackingTarget = _spawnedPlayer.transform;
        }

        private void SpawnNPCs(SceneSettingsSO scene)
        {

        }

        private void SpawnEnemies(int sceneIndex, SceneSettingsSO settings)
        {
            // if there is valid save data, use it to spawn mobs
            if (_sceneObjects[sceneIndex].MobSpawns != null)
            {
                Debug.Log("spawning saved mobs");
                // will trigger if player visits the same scene more than once a day
                // or if loading a previous save
                SpawnSavedMobs(sceneIndex);
            }
            // otherwise, if there is no existing save data, spawn default enemies
            else
            {
                // return if scene has no mobs
                if (settings.MobToSpawn.Type == MobType.None) { return; }

                Debug.Log("spawning default mobs");
                // spawn according to scene settings
                SpawnDefaultMobs(settings);
            }
        }

        /// <summary>
        /// Spawn Mobs in scene based on data in _sceneObjects
        /// </summary>
        /// <param name="sceneIndex"></param>
        private void SpawnSavedMobs(int sceneIndex)
        {
            // temp vars for readability
            SpawnPoint[] spawnPoints = SceneRefManager.Instance.MobSpawns;
            SpawnedMob[] mobSpawns = _sceneObjects[sceneIndex].MobSpawns;

            // spawn mobs according to saved data
            for (int i = 0; i < mobSpawns.Length; i++)
            {
                // check if this spawn point has a mob
                if (mobSpawns[i].Type != MobType.None)
                {
                    // spawn mob at spawn point i
                    GameObject mobPrefab = _spawnables.EnemyPrefabs[mobSpawns[i].Type];
                    GameObject mob = Instantiate(mobPrefab, spawnPoints[i].Spawn.position, Quaternion.identity);
                    mob.GetComponentInChildren<Characters.StateMachine.MobStateMachine>().SetPatrol(i);
                }
            }
        }

        /// <summary>
        /// Spawn default mobs listed in scene settings
        /// </summary>
        /// <param name="settings"></param>
        private void SpawnDefaultMobs(SceneSettingsSO settings)
        {
            #region Debug
            // make sure there are enough spawn points for number of mobs
            Debug.Assert(settings.MobToSpawn.MaxSpawnCount <= SceneRefManager.Instance.MobSpawns.Length, "Not enough spawn points for all mobs!");
            #endregion
            // create array with the indexes of all possible spawn points and shuffle it
            // then take the first x number of values from that array
            // and spawn a mob at that index

            // temp vars for readability
            SpawnPoint[] spawnPoints = SceneRefManager.Instance.MobSpawns;
            int[] indexes = Enumerable.Range(0, spawnPoints.Length).ToArray();
            EnemySpawn spawnData = settings.MobToSpawn;
            GameObject mobPrefab = _spawnables.EnemyPrefabs[spawnData.Type];

            // init spawned mob data
            _sceneObjects[settings.BuildIndex].MobSpawns = new SpawnedMob[spawnPoints.Length];

            // Fisher-Yates shuffle indexes array
            for (int i = 0; i < indexes.Length - 1; i++)
            {
                int r = UnityEngine.Random.Range(i, indexes.Length);
                // swap values with tuple
                (indexes[r], indexes[i]) = (indexes[i], indexes[r]);
            }

            // determine number to spawn - pick random number between min and max
            int numToSpawn = UnityEngine.Random.Range(spawnData.MinSpawnCount, spawnData.MaxSpawnCount + 1);

            // spawn mobs at the first numToSpawn number of indexes
            for (int i = 0; i < indexes.Length; i++)
            {
                // i = order of the indexes
                // value of indexes[i] = the index of the spawn point being evaluated
                int pointIndex = indexes[i];

                // if mob should be spawned
                if (i < numToSpawn)
                {
                    // spawn mob at that index
                    GameObject mob = Instantiate(mobPrefab, spawnPoints[pointIndex].Spawn.position, Quaternion.identity);
                    // set patrol with true index of spawn point
                    mob.GetComponentInChildren<Characters.StateMachine.MobStateMachine>().SetPatrol(pointIndex);

                    // update save data
                    _sceneObjects[settings.BuildIndex].MobSpawns[pointIndex] = new SpawnedMob() { Type = spawnData.Type };
                }
                else
                {
                    // set to none if no mob is spawned at this point
                    _sceneObjects[settings.BuildIndex].MobSpawns[pointIndex] = new SpawnedMob() { Type = MobType.None };
                }

            }
        }
        #endregion
    }

    /// <summary>
    /// Struct to hold scene object info save data
    /// </summary>
    [Serializable]
    public struct SceneInfo
    {
        public Location PlayerLocation;
        public SceneObjectInfo[] SceneObjects;
    }

    /// <summary>
    /// Holds info on a scene's spawned mobs and npcs
    /// </summary>
    [Serializable]
    public struct SceneObjectInfo 
    {
        public SpawnedMob[] MobSpawns;
        public bool[] NpcSpawns;

        public SceneObjectInfo(SpawnedMob[] mobSpawns, bool[] npcSpawns)
        {
            MobSpawns = mobSpawns;
            NpcSpawns = npcSpawns;
        }
    }

    /// <summary>
    /// Data on a mob spawned in a scene
    /// </summary>
    [Serializable]
    public struct SpawnedMob
    {
        public Characters.MobType Type;
        // insert other info like modifiers or whatever here
    }

    public struct SpawnedNpcData
    {

    }
}
