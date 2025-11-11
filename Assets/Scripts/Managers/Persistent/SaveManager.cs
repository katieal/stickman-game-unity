using Characters;
using Cysharp.Threading.Tasks;
using EventChannel;
using Items;
using Language.Lua;
using SaveData;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/*
 * Save Files consist of:
 *      - most recent save (hit continue on main menu) or maybe not?? idk
 *      - autosaves list
 *      - manual saves list
 * 
 * allow user to name saves?
 * 
 */

namespace Managers
{
    public class SaveManager : MonoBehaviour
    {
        [Title("DEBUG")]
        public bool ShouldMakeSaveFile = false;
        public bool ShouldLoadFromFile = false;

        [Title("Invoked Events")]
        [SerializeField] private SaveFileDataEventSO _loadSaveFileEvent;
        [SerializeField] private GameStateRequestEventSO _changeStateSO;
        [SerializeField] private VoidPlayerDataRequestEventSO _sendPlayerDataSO;
        [SerializeField] private VoidPlayerSaveDataUniTaskEventSO _savePlayerDataEvent;
        [SerializeField] private VoidStringListUniTaskEventSO _saveConditionKeysEvent;
        [SerializeField] private VoidSceneInfoUniTaskEventSO _saveSceneDataEvent;

        [Title("Listening to Events")]
        [SerializeField] private IntEventSO _loadAutoSaveEvent;
        [SerializeField] private IntEventSO _loadManualSaveEvent;
        [SerializeField] private SaveTypeVoidUniTaskEventSO _saveGameAutoEvent;
        [SerializeField] private IntSaveDisplayDataUniTaskEventSO _saveGameManualEvent;
        [SerializeField] private IntEventSO _deleteSaveManualEvent;

        [SerializeReference] private SaveFileData[] _autoSaves;
        [SerializeReference] private SaveFileData[] _manualSaves;

        // maybe a list of file names instead?

        /// <summary>
        /// store most recent save data for loading player
        /// </summary>
        private SaveFileData _currentSaveFile = null;
        private string _fileName = "TestSaveFile.txt";


        private void Awake()
        {
            _autoSaves = new SaveFileData[4];
            _manualSaves = new SaveFileData[4];

            // check for save file data
            if (ShouldLoadFromFile) LoadSaveFile();
        }

        private void OnEnable()
        {
            _loadAutoSaveEvent.OnInvokeEvent += OnLoadAutoSave;
            _loadManualSaveEvent.OnInvokeEvent += OnLoadManualSave;
            _saveGameAutoEvent.OnStartEvent += OnAutoSaveGame;
            _saveGameManualEvent.OnStartEvent += OnManualSaveGame;
            _deleteSaveManualEvent.OnInvokeEvent += OnDeleteManualSave;
            _sendPlayerDataSO.OnRequestEvent += LoadPlayer;
        }
        private void OnDisable()
        {
            _loadAutoSaveEvent.OnInvokeEvent -= OnLoadAutoSave;
            _loadManualSaveEvent.OnInvokeEvent -= OnLoadManualSave;
            _saveGameAutoEvent.OnStartEvent -= OnAutoSaveGame;
            _saveGameManualEvent.OnStartEvent -= OnManualSaveGame;
            _deleteSaveManualEvent.OnInvokeEvent -= OnDeleteManualSave;
            _sendPlayerDataSO.OnRequestEvent -= LoadPlayer;
        }

        [Button]
        public void CheckSaves()
        {
            Debug.Log($"autosaves: {_autoSaves.Length}");

            for (int i = 0; i < _autoSaves.Length; i++)
            {
                Debug.Log($"index: {i} | null: {(_autoSaves[i] == null)}");
            }

            Debug.Log($"manualsaves: {_manualSaves.Length}");

            for (int i = 0; i < _manualSaves.Length; i++)
            {
                Debug.Log($"index: {i} | null: {(_manualSaves[i] == null)}");
            }
        }

        [Button]
        public void ClearSaveFiles()
        {
            _autoSaves = new SaveFileData[4];
            _manualSaves = new SaveFileData[4];
        }

        [Button]
        public void LoadFile()
        {
            LoadSaveFile();
        }

        #region Event Callbacks
        private void OnAutoSaveGame(SaveType type) { OnAutoSaveGameAsync(type).Forget(); }
        private void OnManualSaveGame(int index) { OnManualSaveGameAsync(index).Forget(); }
        private void OnDeleteManualSave(int index)
        {
            _manualSaves[index] = null;
        }
        #endregion

        #region File Read/Write

        private void LoadSaveFile()
        {
            // check for save file data
            if (FileManager.CheckForFile(_fileName))
            {
                FileManager.LoadFromFile(_fileName, out string saveFile);
                SaveFile file = JsonUtility.FromJson<SaveFile>(saveFile);

                _autoSaves = file.AutoSaves;
                _manualSaves = file.ManualSaves;
            }
        }

        private void WriteSaveFile()
        {
            #region Debug
            if (!ShouldMakeSaveFile) { return; }
            #endregion

            string jsonString;

            SaveFile file = new SaveFile()
            {
                AutoSaves = _autoSaves,
                ManualSaves = _manualSaves,
            };

            jsonString = JsonUtility.ToJson(file, true);
            FileManager.WriteToFile(_fileName, jsonString);
        }

        #endregion

        #region Display Data
        public void GetSaveDisplayData(out SaveSlotDisplayData[] autoSaves, out SaveSlotDisplayData[] mainSaves)
        {
            autoSaves = GetDisplayData(_autoSaves);
            mainSaves = GetDisplayData(_manualSaves);
        }
        private SaveSlotDisplayData[] GetDisplayData(SaveFileData[] saveData)
        {
            SaveSlotDisplayData[] displayData = new SaveSlotDisplayData[saveData.Length];

            for (int index = 0; index < saveData.Length; index++)
            {
                displayData[index] = GetDisplayData(saveData[index]);
            }
            return displayData;
        }
        private SaveSlotDisplayData GetDisplayData(SaveFileData saveData)
        {
            SaveSlotDisplayData displayData = new SaveSlotDisplayData();

            if (saveData == null)
            {
                displayData.IsEmpty = true;
            }
            else
            {
                displayData.IsEmpty = false;
                displayData.Day = saveData.GameData.CurrentDay;
                displayData.Time = Managers.TimeManager.GetTimeString(saveData.GameData.CurrentGameTime);
                displayData.Completion = saveData.GameData.CompletionPercent;
                displayData.Location = GameData.SceneDatabase.Instance.GetDisplayName(saveData.GameData.CurrentScene);

                //displayData.Day = 1;
                //displayData.Time = "10:00";
                //displayData.Completion = 12.34f;
                //displayData.Location = "Cool Place";
            }
            return displayData;
        }
        #endregion

        #region Load Game
        private void OnLoadAutoSave(int index)
        {
            _changeStateSO.RequestEvent(GameStateMachine.StateType.Start);
            // also set current file here
            _currentSaveFile = _autoSaves[index];
            _loadSaveFileEvent.InvokeEvent(_autoSaves[index]);
            
            // clear autosaves?
        }
        private void OnLoadManualSave(int index)
        {
            _changeStateSO.RequestEvent(GameStateMachine.StateType.Start);

            _currentSaveFile = _manualSaves[index];
            _loadSaveFileEvent.InvokeEvent(_manualSaves[index]);

            // clear autosaves?
        }
        private void LoadPlayer()
        {
            // player components will subscribe to this event and get the data they need
            // to initialize when it is invoked
            if (_currentSaveFile != null) { _sendPlayerDataSO.OnResultEvent(_currentSaveFile.PlayerSaveData); }
            else { _sendPlayerDataSO.OnResultEvent(null); }
        }
        #endregion

        #region Save Game
        private async UniTaskVoid OnAutoSaveGameAsync(SaveType type)
        {
            // reminder: methods are synchronous by default!!

            // save current file
            _currentSaveFile = await GenerateSaveFileAsync(type, (int)type);

            // save autosave in appropriate index
            if (type == SaveType.SceneChange)
            {
                _autoSaves[0] = _currentSaveFile;
            }
            else if (type == SaveType.Event)
            {
                _autoSaves[1] = _currentSaveFile;
            }
            else if (type == SaveType.DayStart)
            {
                // if there is already a save at CurrentDay slot
                if (_autoSaves[2] != null)
                {
                    // move current day to prev day 
                    _autoSaves[3] = _autoSaves[2];
                    _autoSaves[3].Index = 3;
                }
                // save current day
                _autoSaves[2] = _currentSaveFile;
            }

            // write autosave data to file
            WriteSaveFile();

            // broadcast saving is finished
            _saveGameAutoEvent.EndEvent();
        }

        private async UniTaskVoid OnManualSaveGameAsync(int index)
        {
            // update current save file
            _currentSaveFile = await GenerateSaveFileAsync(SaveType.Manual, index);
            // save current file
            _manualSaves[index] = _currentSaveFile;

            // write manual save data to file
            WriteSaveFile();

            // broadcast results
            _saveGameManualEvent.EndEvent(GetDisplayData(_currentSaveFile));
        }

        private async UniTask<SaveFileData> GenerateSaveFileAsync(SaveType type, int index)
        {
            // save player and game data
            var (playerData, gameData) = await UniTask.WhenAll(
                SavePlayerData(),
                SaveGameStateAsync());

            SaveFileData saveFile = new SaveFileData()
            {
                Type = type,
                Index = index,
                PlayerSaveData = playerData,
                PlayerStaticData = SavePlayerStaticData(),
                GameData = gameData
            };
            return saveFile;
        }

        private UniTask<PlayerSaveData> SavePlayerData()
        {
            return _savePlayerDataEvent.InvokeEventAsync();
        }

        private PlayerStaticData SavePlayerStaticData()
        {
            PlayerStaticData staticData = new PlayerStaticData()
            {
                PlayerMoney = Player.Wallet.Instance.Balance,
                PlayerDebt = Player.Wallet.Instance.Debt,
                Inventory = Player.Inventory.Instance.GetItemData(),
                MaxInventorySize = Player.Inventory.Instance.MaxSize,
            };

            return staticData;
        }

        private async UniTask<GameStateData> SaveGameStateAsync()
        {
            // save scene data and condition keys
            var (sceneInfo, keys) = await UniTask.WhenAll(_saveSceneDataEvent.InvokeEventAsync(), 
                _saveConditionKeysEvent.InvokeEventAsync());

            GameStateData gameData = new GameStateData()
            {
                SceneInfo = sceneInfo,
                CurrentScene = SceneManager.GetActiveScene().name,
                CurrentDay = Managers.DayManager.Instance.CurrentDay,
                CurrentGameTime = Managers.TimeManager.Instance.CurrentTime,
                TriggeredConditionKeys = keys,
                CompletionPercent = UnityEngine.Random.Range(0f, 100f)
            };
            
            return gameData;
        }
        #endregion
    }
}

namespace SaveData 
{
    /// <summary>
    /// Data sent to UI to be shown to user
    /// </summary>
    public struct SaveSlotDisplayData
    {
        public Sprite Image;
        public bool IsEmpty;
        public int Day;
        public string Time;
        public float Completion;
        public string Location;
    }

    /// <summary>
    /// Manual - manual saves made by user (will always be index 0)
    /// DayStart - autosave at start of each day
    /// SceneChange - autosave upon entering a new scene
    /// Event - autosave after completing some big event
    /// 
    /// Autosave slots will consist of (in this order)
    ///     - Manual Save Entry (constantly updated on every save call)
    ///         - Most recent SceneChange or Event
    ///         - Previous day
    ///         - Previous day - 1
    ///         - Previous day - 2
    /// </summary>
    public enum SaveType { Manual = -1, SceneChange, Event, DayStart }

    [Serializable]
    public struct SaveFile
    {
        [SerializeReference] public SaveFileData[] AutoSaves;
        [SerializeReference] public SaveFileData[] ManualSaves;
    }

    [Serializable]
    public class SaveFileData
    {
        public SaveType Type;
        /// <summary>
        /// saving index so I can load them in the same order
        /// </summary>
        public int Index;
        public GameStateData GameData;
        public PlayerStaticData PlayerStaticData;
        public PlayerSaveData PlayerSaveData;
    }

    /// <summary>
    /// SpawnManager (and the rest) keeps track of its own data for each session, so this will
    /// only be needed on first load
    /// </summary>
    [Serializable]
    public struct GameStateData
    {
        public Scenes.SceneInfo SceneInfo;
        public string CurrentScene;
        public int CurrentDay;
        public int CurrentGameTime;
        public List<string> TriggeredConditionKeys;
        public float CompletionPercent;
    }

    /// <summary>
    /// Same as GameData, will only need to be broadcast on first load
    /// </summary>
    [Serializable]
    public struct PlayerStaticData
    {
        public int PlayerMoney;
        public int PlayerDebt;
        public List<ItemData> Inventory;
        public int MaxInventorySize;
    }

    /// <summary>
    /// Needs to be broadcast every time player is spawned in a new scene
    /// </summary>
    [Serializable]
    public class PlayerSaveData 
    {
        public float CurrentHealth;
        public float MaxHealth;
        public float CurrentMana;
        public float MaxMana;
        public float MoveSpeed;
        public Items.WeaponType HeldWeapon;
        public int SelectedAbility;
        public AbilitySaveData Abilities;
        public Characters.StateMachine.StateType CurrentState;
        public Characters.StateMachine.StateType PreviousState;
    }
    
    [Serializable]
    public class AbilitySaveData
    {
        // unity doesn't serialize nested lists/arrays
        public Combat.AbilityData[] Sword;
        public Combat.AbilityData[] Bow;
        public Combat.AbilityData[] Staff;

        public Combat.AbilityData[] GetData(WeaponType type)
        {
            if (type == WeaponType.Sword) { return Sword; }
            else if (type == WeaponType.Bow) { return Bow; }
            else if (type == WeaponType.Staff) { return Staff; }
            return null;
        }
    }
}
