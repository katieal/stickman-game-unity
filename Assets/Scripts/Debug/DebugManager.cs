using Characters;
using Cysharp.Threading.Tasks;
using EventChannel;
using Items;
using PixelCrushers.DialogueSystem;
using Player;
using SettingsUI;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using SaveData;
using UnityEngine.EventSystems;
using Scenes;

namespace Testing
{
    public class DebugManager : SerializedMonoBehaviour
    {


        //[TitleGroup("Temp Testing", Order = -1)]
        //public int Holder;


        //[ButtonGroup("Temp Testing/Button")]



        #region Permanent Functions

        [TitleGroup("Add Items", Order = 3)]
        [TitleGroup("Add Items")][ListDrawerSettings(CustomAddFunction = nameof(AddItemEntry))] public List<ItemEntry> StarterItems;
        [TitleGroup("Add Items")][ListDrawerSettings(CustomAddFunction = nameof(AddItemEntry))] public List<ItemEntry> RuntimeItems;
        [TitleGroup("Add Items")][SerializeField] private ItemEventSO _addItemSO;

        [TitleGroup("Start Game Fields", Order = 4)]
        [SerializeField] private VoidEventSO _startGameEvent;

        [TitleGroup("Game Function Fields", Order = 5)]
        [BoxGroup("Game Function Fields/Spawning")] public MobType SpawnType;
        [BoxGroup("Game Function Fields/Spawning")] public int SpawnPointIndex;
        [BoxGroup("Game Function Fields/Spawning")]
        [SerializeField] private Scenes.SpawnManager _spawnManager;
        [BoxGroup("Game Function Fields/Notifications")]
        [ValueDropdown("@NotificationDataSO.GetKeys()")][SerializeField] private string _notificationKey;
        [BoxGroup("Game Function Fields/Notifications")] public StringBoolEventSO _notificationSO;

        [TitleGroup("Player Fuction Fields", Order = 6)]
        [BoxGroup("Player Fuction Fields/Damage Player")]
        [SerializeField] private CharacterHitter _charHitter;
        [BoxGroup("Player Fuction Fields/Add Money")]
        [SerializeField] private IntIntRequestEventSO _changeMoneySO;

        #region Inspector
        public ItemEntry AddItemEntry()
        {
            return new ItemEntry() { Quantity = 1 };
        }
        #endregion

        #region Start Game
        [TitleGroup("Start Game", Order = 0)]
        [ButtonGroup("Start Game/Buttons")]
        public void StartWithPlayer()
        {
            // all scenes at build index < 4 are not gameplay scenes
            if (SceneManager.GetActiveScene().buildIndex < 4)
            {
                int count = SceneManager.sceneCount;
                for (int i = 0; i < count; i++)
                {
                    if (SceneManager.GetSceneAt(i).buildIndex >= 4)
                    {
                        // set active scene to a gameplay scene before spawning player
                        SceneManager.SetActiveScene(SceneManager.GetSceneAt(i));
                        break;
                    }
                }
            }
            _startGameEvent.InvokeEvent();
        }

        [ButtonGroup("Start Game/Buttons")]
        public void StartWithoutPlayer()
        {
            // all scenes at build index < 4 are not gameplay scenes
            if (SceneManager.GetActiveScene().buildIndex < 4)
            {
                int count = SceneManager.sceneCount;
                for (int i = 0; i < count; i++)
                {
                    if (SceneManager.GetSceneAt(i).buildIndex >= 4)
                    {
                        // set active scene to a gameplay scene before spawning player
                        SceneManager.SetActiveScene(SceneManager.GetSceneAt(i));
                        break;
                    }
                }
            }

            Managers.GameStateMachine.StartState start = FindFirstObjectByType<Managers.GameStateMachine.StartState>();
            start.StartGameWithoutPlayer();
        }
        #endregion

        #region Game Functions
        [TitleGroup("Game Functions", Order = 1)]
        [ButtonGroup("Game Functions/Buttons")]
        public void SpawnMob()
        {
            if (SceneRefManager.Instance.MobSpawns.Length <= SpawnPointIndex)
            {
                Debug.Log("spawn point index out of range");
                return;
            }
            _spawnManager.SpawnMob(SpawnType, SpawnPointIndex);
        }

        [ButtonGroup("Game Functions/Buttons")]
        public void SendNotification(bool shouldUseVariant)
        {
            _notificationSO.InvokeEvent(_notificationKey, shouldUseVariant);
        }
        #endregion

        #region Player Functions
        [TitleGroup("Player Functions", Order = 2)]

        [ButtonGroup("Player Functions/Buttons")]
        public void GiveStartingItems()
        {
            Debug.Log("adding starter items");
            foreach (ItemEntry item in StarterItems)
            {
                _addItemSO.InvokeEvent(new ItemInstance(item.ItemSO, item.Quantity));
            }
        }

        [ButtonGroup("Player Functions/Buttons")]
        public void GiveRuntimeItems()
        {
            foreach (ItemEntry item in RuntimeItems)
            {
                _addItemSO.InvokeEvent(new ItemInstance(item.ItemSO, item.Quantity));
            }
        }

        [ButtonGroup("Player Functions/Buttons")]
        public void ToggleOnClickDamage()
        {
            _charHitter.Toggle();
        }

        [ButtonGroup("Player Functions/Buttons")]
        public void GiveMoney()
        {
            _changeMoneySO.OnRequestEvent(50);
        }

        #endregion



        #endregion
    }
}
