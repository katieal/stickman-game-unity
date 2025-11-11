using Cysharp.Threading.Tasks;
using EventChannel;
using SaveData;
using Scenes;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Managers.GameStateMachine
{
    /// <summary>
    /// The state the game state machine starts in when game is first loaded
    /// </summary>
    public class StartState : MonoBehaviour, IGameState
    {
        [Title("Invoked Events")]
        [SerializeField] private SceneNameVoidUniTaskEventSO _loadNewSceneEvent;
        [SerializeField] private PlayerStaticDataEventSO _loadStaticDataEvent;
        [SerializeField] private GameStateDataEventSO _loadGameDataEvent;
        [SerializeField] private LocationVoidUniTaskEventSO _spawnSceneObjectsEvent;

        [Title("Listening to Events")]
        [SerializeField] private VoidEventSO _startNewGameEvent;
        [SerializeField] private SaveFileDataEventSO _loadSaveFileEvent;

        #region Interface Fields
        public List<Transition> Transitions { get; set; } = new();
        public StateType Type { get; } = StateType.Start;
        public void SetEnabled(bool enabled) { this.enabled = enabled; }
        #endregion

        private bool _isGameStarting = false;

        private void Awake()
        {
            Transitions.Add(new Transition(StateType.Run, () => _isGameStarting == true));
        }

        private void OnEnable()
        {
            FreezeGame();

            _startNewGameEvent.OnInvokeEvent += OnStartNewGame;
            _loadSaveFileEvent.OnInvokeEvent += OnLoadSaveFile;
        }

        private void OnDisable()
        {
            _isGameStarting = false;

            _startNewGameEvent.OnInvokeEvent -= OnStartNewGame;
            _loadSaveFileEvent.OnInvokeEvent -= OnLoadSaveFile;

            UnfreezeGame();
        }

        #region Callbacks
        private void OnStartNewGame() { StartNewGameAsync().Forget(); }
        private void OnLoadSaveFile(SaveFileData data) { LoadSaveFileAsync(data).Forget(); }
        #endregion

        #region Debug
        public void StartGameWithoutPlayer() { _isGameStarting = true; }
        #endregion

        private async UniTaskVoid StartNewGameAsync()
        {
            /*
             * Order of Events:
             *      - play some sort of opening cutscene
             *      - load a special starting scene
             *      - spawn scene objects
             *      
             *      - start game
             */


            //await _loadNewSceneEvent.InvokeEventAsync(SceneName.Start);
            //await _loadNewSceneEvent.InvokeEventAsync(SceneName.Village); // starting in village for now

            await _spawnSceneObjectsEvent.InvokeEventAsync(Location.Main);

            // start game
            _isGameStarting = true;
        }

        private async UniTaskVoid LoadSaveFileAsync(SaveFileData data)
        {
            /*
             * Order of Events:
             *      - freeze game / loading screen
             *      - get save data
             *      - load correct scene
             *      - set player static data
             *      - set day/time
             *      - load player prefs ?
             *      - spawn player / others
             *      
             *      - start game time
             */

            await UniTask.WaitForEndOfFrame();

            // load current scene
            //SceneName scene = data.GameData.SceneCurrentScene;
            //await _loadNewSceneEvent.InvokeEventAsync(scene);

            //// send game data
            //_loadGameDataEvent.InvokeEvent(data.GameData);

            //// send static data
            //_loadStaticDataEvent.InvokeEvent(data.PlayerStaticData);

            //// spawn scene objects for current scene
            //await _spawnSceneObjectsEvent.InvokeEventAsync(data.GameData.SceneObjects.Data[data.GameData.CurrentScene].PlayerLocation);

            // start game
            _isGameStarting = true;
        }

        #region Freeze Game
        private void FreezeGame()
        {
            // freeze time
            Time.timeScale = 0f;

            // freeze all input
            InputSystem.actions.FindActionMap("Player").Disable();
            InputSystem.actions.FindActionMap("Menu").Disable();
            InputSystem.actions.FindActionMap("UI").Disable();

            // TODO: add in a loading screen?
        }

        private void UnfreezeGame()
        {
            // unfreeze time
            Time.timeScale = 1f;

            // unfreeze all input
            InputSystem.actions.FindActionMap("Player").Enable();
            InputSystem.actions.FindActionMap("Menu").Enable();
            InputSystem.actions.FindActionMap("UI").Enable();
        }
        #endregion
    }
}